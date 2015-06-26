using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Summer.System.Collections.Concurrent;
using VPITest.Protocol;
using VPITest.Net;

namespace VPITest.Common
{
    public class RxQueue : ConcurrentQueue<Original>
    {

    }

    public class TxQueue : ConcurrentQueue<Original>
    {

    }
}
