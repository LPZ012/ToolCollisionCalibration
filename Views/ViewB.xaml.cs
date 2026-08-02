using ToolCollisionCalibration.Models;
using ToolCollisionCalibration.Normal;
using ToolCollisionCalibration.Servers.Message.ViewBToViewA;
using ToolCollisionCalibration.Servers.Setting;
using Prism.Events;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToolCollisionCalibration.Views
{
    /// <summary>
    /// ViewB.xaml 的交互逻辑
    /// </summary>
    public partial class ViewB : UserControl
    {
        public ViewB()
        {
            InitializeComponent();
        }
    }
}
