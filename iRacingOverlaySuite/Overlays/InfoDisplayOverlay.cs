using GameOverlay.Drawing;
using System.Collections.Generic;
using System;
using System.Windows.Media.Media3D;

namespace iRacingOverlaySuite.Overlays
{
    internal class InfoDisplayOverlay : IOverlayDrawer
    {
        private OverlayCanvas _canvas;

        private float _trackTemp = -99f;

        public InfoDisplayOverlay(int x, int y)
        {
            var _overlayParams = new OverlayParams(0, 0, 300, 400, Location.TopLeft);

            _canvas = new OverlayCanvas(_overlayParams, this);

            _canvas.DrawGrid = true;
            _canvas.Run();
        }

        public void DrawOverlay()
        {
            string trackDelta = String.Empty;

            if (_trackTemp != -99f)
                trackDelta = _trackTemp < (IRData.iRacingData?.TrackTemp ?? -99f) ? "↑" : "↓";

            _trackTemp = IRData.iRacingData?.TrackTemp ?? -99f;
            var brush = _canvas.Brushes;
            var fonts = _canvas.Fonts;

            var trackTempColor = brush["white"];

            if (_trackTemp > 50f)
                trackTempColor = brush["red"];
            else if (_trackTemp > 40f)
                trackTempColor = brush["orange"];
            else if (_trackTemp > 30f)
                trackTempColor = brush["yellow"];
            else if (_trackTemp > 20f)
                trackTempColor = brush["green"];
            else if (_trackTemp > -10f)
                trackTempColor = brush["aqua"];

            _canvas.AddDrawActions(
                new List<Action<Graphics>>()
                {
                    (gfx) =>
                    {
                        gfx.DrawTextWithBackground(fonts["calibri"], 22, trackTempColor, brush["black"], 0, 0, $"Track Temperature: {_trackTemp}°C {trackDelta}");
                    }
                }        
            );
        }
    }
}
