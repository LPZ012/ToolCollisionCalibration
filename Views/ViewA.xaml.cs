using ToolCollisionCalibration.Models;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using WPFLibrary.DataGridTool;
using WPFLibrary.Logger.DataGridLog;

namespace ToolCollisionCalibration.Views
{
    /// <summary>
    /// ViewA.xaml 的交互逻辑
    /// </summary>
    public partial class ViewA : UserControl
    {
        public ViewA(IDataGridLogHelper Log)
        {
            InitializeComponent();
            LogDataGrid.ItemsSource = Log.ListLog;
            Log.LogChangeEvent += () => LogDataGrid.ScrollIntoView(LogDataGrid.Items[LogDataGrid.Items.Count - 1]);



            //LogDataGrid.MouseEnter += (s, e) => Log.IsMouseEnter = true;
            //LogDataGrid.MouseLeave += (s, e) => Log.IsMouseEnter = false;
            //Log.LogChangeEvent += LogChange;
        }
    }
}
