using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCollisionCalibration.Models
{
    /// <summary>
    /// 所有从本地加载的所需的模块全部放在此类中
    /// </summary>
    public class SettingModel:INotifyPropertyChanged
    {
        
        /// <summary>
        /// 本地参数模型
        /// </summary>
        public LocalParams localparams { get; set; } = new LocalParams();
        public DBParams DBParams { get; set; } = new DBParams();
        public ObservableCollection<AxisItem> AxisItems { get; set; } = new ObservableCollection<AxisItem>();

        /// <summary>
        /// 运行状态
        /// </summary>
        public bool IsRunning { get; set; } = false;
        /// <summary>
        /// 复位状态
        /// </summary>
        public bool IsReset { get; set; } = false;


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
