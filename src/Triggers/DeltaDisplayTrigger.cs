namespace Loupedeck.SimHubIntegrationPlugin.Triggers
{
    using System;
    using System.Collections.Generic;
    using Loupedeck;
    using Loupedeck.SimHubIntegrationPlugin;
    using Loupedeck.SimHubIntegrationPlugin.Data;
    using Loupedeck.SimHubIntegrationPlugin.Helpers;

    public class DeltaDisplayTrigger : VisibleDynamicCommand
    {
        private DeltaDisplay _lastDeltaDisplay;

        public DeltaDisplayTrigger()
            : base(
                displayName: "Delta Display",
                description: "Displays delta time across 4 boxes: Sign | Seconds | Thousandths | Empty")
        {
            // Subscribe to data changes
            SimHubData.Instance.Data.TryAdd(EDataKey.SessionBestLiveDeltaSeconds, 
                new Binding<dynamic>(0.0));
            
            // Register for updates
            this.AddParameter("deltaDisplay", String.Empty, "Delta Display Parameters");
        }

        public override void OnLoad()
        {
            // Hook into data changes
            this.SetImage(this.GenerateDeltaImage(0.0));
        }

        private PluginImage GenerateDeltaImage(double deltaSeconds)
        {
            var deltaDisplay = DeltaDisplay.FromSeconds(deltaSeconds);
            _lastDeltaDisplay = deltaDisplay;

            var image = new PluginImage();
            var canvas = new BitmapCanvas(240, 80);

            // Background
            canvas.FillRectangle(0, 0, 240, 80, deltaDisplay.GetBackgroundColor());

            // Layout: 4 boxes, each 60x80 pixels (60 width for each box)
            int boxWidth = 60;
            int boxHeight = 80;
            int fontSize = 40;
            int fontSizeSmall = 32;

            // Box 1: Sign
            DrawBox(canvas, 0, 0, boxWidth, boxHeight, deltaDisplay.Sign, fontSize);

            // Box 2: Seconds
            DrawBox(canvas, boxWidth, 0, boxWidth, boxHeight, deltaDisplay.Seconds, fontSize);

            // Box 3-4: Thousandths (display "234" across 2 boxes)
            var thousandthsParts = SplitThousandths(deltaDisplay.Thousandths);
            DrawBox(canvas, boxWidth * 2, 0, boxWidth, boxHeight, thousandthsParts[0], fontSizeSmall);
            DrawBox(canvas, boxWidth * 3, 0, boxWidth, boxHeight, thousandthsParts[1], fontSizeSmall);

            image.SetBitmapImage(canvas.GetBitmap());
            return image;
        }

        private void DrawBox(BitmapCanvas canvas, int x, int y, int width, int height, string text, int fontSize)
        {
            // Draw border
            canvas.DrawRectangle(x, y, width, height, BitmapColor.White, 2);

            // Draw text centered
            var textSize = canvas.MeasureText(text, fontSize);
            int textX = x + (width - textSize.Width) / 2;
            int textY = y + (height - textSize.Height) / 2;

            canvas.DrawText(text, textX, textY, width, height, BitmapColor.White, fontSize);
        }

        /// <summary>
        /// Split 3-digit thousandths across 2 boxes.
        /// "234" => ["23", "4"] or ["2", "34"] 
        /// Using ["23", "4"] for better visual balance
        /// </summary>
        private string[] SplitThousandths(string thousandths)
        {
            if (thousandths.Length >= 2)
                return new[] { thousandths.Substring(0, 2), thousandths.Substring(2) };
            else
                return new[] { thousandths, "" };
        }

        public override void RunCommand(String actionParameter)
        {
            // This is a display trigger, no command execution needed
        }
    }
}
