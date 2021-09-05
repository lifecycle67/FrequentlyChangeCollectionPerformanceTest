using GalaSoft.MvvmLight;
using Microsoft.Toolkit.Uwp.UI;
using ShareLib;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;

namespace UwpFrequentlyChangeCollectionPerformanceTest
{
    internal class DispatcherMessageBox : ViewModelBase, IMessageBox
    {
        private object _lock = new object();
        private MessagePump _messagePump;
        private ObservableCollection<InboundMessage> _inboundMessages;
        private System.Timers.Timer _removeMessageTimer;
        private Stopwatch _stopwatch;
        private int _incomeCount = 0;
        private CoreDispatcherPriority _dispatcherPriority;

        private string _elapsedTime;
        public string ElapsedTime
        {
            get { return _elapsedTime; }
            set { Set(ref _elapsedTime, value, nameof(ElapsedTime)); }
        }

        public IEnumerable InboundMessages
        {
            get { return _inboundMessages; }
        }

        private bool _isRunning = false;
        public bool IsRunning { get { return _isRunning; } }

        public DispatcherMessageBox(int messagePerSec, CoreDispatcherPriority dispatcherPriority)
        {
            _dispatcherPriority = dispatcherPriority;

            _messagePump = new MessagePump(messagePerSec);
            _messagePump.Pumped += MessagePump_Pumped;

            _inboundMessages = new ObservableCollection<InboundMessage>();
            _inboundMessages.CollectionChanged += InboundMessages_CollectionChanged;

            _removeMessageTimer = new System.Timers.Timer(35);
            _removeMessageTimer.Elapsed += RemoveMessageTimer_Elapsed;

            _stopwatch = new Stopwatch();
        }

        public void Start()
        {
            _isRunning = true;
            _messagePump.Start();
            _removeMessageTimer.Start();

            if (_stopwatch.IsRunning == false)
                _stopwatch.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            _messagePump.Stop();
            _removeMessageTimer.Stop();
            _stopwatch.Stop();
        }

        private async void RemoveMessageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
               _dispatcherPriority,
               () =>
               {
                   lock (_lock)
                   {
                       if (_inboundMessages.Count > 0)
                           _inboundMessages.RemoveAt(_inboundMessages.Count - 1);
                   }
               });
        }

        private void InboundMessages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ElapsedTime = _stopwatch.Elapsed.TotalMilliseconds.ToString();
        }

        private async void MessagePump_Pumped(object sender, MessagePumpEventArgs e)
        {

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                _dispatcherPriority,
                () =>
                {
                    lock (_lock)
                    {
                        _inboundMessages.Insert(0, e.Message);
                    }
                });

            _incomeCount++;

            if (_incomeCount >= 1000)
            {
                Stop();
            }
        }
    }
}