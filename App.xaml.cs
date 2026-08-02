using ToolCollisionCalibration.Devices;
using ToolCollisionCalibration.Models;
using ToolCollisionCalibration.Normal;
using ToolCollisionCalibration.Servers.Setting;
using ToolCollisionCalibration.ViewModels;
using ToolCollisionCalibration.Views;
using Example;
using Prism.Ioc;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using WPFLibrary.DataGridTool;
using WPFLibrary.Logger.DataGridLog;
namespace ToolCollisionCalibration;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainView>();
    }
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        //判断程序是否已经启动
        if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Count() > 1)
        {
            MessageBox.Show("请勿重复启动！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
            return;
        }
        //containerRegistry.RegisterSingleton<ISqlClient>(() => new SqlClient("Data Source=172.19.0.125;Initial Catalog=ES300;User ID=kwdevB;Password=uMo8!#CvaB;TrustServerCertificate=True"));
        containerRegistry.RegisterSingleton<IDataGridLogHelper>(() => new DataGridLogHelper(FilePath.LogFolder));
        containerRegistry.RegisterSingleton<ISettingServer,SettingServer>();
        containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
        containerRegistry.RegisterForNavigation<ViewA, ViewAModel>();
        containerRegistry.RegisterForNavigation<ViewB, ViewBModel>();
        containerRegistry.RegisterForNavigation<ViewC, ViewCModel>();
    }
}

