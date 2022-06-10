using ShareLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfFrequentlyChangeCollectionPerformanceTest
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        Stopwatch _sw = new Stopwatch();
        long _frameCounter = 0;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowVm();

            CompositionTarget.Rendering += CompositionTarget_Rendering;

            new System.Threading.Timer((t) =>
            {
                App.Current.Dispatcher.Invoke(() => fps.Text = _frameCounter.ToString());
                _frameCounter = 0;
            }).Change(0, 1000);
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            _frameCounter++;
        }
    }
}
