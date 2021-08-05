using System;
using System.Timers;

namespace ShareLib
{
    /// <summary>
    /// 초당 지정한 수의 메세지를 발생시킵니다
    /// </summary>
    public class MessagePump
    {
        private Int64 _sequence = 0;
        private Timer _pumpingTimer;

        public event EventHandler<MessagePumpEventArgs> Pumped;

        public MessagePump(int messagePerSec)
        {
            var interval = 1000 / messagePerSec;
            _pumpingTimer = new Timer(interval);
            _pumpingTimer.Elapsed += _pumpingTimer_Elapsed;
        }

        private void _pumpingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            InboundMessage message = new InboundMessage();
            message.Sequence = _sequence++;
            message.Message = Guid.NewGuid().ToString();
            message.CreateDateTime = DateTime.Now;

            Pumped?.Invoke(this, new MessagePumpEventArgs(message));
        }

        public void Start()
        {
            _pumpingTimer.Start();
        }

        public void Stop()
        {
            _pumpingTimer.Stop();
        }
    }
}
