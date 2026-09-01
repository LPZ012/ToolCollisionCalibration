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
            //设置限位和原点
            SetAxisLimitIn(1, 7, 5, 6);
            SetAxisLimitIn(2, 10, 8, 9);
        }

        /// <summary>
        /// 轴报警复位
        /// </summary>
        /// <param name="ionum">轴复位点位</param>
        /// <returns></returns>
        public async Task AxisAlarmReset(int ionum)
        {
            ZAux_Direct_SetOp(ionum, 0);
            await Task.Delay(100);
            ZAux_Direct_SetOp(ionum, 1);
        }

        /// <summary>
        /// 点动
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        /// <param name="dir">方向(1为正方向，-1为反方向)</param>
        public ResultInfo Vmove(int AxisNumber, int dir)
        {
            var axisparam = axisParamModels[AxisNumber];
            return ZAux_Direct_Single_Vmove(AxisNumber, axisparam, dir);
        }

        /// <summary>
        /// 绝对运动
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
        /// 绝对运动(带运动完成标志)
        /// </summary>
        /// <param name="AxisNumber"></param>
        /// <param name="Position"></param>
        /// <param name="IntervalTime">时间间隔，单位ms</param>
        /// <returns></returns>
        public Task<ResultInfo> MoveAbs_DoneStatus(int AxisNumber, float Position,int IntervalTime)
        {
            var axisparam = axisParamModels[AxisNumber];
            return ZAux_Direct_Single_MoveAbs_DoneStatus(AxisNumber, axisparam, Position, IntervalTime);
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
            var Autoaxisparam = axisParamModels[AxisNumber];
            var ReturnOriginalaxisparam = Autoaxisparam.DeepCopy(10);  //回零速度要慢，修改速度默认为10
            return await ReturnOriginal(AxisNumber, ReturnOriginalaxisparam, -9999, 5, 3);
        }
    }
}
