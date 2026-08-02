using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCollisionCalibration.Models
{
    public class DBParams:INotifyPropertyChanged
    {
        public string OrderNum { get; set; }
        public string ProductModel { get; set; }
        public int FailedLimit { get; set; }
        public int TimeLimit { get; set; }
        /// <summary>
        /// 磨豆电机启动时间
        /// </summary>
        public int StartGrindMotorTime { get; set; }
        /// <summary>
        /// 高压煮水时间
        /// </summary>
        public float BoilerWaterUnderHighPressureTime { get; set; }
        /// <summary>
        /// 热水时间
        /// </summary>
        public float HotWaterTime { get; set; }
        /// <summary>
        /// 热水手动阀倾斜使能
        /// </summary>
        public bool HotwWaterManualValveTiltEnable { get; set; }

        public List<LocalModel> ListLocalModels {  get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
