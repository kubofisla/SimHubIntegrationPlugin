namespace Loupedeck.SimHubIntegrationPlugin.Triggers
{
    using System;
    using System.Drawing;
    using Loupedeck;
    using Loupedeck.SimHubIntegrationPlugin;
    using Loupedeck.SimHubIntegrationPlugin.Data;
    using Loupedeck.SimHubIntegrationPlugin.Helpers;
    using global::SimHubIntegrationPlugin.Data;

    public class DeltaDisplayTrigger : PluginDynamicCommand, IDataTrigger
    {
        private double _lastDeltaValue = 0.0;

        public DeltaDisplayTrigger() : base()
        {
            this.DisplayName = "Delta Display";
            this.Description = "Displays delta time across 4 boxes: Sign | Seconds | Thousandths | Empty";
            this.GroupName = "Racing";
            
            // Ensure delta data exists
            if (!SimHubData.Instance.Data.ContainsKey(EDataKey.SessionBestLiveDeltaSeconds))
            {
                SimHubData.Instance.Data[EDataKey.SessionBestLiveDeltaSeconds] = new Binding<dynamic>(null);
            }
            
            this.AddParameter("deltaDisplay", String.Empty, "Delta Display");
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            try
            {
                // Get current delta from SimHub data
                if (SimHubData.Instance.Data.TryGetValue(EDataKey.SessionBestLiveDeltaSeconds, out var binding))
                {
                    if (binding.Value is TimeSpan timespan)
                    {
                        _lastDeltaValue = timespan.TotalSeconds;
                    }
                    else if (binding.Value is double doubleVal)
                    {
                        _lastDeltaValue = doubleVal;
                    }
                }
                
                var deltaDisplay = DeltaDisplay.FromSeconds(_lastDeltaValue);
                var thousandthsParts = SplitThousandths(deltaDisplay.Thousandths);
                
                // Return formatted delta display
                return $"{deltaDisplay.Sign} {deltaDisplay.Seconds} {thousandthsParts[0]}.{thousandthsParts[1]}";
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error in GetCommandDisplayName: {ex.Message}");
                return "Error";
            }
        }

        private string[] SplitThousandths(string thousandths)
        {
            if (thousandths.Length >= 2)
                return new[] { thousandths.Substring(0, 2), thousandths.Substring(2) };
            else
                return new[] { thousandths, "" };
        }

        private Color HexToColor(string hex)
        {
            // Remove # if present
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length == 6)
            {
                int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                return Color.FromArgb(r, g, b);
            }
            return Color.Gray;
        }

        public void Refresh()
        {
            this.ActionImageChanged();
        }
    }
}
