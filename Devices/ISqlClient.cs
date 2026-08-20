using ToolCollisionCalibration.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCollisionCalibration.Devices
{
    public interface ISqlClient: INotifyPropertyChanged
    {
        bool IsConnected { get; set; }
        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="Dbparams"></param>
        /// <returns></returns>
        bool UpLoad(DataBaseModel dataBaseModel);
        /// <summary>
        /// 检查看板信息
        /// </summary>
        /// <param name="OrderNum"></param>
        /// <param name="Line"></param>
        /// <param name="CustModel"></param>
        /// <param name="ManufacturingDate"></param>
        /// <returns></returns>
        bool CheckKanbanInfo(string Line, string CustModel);
        /// <summary>
        /// 更新参数
        /// </summary>
        /// <returns></returns>
        bool UpDateParameters(DBParams dBParams, string LineNum);
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <returns></returns>
        DBParams GetParameters(string LineNum);
        /// <summary>
        /// 检查SN码的失败次数和持续时间是否超过设定值
        /// </summary>
        /// <param name="SN_CODE"></param>
        /// <param name="SetCount"></param>
        /// <param name="SetTime"></param>
        /// <returns></returns>
        bool NumberDurationOfFailureChecks(string SN_CODE, int SetCount, int SetTime);
        /// <summary>
        /// 检查上一站是否通过
        /// </summary>
        /// <param name="SN_CODE"></param>
        /// <param name="ProdLineNo"></param>
        /// <param name="Product_Model"></param>
        /// <returns></returns>
        bool CheckPreviousStation(string SN_CODE, string ProdLineNo, string Product_Model);
        //void HeartBeat();
    }
}
