using System;
using System.Collections.Generic;
using System.Windows;
using GameOverlay.Drawing;

namespace iRacingOverlaySuite.Overlays
{
    internal class DataDisplayOverlay : Overlay, IDisposable
    {

        Func<float>? GetBrake;
        Func<float>? GetThrottle;
        Func<float>? GetTrackTemperature;
        Func<float>? GetAirTemperature;

        const int BAR_WIDTH = 25;

        public DataDisplayOverlay(int x, int y, Location location, int width, int height) : base(x, y, location, width, height)
        {
            this.SetupCompleted += DataDisplayOverlay_SetupCompleted;
        }

        private void DataDisplayOverlay_SetupCompleted(object? sender, EventArgs e)
        {
            CreateDataDisplayOverlay();
        }

        public void CreateDataDisplayOverlay()
        {
            GetBrake = () => IRData.iRacingData?.Brake ?? 0;
            GetThrottle = () => IRData.iRacingData?.Throttle ?? 0;
            GetTrackTemperature = () => IRData.iRacingData?.TrackTemp ?? 999;
            GetAirTemperature = () => IRData.iRacingData?.AirTemp ?? 999;

            AddDrawActions(new List<Action<Graphics>>()
            {
                (gfx) => gfx.ClearScene(_brushes["background"]),
                
                //(gfx) => gfx.DrawCircle(_brushes["transparentBlack"], 0, 0, 1, 5000),

                DrawDataDisplay(0, 0)
            });
        }

        public Action<Graphics> DrawDataDisplay(int x, int y)
        {
            Action<Graphics> drawAction = (gfx) =>
            {
                y = -25;
                gfx.DrawTextWithBackground(_fonts["consolas"], 16, _brushes["white"], _brushes["transparentBlack"], 0, y+=25, $"TT: {IRData.iRacingData?.TrackTemp ?? 999}°");
                gfx.DrawTextWithBackground(_fonts["consolas"], 16, _brushes["white"], _brushes["transparentBlack"], 0, y += 25, $"AT: {IRData.iRacingData?.AirTemp ?? 999}°");
                gfx.DrawTextWithBackground(_fonts["consolas"], 16, _brushes["white"], _brushes["transparentBlack"], 0, y += 25, $"BB: {IRData.iRacingData?.dcBrakeBias ?? 0}%");
                gfx.DrawTextWithBackground(_fonts["consolas"], 16, _brushes["white"], _brushes["transparentBlack"], 0, y += 25, $"LR_Rumble: {IRData.iRacingData?.TireLR_RumblePitch ?? 0}");
                gfx.DrawTextWithBackground(_fonts["consolas"], 16, _brushes["white"], _brushes["transparentBlack"], 0, y += 25, $"Yaw: {IRData.iRacingData?.YawRate ?? 0}");
                gfx.DrawTextWithBackground(_fonts["consolas"], 16, _brushes["white"], _brushes["transparentBlack"], 0, y += 25, $"LF CM Tyre: {IRData.iRacingData?.LFtempCM ?? 0}°");
                gfx.DrawTextWithBackground(_fonts["consolas"], 16, _brushes["white"], _brushes["transparentBlack"], 0, y += 25, $"SteeringAngle: {IRData.iRacingData?.SteeringWheelAngle ?? 0}°");        
            };
            return drawAction;
        }



        public override void RefreshData()
        {
        }
    }
}
