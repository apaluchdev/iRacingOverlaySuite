using GameOverlay.Drawing;
using irsdkSharp.Serialization.Enums.Fastest;
using System;
using System.Collections.Generic;

namespace iRacingOverlaySuite.Overlays
{
    public class ProximityOverlay : Overlay, IDisposable
    {
        public Func<CarLeftRight>? GetCarLeftRight;

        int Margin = 5;

        public ProximityOverlay(int x, int y, Location location, int width, int height) : base(x, y, location, width, height)
        {
            this.SetupCompleted += ProximityOverlay_SetupCompleted;
        }

        private void ProximityOverlay_SetupCompleted(object? sender, EventArgs e)
        {
            CreateProximityOverlay();
        }

        public void CreateProximityOverlay()
        {
            AddDrawActions(new List<Action<Graphics>>()
            {
                (gfx) => gfx.ClearScene(_brushes["background"]),
                //(gfx) => gfx.DrawCircle(_brushes["transparentBlack"], 0, 0, 1, 5000),
                DrawCarProximityIndicator(GameWindow.width / 2, 0)
            });
        }

        public Action<Graphics> DrawCarProximityIndicator(int x, int y)
        {
            Action<Graphics> drawAction = (gfx) =>
            {
                var carLeftRight = IRData.iRacingData?.CarLeftRight ?? 0;
                switch ((CarLeftRight) carLeftRight)
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
            Point pointA = new Point(offsetX + Margin, Height / 2);
            Point pointB = new Point(offsetX + (Width / 6) - 5 + Margin, Margin);
            Point pointC = new Point(offsetX + (Width / 6) - 5 + Margin, Height - Margin);
            gfx.DrawTriangle(_brushes["yellow"], new Triangle(pointA, pointB, pointC), 5);
        }

        private void DrawRightTriangle(Graphics gfx, int offsetX = 0, int offsetY = 0)
        {
            Point pointA = new Point(Width - offsetX - Margin, Height / 2);
            Point pointB = new Point(Width - offsetX - (Width / 6), Margin);
            Point pointC = new Point(Width - offsetX - (Width / 6), Height - Margin);
            gfx.DrawTriangle(_brushes["yellow"], new Triangle(pointA, pointB, pointC), 5);
        }
    }
}
