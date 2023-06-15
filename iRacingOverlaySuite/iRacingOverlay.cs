using GameOverlay.Drawing;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace iRacingOverlaySuite
{
    internal abstract class iRacingOverlay : IOverlayDrawer
    {
        protected OverlayCanvas _canvas;

        protected Dictionary<string, SolidBrush> brushes;
        protected Dictionary<string, Font> fonts;

        public iRacingOverlay(int x, int y, int width, int height, Location location)
        {
            var _overlayParams = new OverlayParams(0, 0, width, height, location);

            _canvas = new OverlayCanvas(_overlayParams, this);
            
            brushes = _canvas.Brushes;
            fonts = _canvas.Fonts;

            _canvas.DrawGrid = false;
            _canvas.Run();
        }

        public virtual void SetupOverlay()
        {
            throw new NotImplementedException();
        }
    }
}
