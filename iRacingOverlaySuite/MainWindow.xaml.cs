using Examples;
using iRacingOverlaySuite.Overlays;
using irsdkSharp;
using irsdkSharp.Serialization;
using irsdkSharp.Serialization.Models.Session;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace iRacingOverlaySuite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Begin();
        }

        static void Begin()
        {
            //RunInputGraphOverlay();
            RunInputOverlay();
            RunProximityOverlay();
            //RunBrakingMarkerOverlay();

            Console.ReadLine();
        }

        private static void RunInputOverlay()
        {
            InputOverlay inputOverlay = new InputOverlay();
            Task.Run(() => inputOverlay.Run());
        }

        private static void RunProximityOverlay()
        {
            ProximityOverlay proximityOverlay = new ProximityOverlay();
            Task.Run(() => proximityOverlay.Run());
        }

        private static void RunInputGraphOverlay()
        {
            InputGraphOverlay inputGraphOverlay = new InputGraphOverlay();
            Task.Run(() => inputGraphOverlay.Run());
        }

        private static void RunBrakingMarkerOverlay()
        {
            BrakingMarkerOverlay brakingMarkerOverlay = new BrakingMarkerOverlay();
            Task.Run(() => brakingMarkerOverlay.Run());
        }
    }
}
