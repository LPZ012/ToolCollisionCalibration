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
        public double LeftAddWaterRealWeight { get; set; }
        public double RightAddWaterRealWeight { get; set; }
        public double LeftOutWaterRealWeight { get; set; }
        public double RightOutWaterRealWeight { get; set; }
        public double LeftRealPower { get; set; }
        public double RightRealPower { get; set; }
        public double LeftRealCurrent { get; set; }
        public double RightRealCurrent { get; set; }
        public double LeftRealVoltage { get; set; }
        public double RightRealVoltage { get; set; }
        public double LeftRealTemperature { get; set; }
        public double RightRealTemperature { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
