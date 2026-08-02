using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCollisionCalibration.Models
{
    public class ItemsModel:INotifyPropertyChanged
    {
        public ItemsModel(LocalModel localModel) 
        {
            this.localModel = localModel;
        }
        public LocalModel localModel { get; set; }
        /// <summary>
        /// 实际值
        /// </summary>
        public string ActValue { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        public string Result { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
