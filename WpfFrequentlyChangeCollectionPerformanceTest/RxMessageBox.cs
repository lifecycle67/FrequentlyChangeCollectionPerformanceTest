using GalaSoft.MvvmLight;
using ShareLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WpfFrequentlyChangeCollectionPerformanceTest
{
    /// <summary>
    /// ReactiveX를 이용하여 단위 시간마다 메세지 박스의 이벤트 시퀀스를 관측하고 주 스레드를 통해 목록을 업데이트합니다.
    /// </summary>
    public class RxMessageBox : ViewModelBase, IMessageBox
    {
        private bool _isRunning = false;
        private ObservableCollection<InboundMessage> _inboundMessages = new ObservableCollection<InboundMessage>();
        private MessagePump _messagePump;
        private Stopwatch _stopwatch;
        private Timer _removeMessageTimer;
        private int _incomeCount = 0;
        private int _collectionChangedCount = 0;
        private object _lock = new object();
        IDisposable _removeChanges;

        public bool IsRunning => _isRunning;

        public IEnumerable InboundMessages => _inboundMessages;

        private string _elapsedTime;
        public string ElapsedTime
        {
            get => _elapsedTime;
            set { Set(ref _elapsedTime, value); }
        }

        public RxMessageBox(int messagePerSec)
        {
            _messagePump = new MessagePump(messagePerSec);
            _stopwatch = new Stopwatch();
            _inboundMessages.CollectionChanged += _inboundMessages_CollectionChanged;
            _removeMessageTimer = new Timer(35);

            Observable.FromEventPattern<MessagePumpEventArgs>(
                h => _messagePump.Pumped += h,
                h => _messagePump.Pumped -= h)
                .Buffer(TimeSpan.FromMilliseconds(100))
                .ObserveOn(DispatcherScheduler.Current)
                .Subscribe(r =>
                {
                    foreach (var arg in r)
                    {
                        if (_incomeCount++ >= 1000)
                            _messagePump.Stop();

                        lock (_lock)
                        {
                            _inboundMessages.Insert(0, arg.EventArgs.Message);
                        }
                    }
                });
        }

        private void _inboundMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                if (_collectionChangedCount++ >= 1000)
                    Stop();
            ElapsedTime = _stopwatch.Elapsed.TotalMilliseconds.ToString();
        }

        public void Start()
        {
            if (IsRunning == false)
            {
                _removeChanges = Observable.Timer(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(35))
                    .ObserveOn(DispatcherScheduler.Current)
                    .Subscribe(r =>
                    {
                        if (_inboundMessages.Count > 0)
                        {
                            lock (_lock)
                            {
                                _inboundMessages.RemoveAt(_inboundMessages.Count - 1);
                            }
                        }
                    });
                _stopwatch.Start();
            }

            _isRunning = true;
            _messagePump.Start();
            _removeMessageTimer.Start();
        }

        public void Stop()
        {
            if (IsRunning == true)
                _removeChanges?.Dispose();

            _isRunning = false;
            _messagePump.Stop();
            _stopwatch.Stop();
            _removeMessageTimer.Stop();
        }
    }
}
