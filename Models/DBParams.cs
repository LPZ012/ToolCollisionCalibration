using CsvHelper.Configuration.Attributes;
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

        /// <summary>
        /// 反转角度
        /// </summary>
        public float InvertAngleCompensation { get; set; }

        public float StartingTorque { get; set; }


        public float EndTorque { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
