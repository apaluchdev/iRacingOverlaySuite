﻿using GameOverlay.Drawing;
using GameOverlay.Windows;
using irsdkSharp.Serialization.Enums.Fastest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;
using YamlDotNet.RepresentationModel;

namespace iRacingOverlaySuite.Overlays
{
    public class InputOverlay : Overlay, IDisposable
    {
        int X;
        int Y;
        int Height;
        int Width;

        public Func<float> GetBrake;
        public Func<float> GetThrottle;
        public Func<double> GetSessionTime;
        public Func<CarLeftRight> GetCarLeftRight;
        public Dictionary<double, float> TimeBrakeValuePairs = new Dictionary<double, float>();

        int Margin = 2;

        public InputOverlay(int x = 0, int y = 0, int width = 600, int height = 200) : base(x, y, width, height)
        {
            X = x;
            Y = y;
            Height = height;
            Width = width;

            GetBrake = () => IRData.Brake;
            GetThrottle = () => IRData.Throttle;
            GetSessionTime = () => Math.Round(IRData.SessionTime,4);

            this.SetupCompleted += InputOverlay_SetupCompleted;
        }

        private void InputOverlay_SetupCompleted(object? sender, EventArgs e)
        {
            CreateInputOverlay();
        }

        public void CreateInputOverlay()
        {
            AddDrawAction((gfx) => gfx.ClearScene(_brushes["background"]));
            AddDrawAction(DrawPercentageBar(Width - Margin, Height - Margin, -20, Height, _brushes["green"], GetThrottle));
            AddDrawAction(DrawPercentageBar(X /* X not needed, remove TODO */ + Margin, Height - Margin, 20, Height, _brushes["red"], GetBrake));
            //AddDrawAction(DrawInputGraph(Width/2 - 150, Height, GetSessionTime, GetBrake));
        }

        private Action<Graphics> DrawInputGraph(int x, int y, Func<double> getSessionTime, Func<float> getBrakePercentage)
        {
            double sessionTime;

            Action<Graphics> drawAction = (gfx) => {

                sessionTime = getSessionTime();
                
                if (sessionTime == 0) return;

                TimeBrakeValuePairs.Add(getSessionTime(), getBrakePercentage());

                TimeBrakeValuePairs = TimeBrakeValuePairs.Where(x => sessionTime - x.Key < 3).ToDictionary(x => x.Key, x => x.Value);

                // Say graph has X width of 300, our time values are between 0 - 3 seconds old
                // An 'older' time value of 2.5 is 83% of 3. So display this time value would have a x value 83% of 300 
                var res = TimeBrakeValuePairs.Select(pair => new KeyValuePair<double, float>((int) ((sessionTime - pair.Key) / 3 * 300), pair.Value));

                foreach (var item in res)
                {
                    Circle c = new Circle(300 - (float) item.Key+x, -y*item.Value+y, 1);
                    gfx.DrawCircle(_brushes["red"], c, 2);
                }
            };

            return drawAction;
        }

        public Action<Graphics> DrawPercentageBar(int x, int y, int width, int height, IBrush color, Func<float> getPercentage)
        {
            Action<Graphics> drawAction = (gfx) => {
                // Dimensions of inner container that changes height depending on the percentage
                Rectangle filling = Rectangle.Create(/* Top Left X */ x, /* Top Left Y */ y, width, -((height - Margin) * getPercentage()));
                // Container dimensions
                Rectangle container = Rectangle.Create(/* Top Left X */ x, /* Top Left Y */ y, width, -(height - Margin));

                if (getPercentage() > 0)
                {
                    gfx.DrawBox2D(_brushes["black"], color, filling, 2);
                }

                gfx.DrawRectangle(_brushes["black"], container, 2);
            };

            return drawAction;
        }
    }
}
