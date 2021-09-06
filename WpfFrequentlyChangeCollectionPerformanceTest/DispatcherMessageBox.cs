using GalaSoft.MvvmLight;
using ShareLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace WpfFrequentlyChangeCollectionPerformanceTest
{
    /// <summary>
    /// Dispatcher를 통해 주스레드에 동기화하는,
    /// 단위 시간마다 추가되는 InboundMessage 개체의 목록을 나타냅니다.
    /// </summary>
    public class DispatcherMessageBox : ViewModelBase, IMessageBox
    {
        private object _lock = new object();
        private int _incomeCount = 0;
        private DispatcherPriority _dispatcherPriority;
        private MessagePump _messagePump;
        private Timer _removeMessageTimer;
        private Stopwatch _stopwatch;
        private ObservableCollection<InboundMessage> _inboundMessages;
        private bool _useInvoke;

        public IEnumerable InboundMessages
        {
            get { return _inboundMessages; }
        }

        private bool _isRunning = false;
        public bool IsRunning { get { return _isRunning; } }

        private string _elapsedTime = string.Empty;
        public string ElapsedTime
        {
            get { return _elapsedTime; }
            set { Set(ref _elapsedTime, value, nameof(ElapsedTime)); }
        }

        public DispatcherMessageBox(int messagePerSec, bool useInvoke, DispatcherPriority dispatcherPriority = DispatcherPriority.Normal)
        {
            _useInvoke = useInvoke;
            _dispatcherPriority = dispatcherPriority;
            _inboundMessages = new ObservableCollection<InboundMessage>();

            _messagePump = new MessagePump(messagePerSec);
            _messagePump.Pumped += MessagePump_Pumped;

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

        private void MessagePump_Pumped(object sender, MessagePumpEventArgs e)
        {
            if (_useInvoke)
                App.Current.Dispatcher.Invoke(
                    () => _inboundMessages.Insert(0, e.Message),
                    _dispatcherPriority);
            else
                App.Current.Dispatcher.BeginInvoke(
                    new Action(() => _inboundMessages.Insert(0, e.Message)),
                    _dispatcherPriority);

            _incomeCount++;

            if (_incomeCount >= 1000)
                Stop();
        }

        private void InboundMessages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ElapsedTime = _stopwatch.Elapsed.TotalMilliseconds.ToString();
        }

        private void RemoveMessageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_useInvoke)
                App.Current.Dispatcher.Invoke(
                    () =>
                    {
                        if (_inboundMessages.Count > 0)
                            _inboundMessages.RemoveAt(_inboundMessages.Count - 1);
                    },
                    _dispatcherPriority);
            else
                App.Current.Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        if (_inboundMessages.Count > 0)
                            _inboundMessages.RemoveAt(_inboundMessages.Count - 1);
                    }),
                    _dispatcherPriority);
        }
    }
}
