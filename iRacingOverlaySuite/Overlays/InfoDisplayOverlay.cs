using GameOverlay.Drawing;
using System.Collections.Generic;
using System;
using System.Windows.Media.Media3D;

namespace iRacingOverlaySuite.Overlays
{
    internal class InfoDisplayOverlay : iRacingOverlay, IOverlayDrawer
    {
        public InfoDisplayOverlay(int width, int height, Location location = Location.TopLeft, int x = 0, int y = 0) : base (x, y, width, height, location)
        {
        }

        public void SetupOverlay()
        {
            Action<Graphics> infoDisplayOverlayAction = new Action<Graphics>((gfx) =>
            {
                gfx.DrawTextWithBackground(fonts["calibri"], 22, GetTrackTemperatureColor(), brushes["black"], 0, 0, $"Track Temperature: {GetTrackTemperature().ToString("0.0")}°C");
            });

            _canvas.AddDrawAction(infoDisplayOverlayAction);
        }

        private float GetTrackTemperature()
        {
            return IRData.iRacingData?.TrackTemp ?? (float)Math.Abs(Math.Sin(DateTime.Now.Second)*50);
        }

        private SolidBrush GetTrackTemperatureColor()
        {
            var temp = GetTrackTemperature();

            if (temp > 50f)
                return brushes["red"];
            else if (temp > 40f)
                return brushes["orange"];
            else if (temp > 30f)
                return brushes["yellow"];
            else if (temp > 20f)
                return brushes["green"];
            else if (temp > -10f)
                return brushes["aqua"];

            return brushes["white"];
        }
    }
}
