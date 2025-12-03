namespace Loupedeck.SimHubIntegrationPlugin.Actions
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;

    using Loupedeck;
    using Loupedeck.SimHubIntegrationPlugin.Data;

    /// <summary>
    /// Dynamic command that displays last / fastest lap time and, when pressed,
    /// resets the SimHub target time accordingly via the DashboardDataProviderPlugin.
    /// </summary>
    public class LapTargetCommand : PluginDynamicCommand, Triggers.IDataTrigger
    {
        private enum LapTargetType
        {
            LastLap,
            FastestLap
        }

        public LapTargetCommand()
            : base()
        {
            try
            {
                this.DisplayName = "Lap Target";
                this.Description = "Set target time from last / fastest lap";
                this.GroupName = "Timing";

                // Ensure bindings exist for the lap times we display.
                this.EnsureBinding(EDataKey.LastLapTime);
                this.EnsureBinding(EDataKey.SessionBest);

                // Parameters for last vs fastest.
                this.AddParameter(nameof(LapTargetType.LastLap), "Target = Last lap time", this.GroupName);
                this.AddParameter(nameof(LapTargetType.FastestLap), "Target = Fastest lap time", this.GroupName);
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "LapTargetCommand: Failed to initialize");
                throw;
            }
        }

        private void EnsureBinding(EDataKey key)
        {
            var data = SimHubData.Instance.Data;

            if (!data.TryGetValue(key, out var value))
            {
                PluginLog.Info($"LapTargetCommand: Creating new binding for {key}");
                data[key] = new Binding<dynamic>(this);
            }
            else
            {
                PluginLog.Info($"LapTargetCommand: Adding trigger to existing {key} binding");
                value.AddTrigger(this);
            }
        }

        // Hide default label text to rely solely on custom imagery.
        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => null;

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            try
            {
                if (!Enum.TryParse(actionParameter, out LapTargetType targetType))
                {
                    return null;
                }

                // Determine which SimHub key and label to use.
                var key = targetType == LapTargetType.LastLap
                    ? EDataKey.LastLapTime
                    : EDataKey.SessionBest;

                var label = targetType == LapTargetType.LastLap ? "LAST" : "FAST";

                TimeSpan? lapTime = null;
                if (SimHubData.Instance.Data.TryGetValue(key, out var binding) && binding.Value is TimeSpan ts)
                {
                    lapTime = ts;
                }

                var timeText = lapTime.HasValue ? this.FormatLapTime(lapTime.Value) : "--.-";

                // Determine image size.
                var width = imageSize == PluginImageSize.Width90 ? 90 : 80;
                var height = imageSize == PluginImageSize.Width90 ? 90 : 80;

                using (var bitmap = new Bitmap(width, height))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    // Background: darker for last lap, brighter for best.
                    var backgroundColor = targetType == LapTargetType.LastLap
                        ? Color.FromArgb(40, 40, 40)
                        : Color.FromArgb(0, 80, 0);

                    graphics.Clear(backgroundColor);
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                    // Draw label at top.
                    using (var labelFont = new Font("Segoe UI", 18f, FontStyle.Bold))
                    using (var labelBrush = new SolidBrush(Color.White))
                    {
                        var labelRect = new RectangleF(0, 2, width, height * 0.3f);
                        var labelFormat = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Near
                        };

                        graphics.DrawString(label, labelFont, labelBrush, labelRect, labelFormat);
                    }

                    // Draw lap time centered in remaining area.
                    using (var timeFont = new Font("Segoe UI", 25f, FontStyle.Bold))
                    using (var timeBrush = new SolidBrush(Color.White))
                    {
                        var timeRect = new RectangleF(0, height * 0.25f, width, height * 0.7f);
                        var timeFormat = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };

                        graphics.DrawString(timeText, timeFont, timeBrush, timeRect, timeFormat);
                    }

                    using (var ms = new System.IO.MemoryStream())
                    {
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return ms.ToArray().ToImage();
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "LapTargetCommand: Error in GetCommandImage");
                return null;
            }
        }

        public void Refresh()
        {
            this.ActionImageChanged();
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                if (!Enum.TryParse(actionParameter, out LapTargetType targetType))
                {
                    PluginLog.Error($"LapTargetCommand: Unknown action parameter '{actionParameter}'");
                    return;
                }

                var simHub = SimHubData.Instance;

                Boolean success;
                switch (targetType)
                {
                    case LapTargetType.LastLap:
                        success = simHub.ResetTargetToLastLap();
                        break;
                    case LapTargetType.FastestLap:
                        success = simHub.ResetTargetToFastestLap();
                        break;
                    default:
                        PluginLog.Error($"LapTargetCommand: Unsupported LapTargetType '{targetType}'");
                        return;
                }

                if (!success)
                {
                    PluginLog.Error($"LapTargetCommand: Failed to reset target for '{targetType}'");
                }

                // Trigger immediate visual refresh; polling will update data shortly after.
                this.ActionImageChanged();
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "LapTargetCommand: Error in RunCommand");
            }
        }

        private String FormatLapTime(TimeSpan time)
        {
            // Format as ss.f (e.g., 23.4)
            var seconds = time.Seconds;
            var milliseconds = time.Milliseconds;

            return $"{seconds:00}.{milliseconds / 100:0}";
        }
    }
}
