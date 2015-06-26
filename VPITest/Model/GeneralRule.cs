using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPITest.Protocol;
using VPITest.Common;

namespace VPITest.Model
{
    public class GeneralRule : FctRule
    {
        Dictionary<Board, Board[]> matrix;

        public GeneralRule(Cabinet cabinet, RxMsgQueue rxMsgQueue, Dictionary<Board, Board[]> matrix)
            : base(cabinet, rxMsgQueue)
        {
            this.matrix = matrix;
        }

        /// <summary>
        /// 某板卡报错后，不一定只是此板卡的问题，还有可能是其他板卡的问题，关联关系通过matrix判定矩阵判断
        /// </summary>
        /// <param name="errorBoard"></param>
        /// <returns></returns>
        public Dictionary<Board,bool> JudgeBoardPassStatus(Board errorBoard)
        {
            List<Board> maybeErrors = new List<Board>();
            if (matrix.ContainsKey(errorBoard))
            {
                maybeErrors.AddRange(matrix[errorBoard]);
            }
            errorBoard.IsGeneralTestPassed = false;

            Dictionary<Board, bool> dicts = new Dictionary<Board, bool>();
            dicts.Add(errorBoard, true);
            foreach (var b in maybeErrors)
            {
                b.IsGeneralTestPassed = false;
                if( !dicts.ContainsKey(b))
                    dicts.Add(b, false);
            }
            return dicts;
        }

        /// <summary>
        /// 过滤VPS板卡消息，当错误码大于零是报错
        /// </summary>
        /// <param name="cr"></param>
        public void ProcessFilter(VPSErrorInfoResponse cr)
        {
            if (cr.ErrorCode > 0)
            {
                FiltedVPSErrorInfoResponse fr = FiltedVPSErrorInfoResponse.CreateNew(cr);
                rxMsgQueue.Push(fr);
            }
        }

        /// <summary>
        /// 过滤VIB板卡正常灯位消息
        /// </summary>
        /// <param name="cr"></param>
        public void ProcessFilter(VIBTestResponse vvr)
        {
            if (vvr.ExpectedCode != vvr.RealCode)
            {
                FiltedVIBTestResponse fr = new FiltedVIBTestResponse(vvr);
                rxMsgQueue.Push(fr);
            }
        }

        /// <summary>
        /// 过滤VOB板卡正常灯位消息
        /// </summary>
        /// <param name="cr"></param>
        public void ProcessFilter(VOBTestResponse vvr)
        {
            if (vvr.ExpectedCode != vvr.RealCode)
            {
                FiltedVOBTestResponse fr = new FiltedVOBTestResponse(vvr);
                rxMsgQueue.Push(fr);
            }
        }

        /// <summary>
        /// 过滤板卡状态消息
        /// </summary>
        /// <param name="cr"></param>
        public void ProcessFilter(BoardStatusResponse br)
        {
            if (br.ErrorCode != 0x01 && br.ErrorCode != 0x02)
            {
                FiltedBoardStatusResponse fr = FiltedBoardStatusResponse.CreateNew(br);
                rxMsgQueue.Push(fr);
            }
        }
    }
}
