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
            RunABSOverlay();
            RunDataDisplayOverlay();
            //RunBrakingMarkerOverlay();

            // IDEA - Use Azure service bus to send session info (laptimes, temps, etc.) to create a more detailed results page.

            Console.ReadLine();
        }

        private static void RunInputOverlay()
        {

            InputOverlay inputOverlay = new InputOverlay(0, 0, Location.Center, 1000, 250); // Define absolute position offsets to the attached window here
            Task.Run(() => inputOverlay.Run());
        }

        private static void RunProximityOverlay()
        {
            ProximityOverlay proximityOverlay = new ProximityOverlay(0, 0, Location.Center, 1400, 200);
            Task.Run(() => proximityOverlay.Run());
        }
        private static void RunDataDisplayOverlay()
        {
            DataDisplayOverlay dataDisplayOverlay = new DataDisplayOverlay(10, 10, Location.TopLeft, 400, 200);
            Task.Run(() => dataDisplayOverlay.Run());
        }

        private static void RunABSOverlay()
        {
            ABSOverlay absOverlay = new ABSOverlay(0, 50, Location.Center, 400, 200);
            Task.Run(() => absOverlay.Run());
        }

        private static void RunInputGraphOverlay()
        {
            //InputGraphOverlay inputGraphOverlay = new InputGraphOverlay();
            //Task.Run(() => inputGraphOverlay.Run());
        }

        private static void RunBrakingMarkerOverlay()
        {
            BrakeDistanceOverlay brakingMarkerOverlay = new BrakeDistanceOverlay(-20, 50, Location.Center, 400, 200);
            Task.Run(() => brakingMarkerOverlay.Run());
        }
    }
}
