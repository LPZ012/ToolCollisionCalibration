using HPSocket.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ToolCollisionCalibration.Devices;
using ToolCollisionCalibration.Models;
using ToolCollisionCalibration.Servers.Setting;
using WPFLibrary.Logger;
using WPFLibrary.Zmotion;

namespace ToolCollisionCalibration.ViewModels
{
    public class ViewCModel : INotifyPropertyChanged
    {
        public ViewCModel(ISettingServer settingServer) 
        {
            this.settingServer = settingServer;
            JogForwardMoveCommand = new DelegateCommand<object>(OnJogForwardMove);
            JogReverseMoveCommand = new DelegateCommand<object>(OnJogReverseMove);
            AxisStopCommand = new DelegateCommand<object>(OnAxisStop);
            ReturnOriginalCommand = new DelegateCommand<object>(OnReturnOriginal);
        }
        public ISettingServer settingServer { get; set; }
        public DelegateCommand<object> JogForwardMoveCommand { get; }
        public DelegateCommand<object> JogReverseMoveCommand { get; }
        public DelegateCommand<object> AxisStopCommand { get; }
        public DelegateCommand<object> ReturnOriginalCommand { get; }
        private MotionCard motionCard => settingServer.motionCard;
        private readonly ILoggers Log;
        
        /// <summary>
        /// 正向点动
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        private void OnJogForwardMove(object AxisNumber)
        {
            int axisNumber = Convert.ToInt32(AxisNumber);
            motionCard.JogMove(axisNumber, 1);
        }

        /// <summary>
        /// 反向点动
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        private void OnJogReverseMove(object AxisNumber)
        {
            int axisNumber = Convert.ToInt32(AxisNumber);
            motionCard.JogMove(axisNumber, -1);
        }

        /// <summary>
        /// 轴停止
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        private void OnAxisStop(object AxisNumber)
        {
            int axisNumber = Convert.ToInt32(AxisNumber);
            motionCard.AxisStop(axisNumber);
        }

        /// <summary>
        /// 回原
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        private async void OnReturnOriginal(object AxisNumber)
        {
            int axisNumber = Convert.ToInt32(AxisNumber);
            await motionCard.ReturnOriginal(axisNumber);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
