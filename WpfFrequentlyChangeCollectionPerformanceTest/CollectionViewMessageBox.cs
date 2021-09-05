using GalaSoft.MvvmLight;
using ShareLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Data;
using System.Windows.Interop;

namespace WpfFrequentlyChangeCollectionPerformanceTest
{
    /// <summary>
    /// CollectionView와 BindingOperations.EnableCollectionSynchronization 메서드를 사용하여 주스레드에 동기화하는,
    /// 단위 시간마다 추가되는 InboundMessage 개체의 목록을 나타냅니다.
    /// </summary>
    public class CollectionViewMessageBox : ViewModelBase, IMessageBox
    {
        private object _messageCollectionLock = new object();
        private object _lock = new object();
        private MessagePump _messagePump;
        private ObservableCollection<InboundMessage> _inboundMessages;
        private System.Timers.Timer _removeMessageTimer;
        private Stopwatch _stopwatch;
        private int _incomeCount = 0;

        private ICollectionView _messageCollectionView;

        private string _elapsedTime;
        public string ElapsedTime
        {
            get { return _elapsedTime; }
            set { Set(ref _elapsedTime, value, nameof(ElapsedTime)); }
        }

        public IEnumerable InboundMessages
        {
            get { return _messageCollectionView; }
        }

        private bool _isRunning = false;
        public bool IsRunning { get { return _isRunning; } }

        public CollectionViewMessageBox(int messagePerSec)
        {
            _messagePump = new MessagePump(messagePerSec);
            _messagePump.Pumped += MessagePump_Pumped;

            _inboundMessages = new ObservableCollection<InboundMessage>();
            _inboundMessages.CollectionChanged += InboundMessages_CollectionChanged;
            BindingOperations.EnableCollectionSynchronization(_inboundMessages, _messageCollectionLock);

            _removeMessageTimer = new System.Timers.Timer(35);
            _removeMessageTimer.Elapsed += RemoveMessageTimer_Elapsed;

            _stopwatch = new Stopwatch();

            _messageCollectionView = CollectionViewSource.GetDefaultView(_inboundMessages);
            _messageCollectionView.SortDescriptions.Add(
                new SortDescription(nameof(InboundMessage.Sequence),
                ListSortDirection.Descending));
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

        private void InboundMessages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ElapsedTime = _stopwatch.Elapsed.TotalMilliseconds.ToString();
        }

        private void MessagePump_Pumped(object sender, MessagePumpEventArgs e)
        {
            lock (_lock)
            {
                _inboundMessages.Add(e.Message);
                _incomeCount++;

                if (_incomeCount >= 1000)
                {
                    Stop();
                }
            }
        }

        private void RemoveMessageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                if (_inboundMessages.Count > 0)
                    _inboundMessages.RemoveAt(0);
            }
        }
    }
}
