using ShareLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfFrequentlyChangeCollectionPerformanceTest
{
    /// <summary>
    /// ReactiveX를 이용하여 단위 시간마다 메세지 박스의 이벤트 시퀀스를 관측하고 주 스레드를 통해 목록을 업데이트합니다.
    /// </summary>
    public class RxMessageBox : IMessageBox
    {
        private bool _isRunning = false;
        private ObservableCollection<InboundMessage> _inboundMessages = new ObservableCollection<InboundMessage>();
        private MessagePump _messagePump;

        public bool IsRunning => _isRunning;

        public IEnumerable InboundMessages => _inboundMessages;

        public string ElapsedTime { get; set; }

        public RxMessageBox(int messagePerSec)
        {
            _messagePump = new MessagePump(messagePerSec);

            Observable.FromEventPattern<MessagePumpEventArgs>(
                h => _messagePump.Pumped += h,
                h => _messagePump.Pumped -= h)
                .Buffer(TimeSpan.FromMilliseconds(200))
                .ObserveOn(System.Reactive.Concurrency.DispatcherScheduler.Current)
                .Subscribe(r =>
                {
                    //Debug.WriteLine(r.Count.ToString());
                    foreach (var arg in r)
                        _inboundMessages.Insert(0, arg.EventArgs.Message);
                });
        }

        public void Start()
        {
            _isRunning = true;
            _messagePump.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            _messagePump.Stop();
        }
    }
}
