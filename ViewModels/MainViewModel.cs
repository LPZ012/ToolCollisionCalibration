using ToolCollisionCalibration.Devices;
using ToolCollisionCalibration.Models;
using ToolCollisionCalibration.Servers.Setting;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ToolCollisionCalibration.ViewModels
{
    public class MainViewModel:INotifyPropertyChanged
    {
        public MainViewModel(IRegionManager regionManager, ISettingServer settingServer)
        {
            _regionManager = regionManager;
            localparams = settingServer.settingModel.localparams;
            NavigateCommand = new DelegateCommand<string>(DisplayView);
            TimerInit();
        }
        private readonly IRegionManager _regionManager;
        public LocalParams localparams { get; }
        /// <summary>
        /// 切换视图命令
        /// </summary>
        public DelegateCommand<string> NavigateCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 定时器
        /// </summary>
        private DispatcherTimer timer;
        /// <summary>
        /// 系统时间
        /// </summary>
        public string SystemTime { get; set; }


        /// <summary>
        /// 显示视图
        /// </summary>
        /// <param name="View"></param>
        private void DisplayView(string View)
        {
            _regionManager.Regions["ViewContent"].RequestNavigate(View);
        }
        /// <summary>
        /// 初始化定时器
        /// </summary>
        private void TimerInit()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                SystemTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            };
            timer.Start();
        }

    }
}
