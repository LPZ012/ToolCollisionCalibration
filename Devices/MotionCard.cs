using HPSocket;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLibrary.Logger;
using WPFLibrary.Result;
using WPFLibrary.Sockets.TCPIP;
using WPFLibrary.Zmotion;

namespace ToolCollisionCalibration.Devices
{
    public class MotionCard : ECI1408
    {
        public MotionCard(TCPIPModel tCPIPModel,List<AxisParamModel> axisParamModels) : base(tCPIPModel)
        {
            this.axisParamModels = axisParamModels;
            Init();
        }
        private readonly List<AxisParamModel> axisParamModels;


        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            SetAxisLimitIn(1, 7, 5, 6);
            SetAxisLimitIn(2, 10, 8, 9);
        }

        /// <summary>
        /// 设置轴的正限位、原点、负限位输入信号
        /// </summary>
        /// <param name="iaxis">轴号</param>
        /// <param name="FwdIn">正限位</param>
        /// <param name="OriginIn">原点</param>
        /// <param name="RevIn">负限位</param>
        private void SetAxisLimitIn(int iaxis, int FwdIn,int DatumIn, int RevIn)
        {
            ZAux_Direct_SetFwdIn(iaxis, FwdIn);
            ZAux_Direct_SetDatumIn(iaxis, DatumIn);
            ZAux_Direct_SetRevIn(iaxis, RevIn);
        }

        /// <summary>
        /// 点动
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        /// <param name="dir">方向(1为正方向，-1为反方向)</param>
        public ResultInfo JogMove(int AxisNumber, int dir)
        {
            var axisparam = axisParamModels[AxisNumber];
            return ZAux_Direct_Single_Vmove(AxisNumber, axisparam, dir);
        }

        /// <summary>
        /// 带运动完成信号的绝对运动
        /// </summary>
        /// <param name="AxisNumber"></param>
        /// <param name="Position"></param>
        /// <returns></returns>
        public ResultInfo MoveAbs(int AxisNumber, float Position)
        {
            var axisparam = axisParamModels[AxisNumber];
            return  ZAux_Direct_Single_MoveAbs(AxisNumber, axisparam, Position);
        }

        /// <summary>
        /// 轴停止
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        public ResultInfo AxisStop(int AxisNumber)
        {
            return ZAux_Direct_Single_Cancel(AxisNumber, 2);
        }

        /// <summary>
        /// 回原
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        public async Task<ResultInfo> ReturnOrigin(int AxisNumber)
        {
            int OriginIOnum = 0;
            if (AxisNumber == 1) OriginIOnum = 5;
            else if (AxisNumber == 2) OriginIOnum = 8;
            else return new ResultInfo();
            var axisparam = axisParamModels[AxisNumber];
            return await ReturnOriginal(AxisNumber, axisparam, -9999, OriginIOnum, 3);
        }
    }
}
