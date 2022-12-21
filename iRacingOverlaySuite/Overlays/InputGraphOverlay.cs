using GameOverlay.Drawing;
using irsdkSharp.Serialization.Enums.Fastest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace iRacingOverlaySuite.Overlays
{
    internal class InputGraphOverlay : Overlay, IDisposable
    {
        int Height;
        int Width;

        const int SECONDS_TO_GRAPH = 3;

        public Func<float> GetBrake;
        public Func<float> GetThrottle;
        public Func<double> GetSessionTime;
        public Func<CarLeftRight> GetCarLeftRight;
        public Dictionary<double, float> TimeBrakeValuePairs = new Dictionary<double, float>();
        public Dictionary<double, float> TimeThrottleValuePairs = new Dictionary<double, float>();

        public InputGraphOverlay(int x = 0, int y = 0, int width = 280, int height = 100) : base(x, y, width, height)
        {
            Height = height;
            Width = width;

            GetBrake = () => IRData.Brake;
            GetThrottle = () => IRData.Throttle;
            GetSessionTime = () => Math.Round(IRData.SessionTime, 4);

            this.SetupCompleted += InputGraphOverlay_SetupCompleted; ;
        }

        private void InputGraphOverlay_SetupCompleted(object? sender, EventArgs e)
        {
            CreateInputGraphOverlay();
        }

        public void CreateInputGraphOverlay()
        {
            AddDrawAction((gfx) => gfx.ClearScene(_brushes["transparent"]));
            AddDrawAction(DrawGraphBackground());
            AddDrawAction(DrawInputGraph(GetSessionTime, GetBrake, GetThrottle));
        }

        private Action<Graphics> DrawInputGraph(Func<double> getSessionTime, Func<float> getBrakePercentage, Func<float> getThrottlePercentage)
        {
            double sessionTime;

            Action<Graphics> drawAction = (gfx) => {

                sessionTime = getSessionTime();

                if (sessionTime == 0) return;

                // TODO - Optimize by using a better data structure for quickly adding and removing many values.
                TimeBrakeValuePairs.Add(sessionTime, getBrakePercentage());
                TimeThrottleValuePairs.Add(sessionTime, getThrottlePercentage());

                TimeBrakeValuePairs = TimeBrakeValuePairs.Where(x => sessionTime - x.Key < SECONDS_TO_GRAPH).ToDictionary(x => x.Key, x => x.Value);
                TimeThrottleValuePairs = TimeThrottleValuePairs.Where(x => sessionTime - x.Key < SECONDS_TO_GRAPH).ToDictionary(x => x.Key, x => x.Value);


                // Say graph has X width of 300, our time values are between 0 - 3 seconds old
                // An 'older' time value of 2.5 is 83% of 3. So display this time value would have a x value 83% of 300 
                var res = TimeBrakeValuePairs.Select(pair => new KeyValuePair<double, float>((int)((sessionTime - pair.Key) / SECONDS_TO_GRAPH * Width), pair.Value)).ToList();
                var resThrottle = TimeThrottleValuePairs.Select(pair => new KeyValuePair<double, float>((int)((sessionTime - pair.Key) / SECONDS_TO_GRAPH * Width), pair.Value)).ToList();

                for (int i = 0; i < res.Count() - 1; i++)
                {
                    Line line = new Line(Width - (float)res[i].Key, Height - (res[i].Value * Height), Width - (float)res[i+1].Key, Height - (res[i+1].Value * Height));
                    gfx.DrawLine(_brushes["red"], line, 2);
                }

                for (int i = 0; i < resThrottle.Count() - 1; i++)
                {
                    Line line = new Line(Width - (float)resThrottle[i].Key, Height - (resThrottle[i].Value * Height), Width - (float)resThrottle[i + 1].Key, Height - (resThrottle[i + 1].Value * Height));
                    gfx.DrawLine(_brushes["green"], line, 2);
                }
            };

            return drawAction;
        }

        private Action<Graphics> DrawGraphBackground()
        {
            List<Line> bgLines = new List<Line>();
            var heightSteps = 4;
            // Split graph into quarters
            for (int i = 0; i < heightSteps; i++)
            { 
                bgLines.Add(new Line(0, (Height/heightSteps)*i, Width, (Height / heightSteps) * i));
            }

            Action<Graphics> drawAction = (gfx) => {
                
                foreach (var line in bgLines)
                {
                    gfx.DrawLine(_brushes["transparentWhite"], line, 1);
                    gfx.DrawText(_fonts["consolas"], 8, _brushes["transparentWhite"], Width - 20, line.End.Y, $"{Height - line.End.Y}%");
                }
            };

            return drawAction;
        }
    }
}
