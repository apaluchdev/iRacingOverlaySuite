using GameOverlay.Drawing;
using irsdkSharp.Serialization.Enums.Fastest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iRacingOverlaySuite.Overlays
{
    public class BrakingMarkerOverlay : Overlay, IDisposable
    {
        int X;
        int Y;
        private int Height;
        private int Width;

        int Margin = 5;

        private Dictionary<int, List<float>> _brakeMarkers = new Dictionary<int, List<float>>();

        public DateTime _lastBrakeMarkerAddition;
        public float _lastBrakeValue { get; private set; }
        private int _currentLap { get; set; }
        private float _currentDist { get; set; }

        public Func<float> GetBrake;

        public BrakingMarkerOverlay(int x = 0, int y = 200, int width = 300, int height = 300) : base(x, y, width, height)
        {
            X = x;
            Y = y;
            Height = height;
            Width = width;

            GetBrake = () => IRData.Brake;

            this.SetupCompleted += ProximityOverlay_SetupCompleted;
        }

        private void ProximityOverlay_SetupCompleted(object? sender, EventArgs e)
        {
            CreateProximityOverlay();
        }

        public void CreateProximityOverlay()
        {
            AddDrawAction((gfx) => gfx.ClearScene(_brushes["background"]));
            //AddDrawAction(DrawCircle(X, Y, GetBrake));
            AddDrawAction(DrawUpcomingBrakeMarker(X, Y));
            //AddDrawAction(DrawDebug(X, Y));
        }

        public Action<Graphics> DrawCircle(int x, int y, Func<float> getBrakePercentage)
        {
            Action<Graphics> drawAction = (gfx) => {

                gfx.DrawCircle(_brushes["red"], 0, 0, 100, 500);
            };

            return drawAction;
        }

        public Action<Graphics> DrawDebug(int x, int y)
        {
            var debugStrings = new List<string>();

            Action<Graphics> drawAction = (gfx) => {
                debugStrings.Add($"Current Lap: {_currentLap}");
                debugStrings.Add($"Set Brake Markers: {(_brakeMarkers.ContainsKey(_currentLap+1) ? _brakeMarkers[_currentLap+1].Count : -1)}");
                debugStrings.Add($"Upcoming Brake Markers: {(_brakeMarkers.ContainsKey(_currentLap) ? _brakeMarkers[_currentLap].Count : -1)}");
                debugStrings.Add($"Next Marker: {_brakeMarkers[_currentLap].Where(x => x > _currentDist).FirstOrDefault() - _currentDist}");
                int ySpacing = -20;
                foreach (var str in debugStrings)
                {
                    gfx.DrawTextWithBackground(_fonts["consolas"], _brushes["white"], _brushes["black"], new Point((Width/2)-50, ySpacing += 20), str);
                }

                debugStrings.Clear();
            };

            return drawAction;
        }

        public Action<Graphics> DrawUpcomingBrakeMarker(int x, int y)
        {
            var timeOfLastWarning = DateTime.Now;
            var showWarning = false;

            Action<Graphics> drawAction = (gfx) =>
            {
                if (_brakeMarkers[_currentLap].Count > 0)
                {
                    // Should be auto sorted already - an exception would be if the car ever went backwards
                    var nextMarker = _brakeMarkers[_currentLap].Where(x => x > _currentDist).FirstOrDefault();
                    if (nextMarker - _currentDist < 300) // Show indicator while 300m or less away
                    {
                        var markerPercentage = ((nextMarker - _currentDist) / 300) * 100;

                        if (markerPercentage < 2)
                        {
                            showWarning = true;
                            timeOfLastWarning = DateTime.Now;
                        }
                            
                        if (DateTime.Now - timeOfLastWarning > TimeSpan.FromSeconds(0.5))
                        {
                            showWarning = false;
                        }

                        gfx.DrawHorizontalProgressBar(_brushes["black"], _brushes["green"], new Rectangle(Width / 2 - 10, 0, Width / 2 + 10, Height), 2, 100);
                        gfx.DrawHorizontalProgressBar(showWarning ? _brushes["blue"] : _brushes["black"], _brushes["red"], new Rectangle(Width / 2 - 10, 0, Width / 2 + 10, Height), showWarning ? 10 : 2, markerPercentage);
                    }
                }
            };

            return drawAction;
        }

        /// <summary>
        /// Runs before the draw actions to prepare any variables that need to be updated
        /// </summary>
        public override void RefreshData()
        {
            _currentLap = IRData.CurrentLap;
            _currentDist = IRData.LapDistance;
            var nextLap = _currentLap + 1;

            if (!_brakeMarkers.ContainsKey(nextLap))
            {
                _brakeMarkers.Add(nextLap, new List<float>());
            }

            if (!_brakeMarkers.ContainsKey(_currentLap))
            {
                _brakeMarkers.Add(_currentLap, new List<float>());
            }

            if (!IRData.InsideCar)
            {
                _brakeMarkers[_currentLap].Clear();
            }

            var currentBrakePercentage = GetBrake();

            if (_lastBrakeValue < 0.1 && currentBrakePercentage > 0.1 && IRData.Speed > 10)
            {
                // Ensure there's a gap of time between adding consecutive brake markers
                if (DateTime.Now - _lastBrakeMarkerAddition > TimeSpan.FromSeconds(3))
                {
                    _brakeMarkers[nextLap].Add(IRData.LapDistance);
                    _lastBrakeMarkerAddition = DateTime.Now;
                }
            }

            _lastBrakeValue = currentBrakePercentage;
        }
    }
}
