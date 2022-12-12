using GameOverlay.Drawing;
using irsdkSharp.Serialization.Enums.Fastest;
using System;

namespace iRacingOverlaySuite.Overlays
{
    public class ProximityOverlay : Overlay, IDisposable
    {
        int X;
        int Y;
        int Height;
        int Width;

        public Func<CarLeftRight> GetCarLeftRight;

        int Margin = 5;

        public ProximityOverlay(int x = 0, int y = 0, int width = 960, int height = 100) : base(x, y, width, height)
        {
            X = x;
            Y = y;
            Height = height;
            Width = width;

            GetCarLeftRight = () => IRData.CarLeftRight;

            this.SetupCompleted += ProximityOverlay_SetupCompleted;
        }

        private void ProximityOverlay_SetupCompleted(object? sender, EventArgs e)
        {
            CreateProximityOverlay();
        }

        public void CreateProximityOverlay()
        {
            AddDrawAction((gfx) => gfx.ClearScene(_brushes["background"]));
            AddDrawAction(DrawCarProximityIndicator(X, Y, GetCarLeftRight));
        }

        public Action<Graphics> DrawCarProximityIndicator(int x, int y, Func<CarLeftRight> getCarLeftRight)
        {
            Action<Graphics> drawAction = (gfx) => {
                switch (getCarLeftRight())
                {
                    case (CarLeftRight.LROff):
                        break;
                    case (CarLeftRight.LRCarLeft):
                        DrawLeftTriangle(gfx);
                        break;
                    case (CarLeftRight.LRCarRight):
                        DrawRightTriangle(gfx);
                        break;
                    case (CarLeftRight.LRCarLeftRight):
                        DrawLeftTriangle(gfx);
                        DrawRightTriangle(gfx);
                        break;
                    case (CarLeftRight.LR2CarsLeft):
                        DrawLeftTriangle(gfx);
                        DrawLeftTriangle(gfx, 5);
                        break;
                    case (CarLeftRight.LR2CarsRight):
                        DrawRightTriangle(gfx);
                        DrawRightTriangle(gfx, 5);
                        break;
                    case (CarLeftRight.LRClear):
                        break;
                    default:
                        break;
                }
            };

            return drawAction;
        }

        private void DrawLeftTriangle(Graphics gfx, int offsetX = 0, int offsetY = 0)
        {
            Point pointA = new Point(X + offsetX, Y + (Height / 2));
            Point pointB = new Point(X + offsetX + (Width / 6) - 5, Y + Margin);
            Point pointC = new Point(X + offsetX + (Width / 6) - 5, Y + Height - Margin);
            gfx.DrawTriangle(_brushes["yellow"], new Triangle(pointA, pointB, pointC), 5);
        }

        private void DrawRightTriangle(Graphics gfx, int offsetX = 0, int offsetY = 0)
        {
            Point pointA = new Point(Width - offsetX - Margin, Y + (Height / 2));
            Point pointB = new Point(Width - offsetX - (Width / 6), Y + Margin);
            Point pointC = new Point(Width - offsetX - (Width / 6), Y + Height - Margin);
            gfx.DrawTriangle(_brushes["yellow"], new Triangle(pointA, pointB, pointC), 5);
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
