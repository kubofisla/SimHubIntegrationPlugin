namespace Loupedeck.SimHubIntegrationPlugin.Data
{
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;

    using Loupedeck.SimHubIntegrationPlugin;

    /// <summary>
    /// HTTP client for communicating with the DashboardDataProviderPlugin.
    /// Wraps GET for telemetry and POST operations for target time control.
    /// </summary>
    public class DataLoader
    {
        private readonly HttpClient _httpClient;
        private readonly String _ip;

        public DataLoader(String ip)
            : this(ip, new HttpClient())
        {
        }

        internal DataLoader(String ip, HttpClient httpClient)
        {
            this._ip = ip ?? throw new ArgumentNullException(nameof(ip));
            this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        private String GetBaseUrl() => $"http://{this._ip}:8080/dashboarddata/";

        public String LoadDashboardData()
        {
            var url = this.GetBaseUrl();
            try
            {
                var response = this._httpClient.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                var json = response.Content.ReadAsStringAsync().Result;
                PluginLog.Verbose($"Dashboard data loaded from {url}");
                return json;
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, $"Failed to load dashboard data from {url}");
                return String.Empty;
            }
        }

        /// <summary>
        /// Reset target time to fastest lap via POST /dashboarddata/resettofast.
        /// </summary>
        public Boolean ResetTargetToFastestLap() => this.SendPost("resettofast", null);

        /// <summary>
        /// Reset target time to last lap via POST /dashboarddata/resettolast.
        /// </summary>
        public Boolean ResetTargetToLastLap() => this.SendPost("resettolast", null);

        /// <summary>
        /// Adjust target time by the given delta in seconds via POST /dashboarddata/adjust.
        /// </summary>
        public Boolean AdjustTargetBy(Double deltaSeconds)
        {
            var body = new { delta = deltaSeconds };
            return this.SendPost("adjust", body);
        }

        private Boolean SendPost(String relativePath, Object body)
        {
            var url = this.GetBaseUrl() + relativePath;
            try
            {
                HttpContent content = null;

                if (body != null)
                {
                    var json = JsonSerializer.Serialize(body);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                }
                else
                {
                    // Some endpoints expect no body; send an empty JSON payload.
                    content = new StringContent(String.Empty, Encoding.UTF8, "application/json");
                }

                var response = this._httpClient.PostAsync(url, content).Result;
                if (!response.IsSuccessStatusCode)
                {
                    PluginLog.Error($"POST {url} failed with status code {(Int32)response.StatusCode} ({response.StatusCode}).");
                    return false;
                }

                PluginLog.Info($"POST {url} succeeded.");
                return true;
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, $"POST {url} failed.");
                return false;
            }
        }
    }
}
