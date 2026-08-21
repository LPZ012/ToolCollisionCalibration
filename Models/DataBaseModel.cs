using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ToolCollisionCalibration.Models
{
    public class DataBaseModel : INotifyPropertyChanged
    {
        // --- 基础信息 ---
        [Name("订单号")]
        public string OrderNum { get; set; }

        [Name("工位")]
        public string WorkStation { get; set; }

        [Name("产品型号")]
        public string ProductModel { get; set; }

        [Name("条码编号")]
        public string BarCodeNumber { get; set; }

        [Name("SN码")]
        public string SN_Code { get; set; }

        [Name("生产线别")]
        public string ProdLineNo { get; set; }

        [Name("上传时间")]
        public string UploadTime { get; set; } = DateTime.Now.ToString("HH:mm:ss");

        [Name("测试结果")]
        public bool TestResult { get; set; } 

        [Name("报错步骤")]
        public string ErrorReportingStep { get; set; }

        [Name("报错信息")]
        public string ErrorReportingInformation { get; set; }
        

        [Name("起始角度")]
        public string StartingAngle { get; set; }

        
        [Name("结束角度")]
        public string EndAngle { get; set; }

        
        [Name("角度差值")]
        public string AngleDifference { get; set; }

        [Name("反转角度")]
        public string InvertAngleCompensation { get; set; }

        [Name("起始扭矩")]
        public string StartingTorque { get; set; }

        
        [Name("结束扭矩")]
        public string EndTorque { get; set; }

        
        [Name("扭矩差值")]
        public string TorqueDifference { get; set; }

        // --- INotifyPropertyChanged 实现 ---
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
