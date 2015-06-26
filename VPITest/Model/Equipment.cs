using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace VPITest.Model
{
    [Serializable]
    public abstract class AbstractEq
    {
        /// <summary>
        /// 设备Id，共五个字节，标示符唯一
        /// </summary>
        public byte[] EqId { get; set; }
        /// <summary>
        /// 设备名称，用于文本显示
        /// </summary>
        public string EqName { get; set; }

        public string GetIdStr()
        {
            return GetIdStr(EqId);
        }

        protected string GetIdStr(byte[] EqId)
        {
            return string.Format("{0}.{1}.{2}.{3}.{4}", EqId[0], EqId[1], EqId[2], EqId[3], EqId[4]);
        }
    }

    /// <summary>
    /// 单项测试和综合测试机柜配置相同
    /// </summary>
    [Serializable]
    public class Cabinet : AbstractEq
    {
        /// <summary>
        /// 机柜里的待测机笼
        /// </summary>
        public List<Rack> Racks { get; set; }

        /// <summary>
        /// 判断本机笼的Fct测试是否通过
        /// </summary>
        /// <returns></returns>
        public bool IsFctTestPassed()
        {
            bool result = true;
            foreach (var r in Racks)
            {
                foreach (var b in r.Boards)
                {
                    result &= b.IsFctTestPassed();
                }
            }
            return result;
        }

        public Board GetCommunicationBoard(IPEndPoint ip)
        {
            foreach (var r in Racks)
            {
                foreach (var b in r.Boards)
                {
                    if (b.IsCommincationBoard && b.CommunicationIP.Equals(ip))
                    {
                        return b;
                    }
                }
            }
            return null;
        }

        public AbstractEq FindEq(byte[] Id)
        {
            foreach (var r in Racks)
            {
                if( r.GetIdStr().Equals( GetIdStr(Id)))
                    return r;
                foreach (var b in r.Boards)
                {
                    if( b.GetIdStr().Equals( GetIdStr(Id)))
                        return b;
                    foreach (var ct in b.ComponentTypes)
                    {
                        if (ct.GetIdStr().Equals(GetIdStr(Id)))
                            return ct;
                        foreach (var c in ct.Components)
                        {
                            if (c.GetIdStr().Equals(GetIdStr(Id)))
                                return c;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获得所有FCT待测组件类型
        /// </summary>
        /// <returns></returns>
        public Dictionary<Board, List<ComponentType>> GetFctTestedComponentTypesDicts()
        {
            Dictionary<Board, List<ComponentType>> dicts = new Dictionary<Board, List<ComponentType>>();
            foreach (var r in Racks)
            {
                foreach (var b in r.Boards)
                {
                    foreach (var ct in b.ComponentTypes)
                    {
                        List<ComponentType> clist = new List<ComponentType>();
                        if (ct.IsFctTestTested)
                        {
                            if (dicts.ContainsKey(b))
                            {
                                dicts[b].Add(ct);
                            }
                            else
                            {
                                clist.Add(ct);
                                dicts.Add(b, clist);
                            }
                        }
                    }
                }
            }
            return dicts;
        }

        /// <summary>
        /// 获得所有FCT待测组件类型列表
        /// </summary>
        /// <returns></returns>
        public List<ComponentType> GetFctTestedComponentTypesList()
        {
            List<ComponentType> list = new List<ComponentType>();
            Dictionary<Board, List<ComponentType>> dicts = GetFctTestedComponentTypesDicts();
            foreach (var kv in dicts)
            {
                list.AddRange(kv.Value);
            }
            return list;
        }

        /// <summary>
        /// 获得所有FCT待测组件列表
        /// </summary>
        /// <returns></returns>
        public List<Component> GetFctTestedComponentsList()
        {
            List<Component> list = new List<Component>();
            foreach (var r in Racks)
            {
                foreach (var b in r.Boards)
                {
                    foreach (var ct in b.ComponentTypes)
                    {
                        if (ct.IsFctTestTested)
                        {
                            list.AddRange(ct.Components);
                        }
                    }
                }
            }
            return list;
        }

        public List<Board> GetGeneralTestBoardsList()
        {
            List<Board> boards = new List<Board>();
            foreach (var r in Racks)
            {
                foreach (var b in r.Boards)
                {
                    if (b.IsGeneralTestTested)
                    {
                        boards.Add(b);
                    }
                }
            }
            return boards;
        }

        public List<Board> GetAllGeneralTestBoardsList()
        {
            List<Board> boards = new List<Board>();
            foreach (var r in Racks)
            {
                foreach (var b in r.Boards)
                {
                    boards.Add(b);
                }
            }
            return boards;
        }

        public List<Board> GetCommunicationBoards()
        {
            List<Board> boards = new List<Board>();
            foreach (var r in Racks)
            {
                foreach (var b in r.Boards)
                {
                    if (b.IsCommincationBoard)
                    {
                        boards.Add(b);
                    }
                }
            }
            return boards;
        }
    }
    /// <summary>
    /// 机笼，上级为机柜类，下级为板卡类
    /// </summary>
    [Serializable]
    public class Rack : AbstractEq
    {
        /// <summary>
        /// 机笼里面配置的板卡信息
        /// </summary>
        public List<Board> Boards { get; set; }
    }
    /// <summary>
    /// 板卡，上级为机笼类，下级为组件类
    /// </summary>
    [Serializable]
    public class Board : AbstractEq
    {
        public Board()
        {
            FctTestSN = "";
            GeneralTestSN = "";
            ComponentTypes = new List<ComponentType>();
        }

        public bool IsCommincationBoard { get; set; }
        public IPEndPoint CommunicationIP { get; set; }

        public string BoardType { get; set; }

        public string FctTestSN { get; set; }
        public bool IsFctTestPassed()
        {
            bool res = true;
            foreach (var ct in ComponentTypes)
            {
                if (ct.IsFctTestTested)
                {
                    res &= ct.IsFctTestPassed();
                }
            }
            return res;
        }

        public string GeneralTestSN { get; set; }
        public bool IsGeneralTestTested { get; set; }
        public bool IsGeneralTestPassed { get; set; }
        public bool CanGeneralTest { get; set; }

        public Rack ParentRack { get; set; }

        public List<ComponentType> ComponentTypes { get; set; }        
    }

    /// <summary>
    /// 板卡上的组件类型
    /// </summary>
    [Serializable]
    public class ComponentType : AbstractEq
    {
        public ComponentType()
        {
            Components = new List<Component>();
        }

        public bool IsFctTestTested { get; set; }

        public List<Component> Components { get; set; }

        public Board ParentBoard { get; set; }

        public bool IsFctTestPassed()
        {
            bool res = true;
            foreach (var c in Components)
            {
                res &= c.IsFctTestPassed;
            }
            return res;
        }
    }

    /// <summary>
    /// 板卡上的组件
    /// </summary>
    [Serializable]
    public class Component : AbstractEq
    {
        public bool IsFctTestPassed { get; set; }

        public int AllTestTimes { get; set; }
        public int ErrorPackageTimes { get; set; }
        public int LostPackageTimes { get; set; }
        public int InterruptTimes { get; set; }

        public ComponentType ParentComponentType { get; set; }
    }
}
