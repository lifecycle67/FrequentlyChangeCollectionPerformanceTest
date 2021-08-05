namespace ShareLib
{
    public class MessagePumpEventArgs
    {
        private InboundMessage _message;
        public InboundMessage Message => _message;

        public MessagePumpEventArgs(InboundMessage message)
        {
            _message = message;
        }
    }
}