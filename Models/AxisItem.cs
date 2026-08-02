using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WPFLibrary.Zmotion;

namespace ToolCollisionCalibration.Models
{
    public class AxisItem:INotifyPropertyChanged
    {
        public int AxisNumber {  get; set; }

        public string AxisName { get; set; }

        public float Dpos { get; set; }

        public float Mpos { get; set; }

        public float Speed { get; set; }

        public int Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); OnPropertyChanged(nameof(StatusText)); OnPropertyChanged(nameof(StatusColor)); }
        }

        public string StatusText => Status == 0 ? "运动中" : "停止";

        public Brush StatusColor => Status == 0 ? Brushes.Red : Brushes.Green;

        public uint HomeStatus
        {
            get => _homeStatus;
            set { _homeStatus = value; OnPropertyChanged(); OnPropertyChanged(nameof(HomeStatusText)); }
        }

        public string HomeStatusText => HomeStatus == 1 ? "已回零" : "未回零";

        public AxisParamModel Param
        {
            get => _param;
            set { _param = value; OnPropertyChanged(); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
