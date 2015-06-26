using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Summer.System.Collections.Concurrent;
using VPITest.Protocol;

namespace VPITest.Common
{
    public class RxMsgQueue : ConcurrentQueue<BaseResponse>
    {

    }

    public class TxMsgQueue : ConcurrentQueue<BaseRequest>
    {

    }
}
