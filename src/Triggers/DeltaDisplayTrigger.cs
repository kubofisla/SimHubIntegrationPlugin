namespace Loupedeck.SimHubIntegrationPlugin.Triggers
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using Loupedeck;
    using Loupedeck.SimHubIntegrationPlugin;
    using Loupedeck.SimHubIntegrationPlugin.Data;
    using Loupedeck.SimHubIntegrationPlugin.Helpers;

    public class DeltaDisplayTrigger : PluginDynamicCommand, IDataTrigger
    {
        private double _lastDeltaValue = 0.0;

        private enum DeltaDigits
        {
            Sign,
            SecondsWithDecimal,
            Milliseconds
        }

        public DeltaDisplayTrigger() : base()
        {
            try
            {
                this.DisplayName = "Delta";
                this.Description = "Displays delta time across 3 boxes: Sign | Seconds. | Milliseconds";
                this.GroupName = "Racing";
                
                PluginLog.Info("DeltaDisplayTrigger: Initializing...");
                
                // Register this trigger to receive delta data updates
                if (!SimHubData.Instance.Data.ContainsKey(EDataKey.SessionBestLiveDeltaSeconds))
                {
                    PluginLog.Info("DeltaDisplayTrigger: Creating new binding for SessionBestLiveDeltaSeconds");
                    SimHubData.Instance.Data[EDataKey.SessionBestLiveDeltaSeconds] = new Binding<dynamic>(this);
                }
                else
                {
                    // Add this trigger to the existing binding
                    PluginLog.Info("DeltaDisplayTrigger: Adding trigger to existing SessionBestLiveDeltaSeconds binding");
                    SimHubData.Instance.Data[EDataKey.SessionBestLiveDeltaSeconds].AddTrigger(this);
                }
                
                // Add parameters for each box
                this.AddParameter(nameof(DeltaDigits.Sign), String.Empty, "Delta time - sign");
                this.AddParameter(nameof(DeltaDigits.SecondsWithDecimal), String.Empty, "Delta time - seconds");
                this.AddParameter(nameof(DeltaDigits.Milliseconds), String.Empty, "Delta time - miliseconds");
                
                PluginLog.Info("DeltaDisplayTrigger: Initialization complete");
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "DeltaDisplayTrigger: Failed to initialize");
                throw;
            }
        }

        //TODO this not works as expected, label is still shown and it's equal to DisplayName
        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            // Return null to completely hide the label text at the bottom
            return null;
        }


        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
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
                
                // Get the text to display based on parameter
                string displayText = "?";
                switch (actionParameter)
                {
                    case nameof(DeltaDigits.Sign):
                        displayText = deltaDisplay.Sign;
                        break;
                    case nameof(DeltaDigits.SecondsWithDecimal):
                        displayText = deltaDisplay.Seconds;
                        break;
                    case nameof(DeltaDigits.Milliseconds):
                        displayText = deltaDisplay.Thousandths;
                        break;
                }
                //TODO Refactor this part of the code to use a common method for generating images with text
                //TODO Parameters of image with text are scattered on multiple levels here. Make it more readable. Define on one level, use utility for conditional logic.
                // Get background color
                Color backgroundColor = ColorTranslator.FromHtml(deltaDisplay.GetBackgroundColor());
                
                // Create bitmap based on image size
                int width = imageSize == PluginImageSize.Width90 ? 90 : 80;
                int height = imageSize == PluginImageSize.Width90 ? 90 : 80;
                
                using (var bitmap = new Bitmap(width, height))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    // Fill background
                    graphics.Clear(backgroundColor);
                    
                    // Set up text rendering with large font
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                    // Use different font sizes based on parameter
                    float fontSize = 31f;
                    FontStyle fontStyle = FontStyle.Bold;
                    
                    // Make sign bigger and bold
                    if (actionParameter == nameof(DeltaDigits.Sign))
                    {
                        fontSize = 42f;
                    }
                    
                    var rect = new RectangleF(0, 0, width, height);

                    using (var font = new Font("Segoe UI", fontSize, fontStyle))
                    using (var textBrush = new SolidBrush(Color.Black))
                    {
                        // Set alignment based on parameter
                        var format = new StringFormat
                        {
                            Alignment = actionParameter == nameof(DeltaDigits.SecondsWithDecimal)
                                ? StringAlignment.Far  // Right-align seconds
                                : StringAlignment.Center,  // Center sign and milliseconds
                            LineAlignment = StringAlignment.Center
                        };

                        graphics.DrawString(displayText, font, textBrush, rect, format);
                    }

                    // Convert bitmap to byte array and return as BitmapImage
                    using (var ms = new System.IO.MemoryStream())
                    {
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return ms.ToArray().ToImage();
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Error in GetCommandImage");
                return null;
            }
        }

        public void Refresh()
        {
            this.ActionImageChanged();
        }
    }
}
