using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib
{
    public class InboundMessage
    {
        public Int64 Sequence { get; set; }
        public string Message { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
