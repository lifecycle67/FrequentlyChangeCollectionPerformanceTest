using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLib
{
    /// <summary>
    /// 메세지 수신 동작 및 수신 목록을 정의합니다
    /// </summary>
    public interface IMessageBox
    {
        /// <summary>
        /// 동작 여부를 가져옵니다
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// 수신받은 메세지의 목록을 가져옵니다
        /// </summary>
        IEnumerable InboundMessages { get; }
        /// <summary>
        /// 동작 경과시간을 나타냅니다
        /// </summary>
        string ElapsedTime { get; set; }
        /// <summary>
        /// 메세지 수신을 시작합니다
        /// </summary>
        void Start();
        /// <summary>
        /// 메세지 수신을 중지합니다
        /// </summary>
        void Stop();
    }
}
