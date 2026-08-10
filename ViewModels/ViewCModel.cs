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
using WPFLibrary.Logger.DataGridLog;

namespace ToolCollisionCalibration.ViewModels
{
    public class ViewCModel : INotifyPropertyChanged
    {
        public ViewCModel(ISettingServer settingServer, IDataGridLogHelper Log) 
        {
            this.settingServer = settingServer;
            this.Log = Log;
            SaveParamsCommand = new DelegateCommand(() => settingServer.WriteSetting());
            JogForwardMoveCommand = new DelegateCommand<object>(OnJogForwardMove);
            JogReverseMoveCommand = new DelegateCommand<object>(OnJogReverseMove);
            AxisStopCommand = new DelegateCommand<object>(OnAxisStop);
            ReturnOriginalCommand = new DelegateCommand<object>(OnReturnOriginal);
            SetPositionCommand = new DelegateCommand<string>(SetAbsPosition);
            AbsMoveCommand = new DelegateCommand<string>(MoveAbs);
            TurnOffOutPutCommand = new DelegateCommand<object>(TurnOffOutPut);
            TurnOnOutPutCommand = new DelegateCommand<object>(TurnOnOutPut);
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

        /// <summary>
        /// 关闭输出命令
        /// </summary>
        public DelegateCommand<object> TurnOffOutPutCommand { get; }

        /// <summary>
        /// 打开输出命令
        /// </summary>
        public DelegateCommand<object> TurnOnOutPutCommand { get; }

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
            if (settingServer.settingModel.IsRunning) return;
            switch (Command)
            {

               case "X_AbsMoveCommand":
                    var X_MoveAbsResult = motionCard.ZAux_Direct_Single_MoveAbs(1, settingServer.settingModel.localparams.AxisParamModels[1], X_AbsMovePostion);
                    if(!X_MoveAbsResult.IsSuccess) Log.Write(X_MoveAbsResult.ErrMessage, LogType.错误);
                    break;
                case "Y_AbsMoveCommand":
                    var Y_MoveAbsResult = motionCard.ZAux_Direct_Single_MoveAbs(2, settingServer.settingModel.localparams.AxisParamModels[2], Y_AbsMovePostion);
                    if (!Y_MoveAbsResult.IsSuccess) Log.Write(Y_MoveAbsResult.ErrMessage, LogType.错误);
                    break;
            }
        }

        /// <summary>
        /// 正向点动
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        private void OnJogForwardMove(object AxisNumber)
        {
            if (settingServer.settingModel.IsRunning) return;
            int axisNumber = Convert.ToInt32(AxisNumber);
            var JogForwardMoveResult = motionCard.JogMove(axisNumber, 1);
            if (!JogForwardMoveResult.IsSuccess) Log.Write(JogForwardMoveResult.ErrMessage, LogType.错误);
        }

        /// <summary>
        /// 反向点动
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        private void OnJogReverseMove(object AxisNumber)
        {
            if (settingServer.settingModel.IsRunning) return;
            int axisNumber = Convert.ToInt32(AxisNumber);
            var JogReverseMoveResult =  motionCard.JogMove(axisNumber, -1);
            if (!JogReverseMoveResult.IsSuccess) Log.Write(JogReverseMoveResult.ErrMessage, LogType.错误);
        }

        /// <summary>
        /// 轴停止
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        private void OnAxisStop(object AxisNumber)
        {
            if (settingServer.settingModel.IsRunning) return;
            int axisNumber = Convert.ToInt32(AxisNumber);
            var AxisStopResult =  motionCard.AxisStop(axisNumber);
            if (!AxisStopResult.IsSuccess) Log.Write(AxisStopResult.ErrMessage, LogType.错误);
        }

        /// <summary>
        /// 回原
        /// </summary>
        /// <param name="AxisNumber">轴号</param>
        private async void OnReturnOriginal(object AxisNumber)
        {
            if (settingServer.settingModel.IsRunning) return;
            int axisNumber = Convert.ToInt32(AxisNumber);
            var ReturnOriginResult =  await Task.Run(() => motionCard.ReturnOrigin(axisNumber));
            if(!ReturnOriginResult.IsSuccess) Log.Write(ReturnOriginResult.ErrMessage, LogType.错误);
        }

        /// <summary>
        /// 关闭输出
        /// </summary>
        /// <param name="ionum"></param>
        private void TurnOffOutPut(object ionum)
        {
            int IoNum = Convert.ToInt32(ionum);
            motionCard.ZAux_Direct_SetOp(IoNum, 0);
        }

        /// <summary>
        /// 打开输出
        /// </summary>
        /// <param name="ionum"></param>
        private void TurnOnOutPut(object ionum)
        {
            int IoNum = Convert.ToInt32(ionum);
            motionCard.ZAux_Direct_SetOp(IoNum, 1);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
