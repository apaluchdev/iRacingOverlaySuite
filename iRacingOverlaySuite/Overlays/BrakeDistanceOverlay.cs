using System;
using System.Collections.Generic;
using System.Linq;
using GameOverlay.Drawing;

namespace iRacingOverlaySuite.Overlays
{
    public class BrakeDistanceOverlay : Overlay, IDisposable
    {
        public Dictionary<int, List<float>> BrakeMarkers { get; private set; }

        private int _currentLap;
        private float _currentDist;
        private DateTime _lastBrakeMarkerAddition;
        private float _lastBrakeValue;
        private float _distanceAtLastBrake;
        private DateTime _distAtLastBrakeTime;

        public BrakeDistanceOverlay(int x, int y, Location location, int width, int height) : base(x, y, location, width, height)
        {
            this.SetupCompleted += BrakeDistanceOverlay_SetupCompleted;
            BrakeMarkers = new Dictionary<int, List<float>>();
        }

        private void BrakeDistanceOverlay_SetupCompleted(object? sender, EventArgs e)
        {
            CreateBrakeDistanceOverlay();
        }

        public void CreateBrakeDistanceOverlay()
        {
            AddDrawActions(new List<Action<Graphics>>()
            {
                (gfx) => gfx.ClearScene(_brushes["background"]),              
                //(gfx) => gfx.DrawCircle(_brushes["transparentBlack"], 0, 0, 1, 5000),

                DrawNextMarker((Width / 2) - 90, 0),
                //DrawDataDisplay(0, 50)
            });
        }

        public Action<Graphics> DrawDataDisplay(int x, int y)
        {
            var originalY = y;
            Action<Graphics> drawAction = (gfx) =>
            {
                y = originalY-25;
                var markersSet = BrakeMarkers.ContainsKey(_currentLap) ? BrakeMarkers[_currentLap + 1].Count() : -1;
                var markersThisLap = BrakeMarkers.ContainsKey(_currentLap + 1) ? BrakeMarkers[_currentLap].Count() : -1;
                var nextMarkerDist = BrakeMarkers[_currentLap].FirstOrDefault(m => m <= _currentDist);

                gfx.DrawTextWithBackground(_fonts["consolas"], 16, _brushes["white"], _brushes["transparentBlack"], 0, y += 25, $"Markers Set: {markersSet}");
                gfx.DrawTextWithBackground(_fonts["consolas"], 16, _brushes["white"], _brushes["transparentBlack"], 0, y += 25, $"Markers: {markersThisLap}");
                gfx.DrawTextWithBackground(_fonts["consolas"], 16, _brushes["white"], _brushes["transparentBlack"], 0, y += 25, $"Next: {nextMarkerDist}m");
            };
            return drawAction;
        }

        public Action<Graphics> DrawNextMarker(int x, int y)
        {
            Action<Graphics> drawAction = (gfx) =>
            {
                gfx.DrawText(_fonts["consolas"], 36, _brushes["white"], x - 40, y, $"{(int)_currentDist}m");

                if (DateTime.Now - _distAtLastBrakeTime < TimeSpan.FromSeconds(4))
                {
                    gfx.DrawText(_fonts["consolas"], 48, _brushes["red"], x - 40, y + 75, $"DIST:{(int)_distanceAtLastBrake}m");
                }

                if (!BrakeMarkers.ContainsKey(_currentLap)) return;
                
                var nextMarkerDist = BrakeMarkers[_currentLap].FirstOrDefault(m => m >= _currentDist);

                if (nextMarkerDist <= 0 || _currentDist > nextMarkerDist) return;

                var distDifference = nextMarkerDist - _currentDist;

                if (distDifference < 50)
                    gfx.DrawText(_fonts["consolas"], 36, _brushes["red"], x - 40, y+50, $"{(int) distDifference}m");
                else if (distDifference < 150)
                    gfx.DrawText(_fonts["consolas"], 36, _brushes["orange"], x - 40, y + 50, $"{(int)distDifference}m");
                else
                    gfx.DrawText(_fonts["consolas"], 36, _brushes["yellow"], x - 40, y + 50, $"{(int)distDifference}m");


            };

            return drawAction;
        }

        public override void RefreshData()
        {
            bool clearMarkers = false;

            // Clear the pending added markers if a reset was used
            if (_currentDist > (IRData.iRacingData?.LapDist ?? 0) && _currentLap == IRData.iRacingData?.Lap)
                clearMarkers = true; 

            _currentLap = IRData.iRacingData?.Lap ?? 0;
            _currentDist = (IRData.iRacingData?.LapDist ?? 0);

            if (!BrakeMarkers.ContainsKey(_currentLap + 1))
            {
                BrakeMarkers.Add(_currentLap + 1, new List<float>());
            }
            
            if (clearMarkers)
            {
                BrakeMarkers[_currentLap + 1].Clear();
                clearMarkers = false;
            }
                

            if (!BrakeMarkers.ContainsKey(_currentLap))
            {
                BrakeMarkers.Add(_currentLap, new List<float>());
            }

            if (!IRData.iRacingData?.IsOnTrackCar ?? false)
            {
                BrakeMarkers[_currentLap].Clear();
            }

            AddMarker();
        }

        /// <summary>
        /// Checks the current game state to see if we need to add a brake marker
        /// </summary>
        public void AddMarker()
        {
            var currentBrakePercentage = IRData.iRacingData?.Brake ?? 0;

            if (_lastBrakeValue < 0.01f && currentBrakePercentage >= 0.01)
            {
                _distanceAtLastBrake = IRData.iRacingData?.LapDist ?? 0;
                _distAtLastBrakeTime = DateTime.Now;
            }

            if (currentBrakePercentage > 0.4f && (IRData.iRacingData?.Speed ?? 0) > 10)
            {
                // Ensure there's a gap of time between adding consecutive brake markers
                if (DateTime.Now - _lastBrakeMarkerAddition > TimeSpan.FromSeconds(5))
                {
                    BrakeMarkers[_currentLap + 1].Add(_distanceAtLastBrake);
                    _lastBrakeMarkerAddition = DateTime.Now;
                }
            }

            _lastBrakeValue = currentBrakePercentage;
        }
    }
}
