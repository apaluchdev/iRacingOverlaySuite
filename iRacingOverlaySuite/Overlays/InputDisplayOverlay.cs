using GameOverlay.Drawing;
using System.Collections.Generic;
using System;
using System.Windows.Media.Media3D;

namespace iRacingOverlaySuite.Overlays
{
    internal class InputDisplayOverlay : IOverlayDrawer
    {
        private OverlayCanvas _canvas;

        const int BAR_WIDTH = 19;

        public InputDisplayOverlay(int x, int y)
        {
            var _overlayParams = new OverlayParams(225, 0, 200, 400, Location.Center);

            _canvas = new OverlayCanvas(_overlayParams, this);

            _canvas.DrawGrid = true;
            _canvas.Run();
        }

        public void DrawOverlay()
        {
            var brake = IRData.iRacingData?.Brake ?? 0.20f;
            var throttle = IRData.iRacingData?.Throttle ?? 0;

            var brush = _canvas.Brushes;
            var bar = new Rectangle(0 + (_canvas.Width / 2.3f), _canvas.Height * 0.25f, _canvas.Width - (_canvas.Width / 2.3f), _canvas.Height * 0.75f + _canvas.Height * 0.20f);

            // When steering exceeds 5 degrees (0.08 rad), use orange to indicate trail braking should be done.
            var brakeColor = Math.Abs(IRData.iRacingData?.SteeringWheelAngle ?? 0f) < Math.Abs(0.08) ? brush["red"] : brush["orange"];

            _canvas.AddDrawActions(
                new List<Action<Graphics>>()
                {
                    (gfx) =>
                    {
                        gfx.DrawHorizontalProgressBar(brush["black"], brakeColor, bar, 2,(int) (brake*100));
                        DrawPercentageText(gfx, bar);
                        //DrawSteeringInput(gfx, (int) (_canvas.Width / 2.2f), _canvas.Height * 0.20f);
                    }
                }        
            );
        }

        private void DrawPercentageText(Graphics gfx, Rectangle bar)
        {
            var brush = _canvas.Brushes;
            var fonts = _canvas.Fonts;

            const int segments = 5;

            var y = bar.Bottom;
            var pctValue = 0;
            var fontYOffset = 6;
            var fontXOffset = 5;

            for (int i = 0; i < segments; i++)
            {
                if (i != 0) // Skip drawing the first line
                    gfx.DrawLine(brush["white"], new Line(bar.Left, y, bar.Right, y), 1);
                
                gfx.DrawText(fonts["consolas"], 11, brush["red"], bar.Right + fontXOffset, y - fontYOffset, $"{pctValue}%");
                
                y -= bar.Height / 5f;
                pctValue += 100 / segments;
            }
        }

        private void DrawSteeringInput(Graphics gfx, int x, float y)
        {
            var brush = _canvas.Brushes;

            double radius = 50;

            // Calculate the x and y coordinates
            var xPoint = (float) (radius * Math.Cos(-IRData.iRacingData?.SteeringWheelAngle - (Math.PI / 2) ?? -(Math.PI / 2)));
            var yPoint = (float) (radius * Math.Sin(-IRData.iRacingData?.SteeringWheelAngle - (Math.PI / 2) ?? -(Math.PI / 2)));

            gfx.DrawLine(brush["gray"], x + 0 + (BAR_WIDTH / 2), y + 0, x + xPoint + (BAR_WIDTH / 2), y + yPoint, 4);
        }
    }
}
