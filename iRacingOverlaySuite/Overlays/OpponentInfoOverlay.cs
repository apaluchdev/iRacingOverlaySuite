using GameOverlay.Drawing;
using System.Collections.Generic;
using System;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Text;
using System.Windows.Navigation;
using System.Windows.Controls;

namespace iRacingOverlaySuite.Overlays
{
    internal class OpponentInfoOverlay : iRacingOverlay, IOverlayDrawer
    {
        private TimeSpan _leaderLastTime = TimeSpan.FromSeconds(0);

        public OpponentInfoOverlay(int width, int height, Location location = Location.TopMiddle, int x = 0, int y = 0) : base(x, y, width, height, location)
        {
        }

        public override void SetupOverlay()
        {
            Action<Graphics> opponentInfoOverlayAction = new Action<Graphics>((gfx) =>
            {
                gfx.DrawTextWithBackground(_canvas.Fonts["calibri"], 22, _canvas.Brushes["white"], _canvas.Brushes["black"], 0, 0, GetOpponentInfo());
                //gfx.DrawTextWithBackground(_canvas.Fonts["calibri"], 22, _canvas.Brushes["red"], _canvas.Brushes["black"], 0, 25, $"{GetClosestCarBehindDistance().ToString()}m");
                gfx.DrawTextWithBackground(_canvas.Fonts["calibri"], 22, GetDeltaColor(GetDeltaTime()), _canvas.Brushes["black"], 0, 50, $"Delta: {GetDeltaTime()}");

            });

            _canvas.AddDrawAction(opponentInfoOverlayAction);
        }

        private float GetDeltaTime()
        {
            return IRData.iRacingData?.LapDeltaToSessionBestLap ?? 0;
        }

        private SolidBrush GetDeltaColor(float delta)
        {
            if (delta < 0)
            {
                return brushes["red"];
            }
            else
            {
                return brushes["green"];
            }
        }

        private string GetOpponentInfo()
        {
            var leaderLastTime = GetLeaderLastLapTime();

            _leaderLastTime = leaderLastTime;

            var opponentInfo = new StringBuilder();
            opponentInfo.Append($"Leader Time: {FormatTime(_leaderLastTime)}");

            return opponentInfo.ToString();
        }

        private TimeSpan GetLeaderLastLapTime()
        {
            if (IRData.iRacingData?.Cars != null)
            {
                var myClass = IRData.iRacingData.Cars[0].CarIdxClass;
                foreach (var car in IRData.iRacingData.Cars)
                {
                    if (car.CarIdxClassPosition == 1 /* Not sure if iRacing counts positions from 0 or 1... */ && car.CarIdxClass == myClass)
                    {
                        return TimeSpan.FromSeconds(car.CarIdxLastLapTime);
                    }
                }
            }

            return TimeSpan.Zero;
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

            if (!IRData.IsConnected)
            {
                return (float)Math.Abs(Math.Sin(DateTime.Now.Second / 30f) * 1000);
            }
            return closestCar;
        }

        /// <summary>
        /// Returns a TimeSpan in a format suitable for racing measurements
        /// </summary>
        /// <param name="raceTime"></param>
        /// <returns></returns>
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
