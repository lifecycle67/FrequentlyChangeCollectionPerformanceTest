using System;
using System.Collections.Concurrent;
using System.Timers;

namespace ShareLib
{
    /// <summary>
    /// 초당 지정한 수의 메세지를 발생시킵니다
    /// </summary>
    public class MessagePump
    {
        private Int64 _sequence = 0;
        private HighPrecisionTimer _highPrecisionTimer;
        private int _interval;

        //private System.Threading.Timer _timer;

        public event EventHandler<MessagePumpEventArgs> Pumped;
        //public ConcurrentQueue<MessagePumpEventArgs> PumpedMessages { get; set; }

        public MessagePump(int messagePerSec)
        {
            _interval = 1000 / messagePerSec;
        }

        private void _highPrecisionTimer_Tick(object sender, HighPrecisionTimer.TickEventArgs e)
        {
            InboundMessage message = new InboundMessage();
            message.Sequence = _sequence++;
            message.Message = Guid.NewGuid().ToString();
            message.CreateDateTime = DateTime.Now;

            Pumped?.Invoke(this, new MessagePumpEventArgs(message));
        }

        public void Start()
        {
            _highPrecisionTimer = new HighPrecisionTimer(_interval);
            _highPrecisionTimer.Tick += _highPrecisionTimer_Tick;
        }

        public void Stop()
        {
            _highPrecisionTimer.Dispose();
        }
    }
}
