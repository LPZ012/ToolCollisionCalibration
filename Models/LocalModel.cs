using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCollisionCalibration.Models
{
    /// <summary>
    /// 本地参数
    /// </summary>
    public class LocalModel:INotifyPropertyChanged
    {
        public LocalModel() { }
        public LocalModel(int id, string itemname,string lowervalue, string uppervalue, string identify)
        {
            ID = id;
            ItemName = itemname;
            UpperValue = uppervalue;
            LowerValue = lowervalue;
            Identify = identify;
        }
        [Name("ID")]
        public int ID { get; set; }
        [Name("测试项")]
        public string ItemName { get; set; }
        [Name("上限")]
        public string UpperValue { get; set; }
        [Name("下限")]
        public string LowerValue { get; set; }
        [Name("标识")]
        public string Identify { get; set; }
        /// <summary>
        /// 启用标志位，0屏蔽，1检测
        /// </summary>
        [Name("是否启用")]
        public bool IsFlag { get; set; } = true;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
