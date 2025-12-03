namespace Loupedeck.SimHubIntegrationPlugin.Actions
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;

    using Loupedeck;
    using Loupedeck.SimHubIntegrationPlugin.Data;

    /// <summary>
    /// Dynamic command providing six buttons to adjust the target time by
    /// ±1.0s, ±0.5s, and ±0.1s, rendered as up/down triangles.
    /// </summary>
    public class TargetDeltaAdjustCommand : PluginDynamicCommand
    {
        private enum DeltaVariant
        {
            Increase1_0,
            Decrease1_0,
            Increase0_5,
            Decrease0_5,
            Increase0_1,
            Decrease0_1
        }

        public TargetDeltaAdjustCommand()
            : base()
        {
            try
            {
                this.DisplayName = "Target Δ";
                this.Description = "Adjust target time by fixed deltas";
                this.GroupName = "Timing";

                this.AddParameter(nameof(DeltaVariant.Increase1_0), "+1.0", this.GroupName);
                this.AddParameter(nameof(DeltaVariant.Decrease1_0), "-1.0", this.GroupName);
                this.AddParameter(nameof(DeltaVariant.Increase0_5), "+0.5", this.GroupName);
                this.AddParameter(nameof(DeltaVariant.Decrease0_5), "-0.5", this.GroupName);
                this.AddParameter(nameof(DeltaVariant.Increase0_1), "+0.1", this.GroupName);
                this.AddParameter(nameof(DeltaVariant.Decrease0_1), "-0.1", this.GroupName);
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "TargetDeltaAdjustCommand: Failed to initialize");
                throw;
            }
        }

        // Hide default label text so the triangle graphic stands alone.
        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => null;

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            try
            {
                if (!Enum.TryParse(actionParameter, out DeltaVariant variant))
                {
                    return null;
                }

                var (isUp, stepLabel) = this.GetVisuals(variant);

                var width = imageSize == PluginImageSize.Width90 ? 90 : 80;
                var height = imageSize == PluginImageSize.Width90 ? 90 : 80;

                using (var bitmap = new Bitmap(width, height))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    var backgroundColor = isUp
                        ? Color.FromArgb(30, 60, 30)   // dark greenish for increase
                        : Color.FromArgb(60, 30, 30);  // dark reddish for decrease

                    graphics.Clear(backgroundColor);
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                    // Triangle points.
                    var margin = width * 0.18f;
                    PointF p1, p2, p3;

                    if (isUp)
                    {
                        p1 = new PointF(width / 2f, margin);
                        p2 = new PointF(margin, height - margin);
                        p3 = new PointF(width - margin, height - margin);
                    }
                    else
                    {
                        p1 = new PointF(width / 2f, height - margin);
                        p2 = new PointF(margin, margin);
                        p3 = new PointF(width - margin, margin);
                    }

                    using (var path = new GraphicsPath())
                    using (var triangleBrush = new SolidBrush(Color.White))
                    {
                        path.AddPolygon(new[] { p1, p2, p3 });
                        graphics.FillPath(triangleBrush, path);
                    }

                    // Step label overlay (e.g., 1.0, 0.5, 0.1).
                    using (var font = new Font("Segoe UI", 16f, FontStyle.Bold))
                    using (var textBrush = new SolidBrush(Color.Black))
                    {
                        var labelRect = isUp
                            ? new RectangleF(0, height * 0.55f, width, height * 0.4f)
                            : new RectangleF(0, 0, width, height * 0.45f);

                        var format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };

                        graphics.DrawString(stepLabel, font, textBrush, labelRect, format);
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
                PluginLog.Error(ex, "TargetDeltaAdjustCommand: Error in GetCommandImage");
                return null;
            }
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                if (!Enum.TryParse(actionParameter, out DeltaVariant variant))
                {
                    PluginLog.Error($"TargetDeltaAdjustCommand: Unknown action parameter '{actionParameter}'");
                    return;
                }

                var deltaSeconds = this.GetDelta(variant);
                var success = SimHubData.Instance.AdjustTargetBy(deltaSeconds);

                if (!success)
                {
                    PluginLog.Error($"TargetDeltaAdjustCommand: Failed to adjust target by {deltaSeconds} seconds for '{variant}'");
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "TargetDeltaAdjustCommand: Error in RunCommand");
            }
        }

        private (Boolean isUp, String stepLabel) GetVisuals(DeltaVariant variant)
        {
            switch (variant)
            {
                case DeltaVariant.Increase1_0:
                    return (true, "1.0");
                case DeltaVariant.Decrease1_0:
                    return (false, "1.0");
                case DeltaVariant.Increase0_5:
                    return (true, "0.5");
                case DeltaVariant.Decrease0_5:
                    return (false, "0.5");
                case DeltaVariant.Increase0_1:
                    return (true, "0.1");
                case DeltaVariant.Decrease0_1:
                    return (false, "0.1");
                default:
                    return (true, string.Empty);
            }
        }

        private Double GetDelta(DeltaVariant variant)
        {
            switch (variant)
            {
                case DeltaVariant.Increase1_0:
                    return 1.0;
                case DeltaVariant.Decrease1_0:
                    return -1.0;
                case DeltaVariant.Increase0_5:
                    return 0.5;
                case DeltaVariant.Decrease0_5:
                    return -0.5;
                case DeltaVariant.Increase0_1:
                    return 0.1;
                case DeltaVariant.Decrease0_1:
                    return -0.1;
                default:
                    return 0.0;
            }
        }
    }
}
