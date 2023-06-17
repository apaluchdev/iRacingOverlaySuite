using Examples;
using iRacingOverlaySuite.Overlays;

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
            Task.Run(() => new InputDisplayOverlay(150, 500, Location.Center, 255));
            Task.Run(() => new InfoDisplayOverlay(300, 400));
            Task.Run(() => new ProximityOverlay(600, 100));
            Task.Run(() => new OpponentInfoOverlay(400, 100));

            Console.ReadLine();
        }
    }
}
