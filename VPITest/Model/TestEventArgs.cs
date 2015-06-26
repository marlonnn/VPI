using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPITest.Common;
using VPITest.Protocol;

namespace VPITest.Model
{
    public class StatusEventArgs : EventArgs
    {
        public List<Board> Boards;
        public TestStatus TestStatus;
    }

    public class BoardStatusEventArgs : EventArgs
    {
        public Board Board;
        public bool IsMessageSource;
    }

    public class ComponentStatusEventArgs : EventArgs
    {
        public Component Component;
    }

    public class TestStatusEventArgs : EventArgs
    {
        public TestStatus LastStatus;
        public TestStatus CurStatus;
        public string Reason;
    }

    public class NewMessageEventArgs : EventArgs
    {
        public BaseResponse rxMessage;
    }
}
