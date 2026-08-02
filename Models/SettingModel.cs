using System;
using System.Collections.Generic;
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
        /// <summary>
        /// 默认模型
        /// </summary>
        public List<ItemsModel> DefaultItemsModels { get; set; } = [];
        public List<ItemsModel> LeftSelectItemModel { get; set; } = [];
        public List<ItemsModel> RightSelectItemModel { get; set; } = [];
        /// <summary>
        /// 左侧ES30X模型
        /// </summary>
        public List<ItemsModel> LeftES3XXItemsModels { get; set; } = [];
        /// <summary>
        /// 右侧ES30X模型
        /// </summary>
        public List<ItemsModel> RightES3XXItemsModels { get; set; } = [];
        public DBParams DBParams { get; set; } = new DBParams();
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
