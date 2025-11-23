namespace Loupedeck.SimHubIntegrationPlugin.Data
{
    using Loupedeck.SimHubIntegrationPlugin;

    public class DataLoader(String ip)
    {
        private readonly HttpClient httpClient = new();
        private readonly String _ip = ip;

        public String LoadDashboardData()
        {
            var url = $"http://{this._ip}:8080/dashboarddata/";
            try
            {
                var response = this.httpClient.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                var json = response.Content.ReadAsStringAsync().Result;
                PluginLog.Verbose($"Dashboard data loaded from {url}");
                return json;
            }
            catch
            {
                PluginLog.Error($"Failed to load dashboard data from {url}");
                return String.Empty;
            }
        }
    }
}