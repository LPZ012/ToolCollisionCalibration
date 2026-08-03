using HPSocket.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ToolCollisionCalibration.Models;
using WPFLibrary.Logger;
using WPFLibrary.Zmotion;

namespace ToolCollisionCalibration.ViewModels
{
    public class ViewCModel : INotifyPropertyChanged
    {
        public ViewCModel() 
        {
            for (int i = 0; i < 4; i++)
            {
                AxisItems.Add(new AxisItem
                {
                    AxisNumber = i,
                    AxisName = $"轴{i}",
                    Dpos = 0,
                    Mpos = 0,
                    Speed = 0,
                    Status = -1,
                    HomeStatus = 0,
                    Param = new AxisParamModel
                    {
                        AxisType = 1,
                        Units = 1,
                        Lspeed = 10,
                        Speed = 100,
                        Accel = 500,
                        Decel = 500,
                        Sramp = 0
                    }
                });
                
            }
            JogPosMoveCommand = new DelegateCommand<object>(OnJogPosMove);
        }

        public ObservableCollection<AxisItem> AxisItems { get; set; } = new ObservableCollection<AxisItem>();
        public DelegateCommand<object> JogPosMoveCommand { get; }
        private readonly IPulseADIOControler PulseADIOControler;
        private readonly ILoggers Log;

        private void OnJogPosMove(object AxisNumber)
        {
            int axisNumber = Convert.ToInt32(AxisNumber);
            var axisparam = AxisItems[axisNumber].Param;
            var result = PulseADIOControler?.ZAux_Direct_Single_Vmove(axisNumber, axisparam, 1);
            if(!result.IsSuccess)
            {
                Log.Write(result.ErrMessage, LogType.错误);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
