using GameOverlay.Drawing;
using System.Collections.Generic;
using System;
using System.Windows.Media.Media3D;
using irsdkSharp.Serialization.Enums.Fastest;

namespace iRacingOverlaySuite.Overlays
{
    internal class ProximityOverlay : IOverlayDrawer
    {
        private OverlayCanvas _canvas;

        const int BAR_WIDTH = 19;

        public ProximityOverlay(int x, int y)
        {
            var _overlayParams = new OverlayParams(0, 0, 600, 100, Location.Center);

            _canvas = new OverlayCanvas(_overlayParams, this);

            _canvas.DrawGrid = true;
            _canvas.Run();
        }

        public void DrawOverlay()
        {

            var brush = _canvas.Brushes;

            // When steering exceeds 5 degrees (0.08 rad), use orange to indicate trail braking should be done.
            var brakeColor = Math.Abs(IRData.iRacingData?.SteeringWheelAngle ?? 0f) < Math.Abs(0.08) ? brush["red"] : brush["orange"];

            _canvas.AddDrawActions(
                new List<Action<Graphics>>()
                {
                    (gfx) =>
                    {
                        DrawCarProximityIndicator(gfx);
                    }
                }
            );
        }

        private void DrawCarProximityIndicator(Graphics gfx)
        {
            var carProximity = IRData.iRacingData?.CarLeftRight ?? (int)CarLeftRight.LRCarLeft;

            switch ((CarLeftRight)carProximity)
            {
                case (CarLeftRight.LROff):
                    break;
                case (CarLeftRight.LRCarLeft):
                    DrawLeftIndicator(gfx);
                    break;
                case (CarLeftRight.LRCarRight):
                    DrawRightIndicator(gfx);
                    break;
                case (CarLeftRight.LRCarLeftRight):
                    DrawLeftIndicator(gfx);
                    DrawRightIndicator(gfx);
                    break;
                case (CarLeftRight.LR2CarsLeft):
                    DrawLeftIndicator(gfx);
                    DrawLeftIndicator(gfx);
                    break;
                case (CarLeftRight.LR2CarsRight):
                    DrawRightIndicator(gfx);
                    DrawRightIndicator(gfx);
                    break;
                default:
                    break;
            }
        }

        private void DrawClearIndicator(Graphics gfx)
        {
            gfx.DrawCrosshair(_canvas.Brushes["blue"], _canvas.CenterX, _canvas.CenterY, 25f, 2f, CrosshairStyle.Plus);
        }

        private void DrawLeftIndicator(Graphics gfx)
        {
            var pointA = new Point(_canvas.CenterX - (_canvas.Width / 2.5f), _canvas.Y1 + 10);
            var pointB = new Point(_canvas.CenterX - (_canvas.Width / 2.5f), _canvas.Y2 - 10);
            var pointC = new Point(_canvas.X1 + 5, _canvas.CenterY);

            var leftTriangle = new Triangle(pointA, pointB, pointC);
            gfx.DrawTriangle(_canvas.Brushes["yellow"], leftTriangle, 3f);
        }

        private void DrawRightIndicator(Graphics gfx)
        {
            var pointA = new Point(_canvas.CenterX + (_canvas.Width / 2.5f), _canvas.Y1 + 10);
            var pointB = new Point(_canvas.CenterX + (_canvas.Width / 2.5f), _canvas.Y2 - 10);
            var pointC = new Point(_canvas.X2 - 5, _canvas.CenterY);

            var rightTriangle = new Triangle(pointA, pointB, pointC);
            gfx.DrawTriangle(_canvas.Brushes["yellow"], rightTriangle, 3f);
        }
    }
}









//using GameOverlay.Drawing;
//using irsdkSharp.Serialization.Enums.Fastest;
//using System;
//using System.Collections.Generic;

//namespace iRacingOverlaySuite.Overlays
//{
//    public class ProximityOverlay : Overlay, IDisposable
//    {
//        public Func<CarLeftRight>? GetCarLeftRight;

//        int Margin = 5;

//        public ProximityOverlay(int x, int y, Location location, int width, int height) : base(x, y, location, width, height)
//        {
//            this.SetupCompleted += ProximityOverlay_SetupCompleted;
//        }

//        private void ProximityOverlay_SetupCompleted(object? sender, EventArgs e)
//        {
//            CreateProximityOverlay();
//        }

//        public void CreateProximityOverlay()
//        {
//            AddDrawActions(new List<Action<Graphics>>()
//            {
//                (gfx) => gfx.ClearScene(_brushes["background"]),
//                //(gfx) => gfx.DrawCircle(_brushes["transparentBlack"], 0, 0, 1, 5000),
//                DrawCarProximityIndicator(GameWindow.width / 2, 0)
//            });
//        }

//        public Action<Graphics> DrawCarProximityIndicator(int x, int y)
//        {
//            Action<Graphics> drawAction = (gfx) =>
//            {
//                var carLeftRight = IRData.iRacingData?.CarLeftRight ?? 0;
//                switch ((CarLeftRight) carLeftRight)
//                {
//                    case (CarLeftRight.LROff):
//                        break;
//                    case (CarLeftRight.LRCarLeft):
//                        DrawLeftTriangle(gfx);
//                        break;
//                    case (CarLeftRight.LRCarRight):
//                        DrawRightTriangle(gfx);
//                        break;
//                    case (CarLeftRight.LRCarLeftRight):
//                        DrawLeftTriangle(gfx);
//                        DrawRightTriangle(gfx);
//                        break;
//                    case (CarLeftRight.LR2CarsLeft):
//                        DrawLeftTriangle(gfx);
//                        DrawLeftTriangle(gfx, 5);
//                        break;
//                    case (CarLeftRight.LR2CarsRight):
//                        DrawRightTriangle(gfx);
//                        DrawRightTriangle(gfx, 5);
//                        break;
//                    case (CarLeftRight.LRClear):
//                        break;
//                    default:
//                        break;
//                }
//            };

//            return drawAction;
//        }

//        private void DrawLeftTriangle(Graphics gfx, int offsetX = 0, int offsetY = 0)
//        {
//            Point pointA = new Point(offsetX + Margin, Height / 2);
//            Point pointB = new Point(offsetX + (Width / 6) - 5 + Margin, Margin);
//            Point pointC = new Point(offsetX + (Width / 6) - 5 + Margin, Height - Margin);
//            gfx.DrawTriangle(_brushes["yellow"], new Triangle(pointA, pointB, pointC), 5);
//        }

//        private void DrawRightTriangle(Graphics gfx, int offsetX = 0, int offsetY = 0)
//        {
//            Point pointA = new Point(Width - offsetX - Margin, Height / 2);
//            Point pointB = new Point(Width - offsetX - (Width / 6), Margin);
//            Point pointC = new Point(Width - offsetX - (Width / 6), Height - Margin);
//            gfx.DrawTriangle(_brushes["yellow"], new Triangle(pointA, pointB, pointC), 5);
//        }
//    }
//}
