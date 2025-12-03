namespace Loupedeck.SimHubIntegrationPlugin.Actions
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;

    using Loupedeck;
    using Loupedeck.SimHubIntegrationPlugin.Data;

    /// <summary>
    /// Dynamic command that displays the current target time.
    /// </summary>
    public class TargetTimeCommand : PluginDynamicCommand, Triggers.IDataTrigger
    {
        public TargetTimeCommand()
            : base()
        {
            try
            {
                this.DisplayName = "Target";
                this.Description = "Displays the current target time";
                this.GroupName = "Timing";

                this.AddParameter("Default", "Target Time", this.GroupName);

                this.EnsureBinding();
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "TargetTimeCommand: Failed to initialize");
                throw;
            }
        }

        private void EnsureBinding()
        {
            var data = SimHubData.Instance.Data;
            var key = EDataKey.TargetTime;

            if (!data.TryGetValue(key, out var value))
            {
                PluginLog.Info($"TargetTimeCommand: Creating new binding for {key}");
                data[key] = new Binding<dynamic>(this);
            }
            else
            {
                PluginLog.Info($"TargetTimeCommand: Adding trigger to existing {key} binding");
                value.AddTrigger(this);
            }
        }

        // Hide default label text to rely solely on custom imagery.
        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => null;

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            try
            {
                TimeSpan? targetTime = null;
                if (SimHubData.Instance.Data.TryGetValue(EDataKey.TargetTime, out var binding) && binding.Value is TimeSpan ts)
                {
                    targetTime = ts;
                }

                var timeText = targetTime.HasValue ? this.FormatTime(targetTime.Value) : "-.--:-";

                // Determine image size.
                var width = imageSize == PluginImageSize.Width90 ? 90 : 80;
                var height = imageSize == PluginImageSize.Width90 ? 90 : 80;

                using (var bitmap = new Bitmap(width, height))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    // Background: Dark blueish/neutral
                    var backgroundColor = Color.FromArgb(20, 20, 40);

                    graphics.Clear(backgroundColor);
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                    // Draw label at top.
                    using (var labelFont = new Font("Segoe UI", 16f, FontStyle.Bold))
                    using (var labelBrush = new SolidBrush(Color.LightGray))
                    {
                        var labelRect = new RectangleF(0, 2, width, height * 0.3f);
                        var labelFormat = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Near
                        };

                        graphics.DrawString("TARGET", labelFont, labelBrush, labelRect, labelFormat);
                    }

                    // Draw time centered in remaining area.
                    using (var timeFont = new Font("Segoe UI", 20f, FontStyle.Bold))
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
                PluginLog.Error(ex, "TargetTimeCommand: Error in GetCommandImage");
                return null;
            }
        }

        public void Refresh()
        {
            this.ActionImageChanged();
        }

        protected override void RunCommand(String actionParameter)
        {
            // No action needed on press, just refresh visual
            this.ActionImageChanged();
        }

        private String FormatTime(TimeSpan time)
        {
            // Format as m:ss.f (e.g., 1:23.4)
            // If it's less than a minute, maybe just ss.f? 
            // The user asked for "-.--:-", which is a bit ambiguous but likely means minutes:seconds.tenths

            // Let's stick to a standard lap time format: m:ss.f
            return $"{time.Minutes}:{time.Seconds:00}.{time.Milliseconds / 100:0}";
        }
    }
}
