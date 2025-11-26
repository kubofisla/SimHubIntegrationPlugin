namespace Loupedeck.SimHubIntegrationPlugin.Helpers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Formats delta time for display on a 3x4 matrix (4 squares in middle row).
    /// Box 1: Sign (+/-)
    /// Box 2: Seconds digit
    /// Box 3-4: Thousandths (3 digits displayed across 2 boxes)
    /// </summary>
    public class DeltaDisplay
    {
        public string Sign { get; set; }
        public string Seconds { get; set; }
        public string Thousandths { get; set; }
        public string Empty { get; set; }
        
        public bool IsPositive { get; set; }
        public bool IsNegative { get; set; }
        public bool IsZero { get; set; }
        public double AbsoluteDelta { get; set; }

        /// <summary>
        /// Format delta seconds into display components.
        /// Example: -1.234 => Sign: "-", Seconds: "1.", Thousandths: "234", Color: Yellow
        /// Example: +0.567 => Sign: "+", Seconds: "0.", Thousandths: "567", Color: Green
        /// </summary>
        public static DeltaDisplay FromSeconds(double deltaSeconds)
        {
            var display = new DeltaDisplay();
            
            if (deltaSeconds == 0)
            {
                display.Sign = " ";
                display.Seconds = "0.";
                display.Thousandths = "000";
                display.Empty = "";
                display.IsZero = true;
                display.IsPositive = false;
                display.IsNegative = false;
                display.AbsoluteDelta = 0;
                return display;
            }

            display.IsNegative = deltaSeconds < 0;
            display.IsPositive = deltaSeconds > 0;
            
            // Get absolute value for formatting
            double absDelta = Math.Abs(deltaSeconds);
            display.AbsoluteDelta = absDelta;
            
            // Sign
            display.Sign = display.IsNegative ? "−" : "+";
            
            // Extract seconds and milliseconds
            int seconds = (int)Math.Floor(absDelta);
            int milliseconds = (int)Math.Round((absDelta - seconds) * 1000);
            
            // Handle rounding edge case (e.g., 0.9999 rounds to 1.0)
            if (milliseconds >= 1000)
            {
                milliseconds = 0;
                seconds += 1;
            }
            
            display.Seconds = seconds.ToString() + ".";
            display.Thousandths = milliseconds.ToString("D3"); // Pad to 3 digits
            display.Empty = "";
            
            return display;
        }

        /// <summary>
        /// Gets CSS color based on delta magnitude.
        /// Green: within ±0.25s
        /// Yellow: ±0.25s to ±0.5s
        /// Orange: ±0.5s to ±1.0s
        /// Red: ±1.0s or more
        /// </summary>
        public string GetBackgroundColor()
        {
            if (this.IsZero)
                return "#00AA00"; // Green for on target
            
            if (this.AbsoluteDelta <= 0.25)
                return "#00AA00"; // Green
            else if (this.AbsoluteDelta <= 0.5)
                return "#FFD700"; // Yellow
            else if (this.AbsoluteDelta <= 1.0)
                return "#FFA500"; // Orange
            else
                return "#FF0000"; // Red
        }
    }
}
