using GameOverlay.Drawing;
using System.Collections.Generic;
using System;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Text;
using System.Windows.Navigation;

namespace iRacingOverlaySuite.Overlays
{
    internal class ToastOverlay : IOverlayDrawer
    {
        private OverlayCanvas _canvas;

        private TimeSpan _leaderLastTime = TimeSpan.FromSeconds(0);
        private int _leaderCarNumber = 0;

        public ToastOverlay(int x, int y)
        {
            var _overlayParams = new OverlayParams(0, 0, 400, 100, Location.TopMiddle);

            _canvas = new OverlayCanvas(_overlayParams, this);

            _canvas.DrawGrid = true;
            _canvas.Run();
        }

        public void DrawOverlay()
        {
            var leaderLastTime = GetLeaderLastLapTime();

            _leaderLastTime = leaderLastTime.Item1;
            _leaderCarNumber = leaderLastTime.Item2;

            var displayString = new StringBuilder();
            displayString.Append($"Leader Last: {FormatTime(_leaderLastTime)}");

            _canvas.AddDrawActions(
                new List<Action<Graphics>>()
                {
                    (gfx) =>
                    {
                        gfx.DrawTextWithBackground(_canvas.Fonts["courier"], 22, _canvas.Brushes["white"], _canvas.Brushes["black"], 0, 0, displayString.ToString());
                        
                        if (GetClosestCarBehindDistance() < 50)
                        {
                            gfx.DrawTextWithBackground(_canvas.Fonts["courier"], 22, _canvas.Brushes["red"], _canvas.Brushes["black"], 0, 25, $"Behind: {GetClosestCarBehindDistance()}m");
                        }
                        else if (GetClosestCarBehindDistance() < 100)
                        {
                            gfx.DrawTextWithBackground(_canvas.Fonts["courier"], 22, _canvas.Brushes["orange"], _canvas.Brushes["black"], 0, 25, $"Behind: {GetClosestCarBehindDistance()}m");
                        }
                        else if (GetClosestCarBehindDistance() < 500)
                        {
                            gfx.DrawTextWithBackground(_canvas.Fonts["courier"], 22, _canvas.Brushes["yellow"], _canvas.Brushes["black"], 0, 25, $"Behind: {GetClosestCarBehindDistance()}m");
                        }
                        else
                        {
                            gfx.DrawTextWithBackground(_canvas.Fonts["courier"], 22, _canvas.Brushes["white"], _canvas.Brushes["black"], 0, 25, $"Behind: {GetClosestCarBehindDistance()}m");
                        }
                    }
                }
            );
        }

        private Tuple<TimeSpan, int> GetLeaderLastLapTime()
        {
            if (IRData.iRacingData?.Cars != null)
            {
                var myClass = IRData.iRacingData.Cars[0].CarIdxClass;
                foreach (var car in IRData.iRacingData.Cars)
                {
                    if (car.CarIdxClassPosition == 1 && car.CarIdxClass == myClass)
                    {
                        return new Tuple<TimeSpan, int>(TimeSpan.FromSeconds(car.CarIdxLastLapTime), car.CarIdx);
                    }
                }
            }

            return new Tuple<TimeSpan, int>(TimeSpan.Zero, 0);
        }

        private float GetClosestCarBehindDistance()
        {
            float closestCar = 999;
            if (IRData.iRacingData?.Cars != null)
            {
                var myClass = IRData.iRacingData.Cars[0].CarIdxClass;
                foreach (var car in IRData.iRacingData.Cars)
                {
                    var opponentDistance = car.CarIdxLapDistPct * float.Parse(IRData.Session?.WeekendInfo.TrackLength ?? "0");
                    var distanceToOpponent = IRData.iRacingData.LapDist - opponentDistance;
                    if (distanceToOpponent > 0 && distanceToOpponent < closestCar)
                    {
                        closestCar = distanceToOpponent;
                    }
                }
            }

            return closestCar;
        }

        public string FormatTime(TimeSpan raceTime)
        {
            string formattedTime = string.Format("{0}:{1:D2}.{2:D1}{3:D2}",
            (int)raceTime.TotalMinutes,      // Minutes
            raceTime.Seconds,                 // Seconds
            (int)(raceTime.Milliseconds / 100),   // Tenths
            (int)(raceTime.Milliseconds % 100));  // Hundredths

            return formattedTime;
        }
    }
}
