using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCollisionCalibration.Models
{
    public class DeviceValueModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 实时扭矩
        /// </summary>
        public double RealTorque { get; set; }

        /// <summary>
        /// 实时角度
        /// </summary>
        public double RealAngle { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
