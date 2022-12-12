using GameOverlay.Drawing;
using GameOverlay.Windows;
using irsdkSharp.Serialization.Enums.Fastest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;
using YamlDotNet.RepresentationModel;

namespace iRacingOverlaySuite.Overlays
{
    public class InputOverlay : Overlay, IDisposable
    {
        int X;
        int Y;
        int Height;
        int Width;

        public Func<float> GetBrake;
        public Func<float> GetThrottle;
        public Func<CarLeftRight> GetCarLeftRight;

        int Margin = 2;

        public InputOverlay(int x = 0, int y = 0, int width = 600, int height = 200) : base(x, y, width, height)
        {
            X = x;
            Y = y;
            Height = height;
            Width = width;

            GetBrake = () => IRData.Brake;
            GetThrottle = () => IRData.Throttle;

            this.SetupCompleted += InputOverlay_SetupCompleted;
        }

        private void InputOverlay_SetupCompleted(object? sender, EventArgs e)
        {
            CreateInputOverlay();
        }

        public void CreateInputOverlay()
        {
            AddDrawAction((gfx) => gfx.ClearScene(_brushes["background"]));
            AddDrawAction(DrawPercentageBar(Width - Margin, Height - Margin, -20, Height, _brushes["green"], GetThrottle));
            AddDrawAction(DrawPercentageBar(X + Margin, Height - Margin, 20, Height, _brushes["red"], GetBrake));
        }

        private void DrawLeftTriangle(Graphics gfx, int x, int y)
        {
            Point pointA = new Point(x, y + (Height / 2));
            Point pointB = new Point(x + (Width / 2) - 5, y);
            Point pointC = new Point(x + (Width / 2) - 5, y + Height);
            gfx.DrawTriangle(_brushes["red"], new Triangle(pointA, pointB, pointC), 5);
        }

        public Action<Graphics> DrawPercentageBar(int x, int y, int width, int height, IBrush color, Func<float> getPercentage)
        {
            Action<Graphics> drawAction = (gfx) => {
                // Dimensions of inner container that changes height depending on the percentage
                Rectangle filling = Rectangle.Create(/* Top Left X */ x, /* Top Left Y */ y, width, -((height - Margin) * getPercentage()));
                // Container dimensions
                Rectangle container = Rectangle.Create(/* Top Left X */ x, /* Top Left Y */ y, width, -(height - Margin));

                if (getPercentage() > 0)
                {
                    gfx.DrawBox2D(_brushes["black"], color, filling, 2);
                }

                gfx.DrawRectangle(_brushes["black"], container, 2);
            };

            return drawAction;
        }
    }
}
