using GameOverlay.Drawing;
using System.Collections.Generic;
using System;

namespace iRacingOverlaySuite.Overlays
{
    internal class InputDisplayOverlay : iRacingOverlay, IOverlayDrawer
    {
        public Rectangle BrakeBar;

        const int BRAKE_SEGMENTS = 5;

        public InputDisplayOverlay(int width, int height, Location location = Location.Center, int x = 0, int y = 0) : base(x, y, width, height, location)
        {
        }

        /// <summary>
        /// Setup the action that will draw the brake bar onto the canvas
        /// </summary>
        public override void SetupOverlay()
        {       
            BrakeBar = new Rectangle(0 + (_canvas.Width / 2.3f), _canvas.Height * 0.25f, _canvas.Width - (_canvas.Width / 2.3f), _canvas.Height * 0.75f + _canvas.Height * 0.20f);

            Action<Graphics> brakeOverlayAction = new Action<Graphics>((gfx) =>
            {   
                gfx.DrawHorizontalProgressBar(brushes["black"], brushes["green"], BrakeBar, 2, GetThrottle());
                gfx.DrawHorizontalProgressBar(brushes["black"], GetBrakeColor(), BrakeBar, 2, GetBrake());

                DrawPercentageText(gfx);
            });

            _canvas.AddDrawAction(brakeOverlayAction);
        }

        /// <summary>
        /// Draws the divider lines that split up the brake bar
        /// </summary>
        /// <param name="gfx"></param>
        private void DrawPercentageText(Graphics gfx)
        {
            var y = BrakeBar.Bottom;
            var pctValue = 0;
            var fontYOffset = 6;
            var fontXOffset = 5;

            for (int i = 0; i < BRAKE_SEGMENTS; i++)
            {
                if (i != 0) // Skip drawing the first line
                    gfx.DrawLine(brushes["white"], new Line(BrakeBar.Left, y, BrakeBar.Right, y), 1);
                
                gfx.DrawText(fonts["calibri"], 11, brushes["red"], BrakeBar.Right + fontXOffset, y - fontYOffset, $"{pctValue}%");
                
                y -= BrakeBar.Height / BRAKE_SEGMENTS;
                pctValue += 100 / BRAKE_SEGMENTS;
            }
        }

        private float GetBrake()
        {
            return IRData.iRacingData?.Brake ?? (float)Math.Abs(Math.Cos(DateTime.Now.Second))*100;
        }

        private float GetThrottle()
        {
            return IRData.iRacingData?.Throttle ?? (float)Math.Abs(Math.Sin(DateTime.Now.Second))*100;
        }

        private SolidBrush GetBrakeColor()
        {
            return Math.Abs(IRData.iRacingData?.SteeringWheelAngle ?? 0f) < Math.Abs(0.08) ? brushes["red"] : brushes["orange"];
        }
    }
}
