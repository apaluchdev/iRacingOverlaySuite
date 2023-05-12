using GameOverlay.Drawing;
using System;
using System.Collections.Generic;

namespace iRacingOverlaySuite.Overlays
{
    public class InputOverlay : Overlay, IDisposable
    {
        Func<float>? GetBrake;
        Func<float>? GetThrottle;

        const int BAR_WIDTH = 25;

        public InputOverlay(int x, int y, Location location, int width, int height) : base(x, y, location, width, height)
        {
            this.SetupCompleted += InputOverlay_SetupCompleted;
        }

        private void InputOverlay_SetupCompleted(object? sender, EventArgs e)
        {
            CreateInputOverlay();
        }

        public void CreateInputOverlay()
        {
            const int GAP_SIZE = 400;

            GetBrake = () => IRData.iRacingData?.Brake ?? 0;
            GetThrottle = () => IRData.iRacingData?.Throttle ?? 0;

            AddDrawActions(new List<Action<Graphics>>()
            {
                (gfx) => gfx.ClearScene(_brushes["background"]),
                //(gfx) => gfx.DrawCircle(_brushes["transparentBlack"], 0, 0, 1, 5000),

                // Brake
                DrawPercentageBar(Width / 2 - (BAR_WIDTH / 2), 0, _brushes["transparentRed"], GetBrake),
                DrawPercentageText(Width / 2 - (BAR_WIDTH / 2), 0),

                //DrawPercentageBar(Width / 2 - GAP_SIZE, 0, _brushes["transparentRed"], GetBrake),
                //DrawPercentageBar(Width / 2 + GAP_SIZE - BAR_WIDTH, 0, _brushes["transparentGreen"], GetThrottle),

                //DrawPercentageText(Width / 2 - GAP_SIZE, 0),
                //DrawPercentageText(Width / 2 + GAP_SIZE - BAR_WIDTH, 0),

                //DrawABS((Width / 2) - 70, 0),
            });
        }

        public Action<Graphics> DrawABS(int x, int y)
        {
            Action<Graphics> drawAction = (gfx) =>
            {
                if (IRData.iRacingData?.BrakeABSactive ?? false)
                {
                    gfx.DrawImage(ABSOn, x, 0, 0.5f);
                }
            };

            return drawAction;
        }

        public Action<Graphics> DrawPercentageText(int x, int y)
        {
            Action<Graphics> drawAction = (gfx) =>
            {
                gfx.DrawLine(_brushes["transparentWhite"], new Line(x, y + (Height / 4), x + BAR_WIDTH, y + (Height / 4)), 1);
                gfx.DrawText(_fonts["consolas"], 11, _brushes["white"], x + 30, y + (Height / 4), "75%");

                gfx.DrawLine(_brushes["transparentWhite"], new Line(x, y + (Height / 4) * 2, x + BAR_WIDTH, y + (Height / 4) * 2), 1);
                gfx.DrawText(_fonts["consolas"], 11, _brushes["white"], x + 30, y + (Height / 4)*2, "50%");

                gfx.DrawLine(_brushes["transparentWhite"], new Line(x, y + (Height / 4) * 3, x + BAR_WIDTH, y + (Height / 4) * 3), 1);
                gfx.DrawText(_fonts["consolas"], 11, _brushes["white"], x + 30, y + (Height / 4)*3, "25%");
            };

            return drawAction;
        }

        public Action<Graphics> DrawPercentageBar(int x, int y, IBrush color, Func<float> GetValue)
        {
            Action<Graphics> drawAction = (gfx) => 
            {
                Rectangle filling = Rectangle.Create(x, y + (Height * (1f - GetValue())), BAR_WIDTH, Height);
                Rectangle container = Rectangle.Create(x - 1, y + 1, BAR_WIDTH + 2, Height - 2);

                gfx.DrawBox2D(_brushes["transparentBlack"], color, filling, 0);

                if (IRData.iRacingData?.Brake > 0.8f)
                    gfx.DrawBox2D(_brushes["purple"], _brushes["transparent"], container, 4);
                else if (IRData.iRacingData?.Brake > 0.65f)
                    gfx.DrawBox2D(_brushes["violet"], _brushes["transparent"], container, 4);
                else if (IRData.iRacingData?.Brake > 0.4f)
                    gfx.DrawBox2D(_brushes["indigo"], _brushes["transparent"], container, 4);
                else if (IRData.iRacingData?.Brake > 0.2f)
                    gfx.DrawBox2D(_brushes["softBlue"], _brushes["transparent"], container, 4);
                else if (IRData.iRacingData?.Brake > 0.05f)
                    gfx.DrawBox2D(_brushes["softGreen"], _brushes["transparent"], container, 4);
                else
                    gfx.DrawBox2D(_brushes["transparentBlack"], _brushes["transparent"], container, 2);
            };

            return drawAction;
        }

        public override void RefreshData()
        {
        }
    }
}
