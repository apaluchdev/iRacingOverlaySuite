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
            Task.Run(() => new InputDisplayOverlay(0, 0));
            Task.Run(() => new InfoDisplayOverlay(0, 0));
            Task.Run(() => new ProximityOverlay(0, 0));

            Console.ReadLine();
        }
    }
}
