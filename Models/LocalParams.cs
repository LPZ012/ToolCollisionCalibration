using ToolCollisionCalibration.Normal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLibrary.ComSerialPort;

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
        /// <summary>
        /// 加水限制重量
        /// </summary>
        public float AddWaterWeightLimit { get; set; } = 0;
        /// <summary>
        /// 电流系数
        /// </summary>
        public float CurrentCoefficient { get; set; } = 1; 
        /// <summary>
        /// 电压系数
        /// </summary>
        public float VoltageCoefficient { get; set; } = 1;
        /// <summary>
        /// 功率系数
        /// </summary>
        public float PowerCoefficient { get; set; } = 1;
        /// <summary>
        /// 温度系数
        /// </summary>
        public float TemperatureCoefficient { get; set; } = 1;
        //public float AddWaterWeightCoefficient { get; set; } = 1;
        //public float RightAddWaterWeightCoefficient { get; set; } = 1;
        //public float LeftOutWaterWeightCoefficient { get; set; } = 1;
        //public float RightOutWaterWeightCoefficient { get; set; } = 1;
        public string WorkStation { get; set; }
        public string ProdLineNo { get; set; }
        public SerialPortModel VoltageCurrentPowerModel { get; set; }
        public SerialPortModel TemperatureModel { get; set; }
        public SerialPortModel LeftScannerModel { get; set; }
        public SerialPortModel RightScannerModel { get; set; }
        public SerialPortModel AddWaterWeightModel { get; set; }
        public SerialPortModel LeftES3XXModel { get; set; }
        public SerialPortModel RightES3XXModel { get; set; }

	}
}
