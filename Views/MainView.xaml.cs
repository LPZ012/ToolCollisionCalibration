using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ToolCollisionCalibration.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
            MinWinBtn.Click += (s, e) => { this.WindowState = WindowState.Minimized; };
            MinMaxWinBtn.Click += (s, e) =>
            {
                if (this.WindowState == WindowState.Normal) this.WindowState = WindowState.Maximized;
                else this.WindowState = WindowState.Normal;
            };
            CloseWinBtn.Click += (s, e) => 
            {
                var CloseResult = MessageBox.Show("关闭应用程序?", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(CloseResult == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            };
            TitleDp.MouseMove += (s, e) => { if(e.LeftButton == MouseButtonState.Pressed) this.DragMove(); };

        }
        private readonly IRegionManager _regionManager;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //_regionManager.Regions["ViewContent"].RequestNavigate("ViewB");
            _regionManager.Regions["ViewContent"].RequestNavigate("ViewA");
        }
    }
}
