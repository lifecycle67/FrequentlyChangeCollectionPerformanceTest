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
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (_sw.IsRunning == false)
                _sw.Start();

            _frameCounter++;

            if (_sw.ElapsedMilliseconds <= 1000)
                return;

            long frameRate = (long)(_frameCounter / _sw.Elapsed.TotalMilliseconds * 1000);
            if (frameRate > 0)
                fps.Text = frameRate.ToString();

            if (_sw.Elapsed.TotalSeconds >= 8)
            {
                _sw.Restart();
                _frameCounter = 0;
            }
        }
    }
}
