using GameOverlay.Drawing;
using GameOverlay.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;

namespace iRacingOverlaySuite.Overlays
{
    public class InputOverlay : Overlay, IDisposable
    {
        int X;
        int Y;
        int Width;
        int Height;

        public InputOverlay(int x = 0, int y = 0, int width = 300, int height = 300) : base(x, y, width, height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            CreateInputOverlay();
        }

        public void CreateInputOverlay()
        {
            AddDrawAction((gfx) => gfx.DrawRectangle(_brushes["red"], X + 10, Y + 10, X + 110, Y + 110, 20.0f));
            AddDrawAction((gfx) => gfx.DrawText(_fonts["consolas"], _brushes["white"], X, Y, IRData.DriverId.ToString()));
            AddDrawAction((gfx) => gfx.DrawText(_fonts["consolas"], _brushes["white"], X, Y+15, IRData.TrackName.ToString()));
        }
    }
}
