namespace SimHubIntegrationPlugin.Data
{
    using Loupedeck.SimHubIntegrationPlugin;
    using Loupedeck.SimHubIntegrationPlugin.Data;
    public class SimHubData
    {
        private String _lastRawData;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _monitorTask;
        private readonly DataLoader _dataLoader;
        private static readonly Lazy<SimHubData> _lazy = new(() => new SimHubData());
        public static SimHubData Instance => _lazy.Value;
        public readonly Dictionary<EDataKey, Binding<dynamic>> Data = new();

        public SimHubData()
        {
            this._dataLoader = new DataLoader("localhost");
            this.StartMonitoring();
        }

        public void StartMonitoring()
        {
            PluginLog.Info("Starting SimHub data monitoring...");
            this._cancellationTokenSource = new CancellationTokenSource();
            this._monitorTask = Task.Run(async () =>
            {
                while (!this._cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var newValue = this._dataLoader.LoadDashboardData();
                    if (!Equals(newValue, this._lastRawData))
                    {
                        PluginLog.Verbose($"New data received: {newValue}");
                        // Handle new value (e.g., update _data or raise event)
                        this._lastRawData = newValue;
                        this.ProcessNewData(newValue);
                    }
                    await Task.Delay(1000, this._cancellationTokenSource.Token);
                }
            }, this._cancellationTokenSource.Token);
        }

        public void StopMonitoring()
        {
            this._cancellationTokenSource?.Cancel();
            this._monitorTask?.Wait();
            PluginLog.Info("SimHub data monitoring stopped...");
}

        private void ProcessNewData(String newValue)
        {
            Response response = new(newValue);
            var dataDict = response.Parse();
            foreach (var pair in dataDict)
            {
                try
                {
                    Enum.TryParse<EDataKey>(pair.Key, out var eValue);
                    if (this.Data.TryGetValue(eValue, out var existingData))
                    {
                        var newDataValue = EDataKeyMapping.Mapping[eValue].ParseFunc(pair.Value);
                        if (!Equals(existingData.Value, newDataValue))
                        {
                            existingData.Value = newDataValue;
                        }
                    }
                }
                catch (ArgumentNullException ex)
                {
                    PluginLog.Error($"Failed to parse data for key {pair.Key}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    PluginLog.Error(ex, $"Unexpected error while processing data for key {pair.Key}");
                    throw;
                }
                
            }
        }
    }
}