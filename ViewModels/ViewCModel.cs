using HPSocket.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ToolCollisionCalibration.Models;
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
                JogPosMoveCommand = new DelegateCommand(OnJogPosMove);
            }

        }

        public ObservableCollection<AxisItem> AxisItems { get; set; } = new ObservableCollection<AxisItem>();
        public DelegateCommand JogPosMoveCommand { get; }

        private void OnJogPosMove()
        {
           MessageBox.Show("正向点动移动命令已触发！");
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
