using ToolCollisionCalibration.Models;
using ToolCollisionCalibration.Normal;
using ToolCollisionCalibration.Servers.Message.ViewBToViewA;
using ToolCollisionCalibration.Servers.Setting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFLibrary.Json;
using WPFLibrary.Logger;

namespace ToolCollisionCalibration.ViewModels
{
    public class ViewBModel:INotifyPropertyChanged
    {
        public ViewBModel(ISettingServer IsettingServer, IEventAggregator eventAggregator)
        {
            _IsettingServer = IsettingServer;
            SaveParamsCommand = new DelegateCommand(Save);
            GetParamsCommand = new DelegateCommand(() => { _IsettingServer.GetParamsFromDB(); });
            UpDateParamsCommand = new DelegateCommand(_IsettingServer.UpDateParamsToDB);
            LoadModelCommand = new DelegateCommand(LoadModel);
            _eventAggregator = eventAggregator;

        }
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IEventAggregator _eventAggregator;
        public DelegateCommand LoadModelCommand { get; }
        public DelegateCommand SaveParamsCommand { get; }
        public DelegateCommand UpDateParamsCommand { get; }
        public DelegateCommand GetParamsCommand { get; }
        private readonly ILoggers _Log;
        public ISettingServer _IsettingServer { get; set; }
        private ViewBToViewAModel _ViewBToViewAModel = new ViewBToViewAModel();

        private void LoadModel()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "请选择一个型号";
                openFileDialog.InitialDirectory = FilePath.DBParamFolder;
                openFileDialog.Filter = "JSON 文件 (*.json)|*.json";
                if (openFileDialog.ShowDialog() == true)
                {
                    // 6. 获取文件路径
                    string selectedFilePath = openFileDialog.FileName;
                    var db = JsonHelper.ReadJson<DBParams>(openFileDialog.FileName);
                    if (db == null)
                    {
                        MessageBox.Show("文件内容无效或格式错误。");
                    }
                    else
                    {
                        _IsettingServer.ResetParams(db);
                        MessageBox.Show("加载型号成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Save()
        {
            ToViewAModel();
            if (_IsettingServer.WriteSetting()) MessageBox.Show("保存成功!", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            else MessageBox.Show("保存失败!", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 发送消息到视图A
        /// </summary>
        private void ToViewAModel()
        {
            _eventAggregator.GetEvent<ViewBToViewAServer>().Publish(_ViewBToViewAModel);
        }

    }
}
