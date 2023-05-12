using System;
using System.Collections.Generic;
using GameOverlay.Drawing;

namespace iRacingOverlaySuite.Overlays
{
    internal class ABSOverlay : Overlay, IDisposable
    {
        DateTime BrakeBegin;
        bool BrakeWarningEligible = false;
        bool BrakeBeginReset = false;
        bool ShowBrakeWarning;
        DateTime TimeOfWarning;

        public ABSOverlay(int x, int y, Location location, int width, int height) : base(x, y, location, width, height)
        {
            this.SetupCompleted += ABSOverlay_SetupCompleted;
        }

        private void ABSOverlay_SetupCompleted(object? sender, EventArgs e)
        {
            CreateABSOverlay();
        }

        public void CreateABSOverlay()
        {
            AddDrawActions(new List<Action<Graphics>>()
            {
                (gfx) => gfx.ClearScene(_brushes["background"]),              
                //(gfx) => gfx.DrawCircle(_brushes["transparentBlack"], 0, 0, 1, 5000),

                DrawABS((Width / 2) - 70, 0),
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

        public override void RefreshData()
        {
            if (!BrakeWarningEligible)
                BrakeWarningEligible = (IRData.iRacingData?.Brake ?? 0) > 0.75;
        }
    }
}
