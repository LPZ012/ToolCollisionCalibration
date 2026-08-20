using ToolCollisionCalibration.Normal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLibrary.ComSerialPort;
using WPFLibrary.Zmotion;
using System.ComponentModel;

namespace ToolCollisionCalibration.Models
{
    public class LocalParams: INotifyPropertyChanged
    {
        /// <summary>
        /// 扫码使能
        /// </summary>
        public bool ScanEnable { get; set; } = true;
        /// <summary>
        /// 调试使能
        /// </summary>
        public bool DebugEnable { get; set; } = true;
        public string WorkStation { get; set; }
        public string ProdLineNo { get; set; }
        /// <summary>
        /// 销钉轴初始位置
        /// </summary>
        public float PinAxis_InitialPosition { get; set; } = 0;
        /// <summary>
        /// 销钉轴工作位置
        /// </summary>
        public float PinAxis_WorkPosition { get; set;} = 0;
        /// <summary>
        /// 定位轴初始位置
        /// </summary>
        public float LocationAxis_InitialPosition { get; set; } = 0;
        /// <summary>
        /// 定位轴工作位置
        /// </summary>
        public float LocationAxis_WorkPosition { get; set; } = 0;

        /// <summary>
        /// 角度系数
        /// </summary>
        public float AngleCoefficient { get; set; } = 1;

        public SerialPortModel ScannerModel { get; set; }
        public SerialPortModel TorqueModel { get; set; }
        public SerialPortModel AngleModel { get; set; }
        public List<AxisParamModel> AxisParamModels { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
