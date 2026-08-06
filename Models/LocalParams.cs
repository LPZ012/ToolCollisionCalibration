using ToolCollisionCalibration.Normal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLibrary.ComSerialPort;
using WPFLibrary.Zmotion;

namespace ToolCollisionCalibration.Models
{
    public class LocalParams
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
        /// X轴取销位置
        /// </summary>
        public float X_TakePinPosition { get; set; } = 0.00f;
        /// <summary>
        /// X轴打销位置
        /// </summary>
        public float X_PinPosition { get; set;} = 0.00f;
        /// <summary>
        /// Y轴取销位置
        /// </summary>
        public float Y_TakePinPosition { get; set; } = 0.00f;
        /// <summary>
        /// Y轴打销位置
        /// </summary>
        public float Y_PinPosition { get; set; } = 0.00f;
        public SerialPortModel ScannerModel { get; set; }
        public SerialPortModel TorqueModel { get; set; }
        public SerialPortModel AngleModel { get; set; }
        public List<AxisParamModel> AxisParamModels { get; set; }

	}
}
