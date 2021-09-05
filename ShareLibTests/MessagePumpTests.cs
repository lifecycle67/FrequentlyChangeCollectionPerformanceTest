using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ShareLib.Tests
{
    [TestClass()]
    public class MessagePumpTests
    {
        /// <summary>
        /// 1초에 50개의 메세지를 발생시킬 수 있는지 확인합니다
        /// </summary>
        [TestMethod()]
        public void IncomeMessageFiftyPerSecTest()
        {
            int messagePerSec = 50;

            Stopwatch sw = new Stopwatch();
            ConcurrentQueue<InboundMessage> messageQueue = new ConcurrentQueue<InboundMessage>();

            MessagePump messagePump = new MessagePump(messagePerSec);
           
            sw.Start();
            messagePump.Start();

            messagePump.Pumped += (sender, args) =>
            {
                messageQueue.Enqueue(args.Message);
            };

            while (true)
            {
                if (messageQueue.Count >= messagePerSec)
                {
                    sw.Stop();
                    messagePump.Stop();
                    break;
                }

                if (sw.ElapsedMilliseconds > 3000)
                    throw new TimeoutException();
            }

            Assert.IsTrue(sw.ElapsedMilliseconds >= 1000, sw.ElapsedMilliseconds.ToString());
            Assert.IsTrue(sw.ElapsedMilliseconds < 1050, sw.ElapsedMilliseconds.ToString());
        }
    }
}
