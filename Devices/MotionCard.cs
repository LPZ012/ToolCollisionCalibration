using HPSocket;
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
        }
        private readonly List<AxisParamModel> axisParamModels;
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
        public async Task<ResultInfo> ReturnOriginal(int AxisNumber)
        {
            int OriginIOnum = 0;
            if (AxisNumber == 1) OriginIOnum = 1;
            else if (AxisNumber == 2) OriginIOnum = 2;
            else return new ResultInfo();
            var axisparam = axisParamModels[AxisNumber];

            return await ReturnOriginal(AxisNumber, axisparam, -999999, 100, OriginIOnum, 2);
        }
    }
}
