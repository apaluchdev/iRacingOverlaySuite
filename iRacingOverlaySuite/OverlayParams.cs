using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingOverlaySuite
{
    public class OverlayParams
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Location Location { get; set; }

        public OverlayParams(int x, int y, int width, int height, Location location = Location.Center)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Location = location;
        }
    }
}
