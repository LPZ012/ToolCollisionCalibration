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

namespace ToolCollisionCalibration.ViewModels
{
    public class ViewCModel : INotifyPropertyChanged
    {
        public ViewCModel(ISettingServer settingServer) 
        {
            this.settingServer = settingServer;
            SaveParamsCommand = new DelegateCommand(() => settingServer.WriteSetting());
            JogForwardMoveCommand = new DelegateCommand<object>(OnJogForwardMove);
            JogReverseMoveCommand = new DelegateCommand<object>(OnJogReverseMove);
            AxisStopCommand = new DelegateCommand<object>(OnAxisStop);
            ReturnOriginalCommand = new DelegateCommand<object>(OnReturnOriginal);
            SetPositionCommand = new DelegateCommand<string>(SetAbsPosition);
            AbsMoveCommand = new DelegateCommand<string>(MoveAbs);
        }
        public ISettingServer settingServer { get; set; }
        /// <summary>
        /// X轴绝对移动位置
        /// </summary>
        public float X_AbsMovePostion { get; set; }
        /// <summary>
        /// Y轴绝对移动位置
        /// </summary>
        public float Y_AbsMovePostion { get; set; }
        public DelegateCommand SaveParamsCommand { get; }
        public DelegateCommand<object> JogForwardMoveCommand { get; }
        public DelegateCommand<object> JogReverseMoveCommand { get; }
        public DelegateCommand<object> AxisStopCommand { get; }
        public DelegateCommand<object> ReturnOriginalCommand { get; }
        /// <summary>
        /// 设置位置命令
        /// </summary>
        public DelegateCommand<string> SetPositionCommand { get; }
        /// <summary>
        /// 绝对运动命令
        /// </summary>
        public DelegateCommand<string> AbsMoveCommand { get; }
        private MotionCard motionCard => settingServer.motionCard;
        private readonly ILoggers Log;
        
        /// <summary>
        /// 标定取销钉和打销钉位置
        /// </summary>
        /// <param name="Command">命令</param>
        private void SetAbsPosition(string Command)
        {
            switch(Command)
            {
                case "X_TakePinPositionCommand":
                    settingServer.settingModel.localparams.X_TakePinPosition = settingServer.settingModel.AxisItems[1].Mpos;
                    break;
                case "X_PinPositionCommand":
                    settingServer.settingModel.localparams.X_PinPosition = settingServer.settingModel.AxisItems[1].Mpos;
                    break;
                case "Y_TakePinPositionCommand":
                    settingServer.settingModel.localparams.Y_TakePinPosition = settingServer.settingModel.AxisItems[2].Mpos;
                    break;
                case "Y_PinPositionCommand":
                    settingServer.settingModel.localparams.Y_PinPosition = settingServer.settingModel.AxisItems[2].Mpos;
                    break;
            }
        }
        /// <summary>
        /// 绝对运动
        /// </summary>
        /// <param name="Command"></param>
        public void MoveAbs(string  Command)
        {
            if (settingServer.settingModel.IsRunning || !settingServer.settingModel.IsReset) return;
            switch (Command)
            {

               case "X_AbsMoveCommand":
                    motionCard.ZAux_Direct_Single_MoveAbs(1, settingServer.settingModel.localparams.AxisParamModels[1], X_AbsMovePostion);
                    break;
                case "Y_AbsMoveCommand":
                    motionCard.ZAux_Direct_Single_MoveAbs(2, settingServer.settingModel.localparams.AxisParamModels[2], Y_AbsMovePostion);
                    break;
            }
        }

        /// <summary>
        /// 正向点动
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        private void OnJogForwardMove(object AxisNumber)
        {
            if (settingServer.settingModel.IsRunning || !settingServer.settingModel.IsReset) return;
            int axisNumber = Convert.ToInt32(AxisNumber);
            motionCard.JogMove(axisNumber, 1);
        }

        /// <summary>
        /// 反向点动
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        private void OnJogReverseMove(object AxisNumber)
        {
            if (settingServer.settingModel.IsRunning || !settingServer.settingModel.IsReset) return;
            int axisNumber = Convert.ToInt32(AxisNumber);
            motionCard.JogMove(axisNumber, -1);
        }

        /// <summary>
        /// 轴停止
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        private void OnAxisStop(object AxisNumber)
        {
            if (settingServer.settingModel.IsRunning || !settingServer.settingModel.IsReset) return;
            int axisNumber = Convert.ToInt32(AxisNumber);
            motionCard.AxisStop(axisNumber);
        }

        /// <summary>
        /// 回原
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        private async void OnReturnOriginal(object AxisNumber)
        {
            if (settingServer.settingModel.IsRunning || !settingServer.settingModel.IsReset) return;
            int axisNumber = Convert.ToInt32(AxisNumber);
            await Task.Run(() => motionCard.ReturnOrigin(axisNumber));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
