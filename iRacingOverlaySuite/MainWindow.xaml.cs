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

        private static IRacingSDK sdk;
        private static IRacingSessionModel _session;
        private static int _driverId;

        /// <summary>
        /// Gets the Id (CarIdx) of yourself (the driver running this application).
        /// </summary>
        public static int DriverId { get { return _driverId; } }


        static void Begin()
        {
            sdk = new IRacingSDK();
            sdk.OnConnected += Sdk_OnConnected;

            RunInputOverlay();

            Console.ReadLine();
        }

        private static void Sdk_OnConnected()
        {

        }

        private static void RunInputOverlay()
        {
            InputOverlay inputOverlay = new InputOverlay();
            Task.Run(() => inputOverlay.Run());
        }

        private static void RunExampleOverlay()
        {
            Example example = new Example();
            Task.Run(() => example.Run());
        }


        private static void Loop()
        {
            int lastUpdate = -1;
            while (true)
            {
                var currentlyConnected = sdk.IsConnected();

                // Check if we can find the sim
                if (currentlyConnected)
                {
                    var testData = sdk.GetData();

                    // Parse out your own driver Id
                    if (DriverId == -1)
                    {
                        var headers = sdk.Header;
                        _driverId = (int)sdk.GetData("PlayerCarIdx");
                    }

                    // Is the session info updated?
                    int newUpdate = sdk.Header.SessionInfoUpdate;
                    if (newUpdate != lastUpdate)
                    {
                        lastUpdate = newUpdate;
                        _session = sdk.GetSerializedSessionInfo();
                    }

                    Thread.Sleep(15);
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
