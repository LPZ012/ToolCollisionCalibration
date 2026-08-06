using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using WPFLibrary.Zmotion;

namespace ToolCollisionCalibration.Models
{
    public class AxisItem:INotifyPropertyChanged
    {
        public int AxisNumber {  get; set; }

        public string AxisName { get; set; }
        public int IdleStatus{ get; set; }

        public float Dpos { get; set; }

        public float Mpos { get; set; }

        public float Speed { get; set; }
        /// <summary>
        /// 运动状态，0表示运动中，1表示停止
        /// </summary>
        public int Status { get; set; }

        public string StatusText => Status == 0 ? "运动中" : "停止";

        public Brush StatusColor => Status == 0 ? Brushes.Red : Brushes.Green;

        public AxisParamModel Param { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(Status))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusColor)));
            } 
        }
    }





    //public class AxisItem : INotifyPropertyChanged
    //{
    //    private int _axisNumber;
    //    private string _axisName;
    //    private float _dpos;
    //    private float _mpos;
    //    private float _speed;
    //    private int _status;
    //    private uint _homeStatus;
    //    private AxisParamModel _param;

    //    public int AxisNumber
    //    {
    //        get => _axisNumber;
    //        set { _axisNumber = value; OnPropertyChanged(); }
    //    }

    //    public string AxisName
    //    {
    //        get => _axisName;
    //        set { _axisName = value; OnPropertyChanged(); }
    //    }

    //    public float Dpos
    //    {
    //        get => _dpos;
    //        set { _dpos = value; OnPropertyChanged(); }
    //    }

    //    public float Mpos
    //    {
    //        get => _mpos;
    //        set { _mpos = value; OnPropertyChanged(); }
    //    }

    //    public float Speed
    //    {
    //        get => _speed;
    //        set { _speed = value; OnPropertyChanged(); }
    //    }

    //    public int Status
    //    {
    //        get => _status;
    //        set { _status = value; OnPropertyChanged(); OnPropertyChanged(nameof(StatusText)); OnPropertyChanged(nameof(StatusColor)); }
    //    }

    //    public string StatusText => Status == 0 ? "运动中" : "停止";

    //    public Brush StatusColor => Status == 0 ? Brushes.Red : Brushes.Green;

    //    public uint HomeStatus
    //    {
    //        get => _homeStatus;
    //        set { _homeStatus = value; OnPropertyChanged(); OnPropertyChanged(nameof(HomeStatusText)); }
    //    }

    //    public string HomeStatusText => HomeStatus == 1 ? "已回零" : "未回零";

    //    public AxisParamModel Param
    //    {
    //        get => _param;
    //        set { _param = value; OnPropertyChanged(); }
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;

    //    protected void OnPropertyChanged(string propertyName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}










    //public partial class MainWindow : Window, INotifyPropertyChanged
    //{
    //    private ECI1408 _controller;
    //    private DispatcherTimer _statusTimer;
    //    private bool _isConnected;
    //    private bool _isClosing;
    //    private readonly object _lockObj = new object();
    //    private bool[] _joggingFlags;

    //    public ObservableCollection<AxisItem> AxisItems { get; set; }

    //     ---- 位置设定：标定坐标显示 ----
    //    private string _calPickPosText = "X:0.00 Y:0.00 Z:0.00 R:0.00";
    //    public string CalPickPosText { get => _calPickPosText; set { _calPickPosText = value; OnPropertyChanged(); } }

    //    private string _calDrivePosText = "X:0.00 Y:0.00 Z:0.00 R:0.00";
    //    public string CalDrivePosText { get => _calDrivePosText; set { _calDrivePosText = value; OnPropertyChanged(); } }

    //    private string _calRotatePosText = "X:0.00 Y:0.00 Z:0.00 R:0.00";
    //    public string CalRotatePosText { get => _calRotatePosText; set { _calRotatePosText = value; OnPropertyChanged(); } }

    //    private string _calReturnPosText = "X:0.00 Y:0.00 Z:0.00 R:0.00";
    //    public string CalReturnPosText { get => _calReturnPosText; set { _calReturnPosText = value; OnPropertyChanged(); } }

    //    public event PropertyChangedEventHandler PropertyChanged;
    //    protected void OnPropertyChanged(string propertyName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }

    //    public MainWindow()
    //    {
    //        InitializeComponent();
    //        DataContext = this;

    //        AxisItems = new ObservableCollection<AxisItem>();
    //        _joggingFlags = new bool[4];

    //        for (int i = 0; i < 4; i++)
    //        {
    //            AxisItems.Add(new AxisItem
    //            {
    //                AxisNumber = i,
    //                AxisName = $"轴{i}",
    //                Dpos = 0,
    //                Mpos = 0,
    //                Speed = 0,
    //                Status = -1,
    //                HomeStatus = 0,
    //                Param = new AxisParamModel
    //                {
    //                    AxisType = 1,
    //                    Units = 1,
    //                    Lspeed = 10,
    //                    Speed = 100,
    //                    Accel = 500,
    //                    Decel = 500,
    //                    Sramp = 0
    //                }
    //            });
    //        }

    //        _statusTimer = new DispatcherTimer
    //        {
    //            Interval = TimeSpan.FromMilliseconds(100)
    //        };
    //        _statusTimer.Tick += StatusTimer_Tick;

    //        LoadParamsOnStartup();
    //    }

    //    private void btnConnect_Click(object sender, RoutedEventArgs e)
    //    {
    //        try
    //        {
    //            var tcpModel = new TCPIPModel
    //            {
    //                Address = txtIpAddress.Text.Trim()
    //            };

    //            AppendLog($"正在连接控制器: {txtIpAddress.Text.Trim()}");
    //            _controller = new ECI1408(tcpModel);
    //            bool connected = _controller.Connect();

    //            if (connected)
    //            {
    //                _isConnected = true;
    //                txtConnectionStatus.Text = "已连接";
    //                txtConnectionStatus.Foreground = Brushes.Green;
    //                btnConnect.IsEnabled = false;
    //                btnDisconnect.IsEnabled = true;
    //                _statusTimer.Start();
    //                AppendLog($"控制器连接成功");
    //            }
    //            else
    //            {
    //                AppendLog($"连接失败");
    //                MessageBox.Show("连接失败，请检查IP地址");
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            AppendLog($"连接异常: {ex.Message}");
    //            MessageBox.Show($"连接异常: {ex.Message}");
    //        }
    //    }

    //    private void btnDisconnect_Click(object sender, RoutedEventArgs e)
    //    {
    //        AppendLog("正在断开连接");
    //        _statusTimer.Stop();
    //        _controller?.DisConnect();
    //        _isConnected = false;
    //        txtConnectionStatus.Text = "未连接";
    //        txtConnectionStatus.Foreground = Brushes.Red;
    //        btnConnect.IsEnabled = true;
    //        btnDisconnect.IsEnabled = false;

    //        foreach (var item in AxisItems)
    //        {
    //            item.Dpos = 0;
    //            item.Mpos = 0;
    //            item.Speed = 0;
    //            item.Status = -1;
    //            item.HomeStatus = 0;
    //        }
    //    }

    //    private void StatusTimer_Tick(object sender, EventArgs e)
    //    {
    //        if (!_isConnected || _controller == null) return;

    //        try
    //        {
    //            int[] idleStatus = new int[4];
    //            float[] dposStatus = new float[4];
    //            float[] mposStatus = new float[4];
    //            int[] axisStatus = new int[4];

    //            var result = _controller.ZAux_Direct_GetAllAxisInfo(4, idleStatus, dposStatus, mposStatus, axisStatus);
    //            if (result.IsSuccess)
    //            {
    //                for (int i = 0; i < AxisItems.Count; i++)
    //                {
    //                    AxisItems[i].Dpos = dposStatus[i];
    //                    AxisItems[i].Mpos = mposStatus[i];
    //                    AxisItems[i].Status = idleStatus[i];

    //                    float speed = 0;
    //                    _controller.ZAux_Direct_GetVpSpeed(i, ref speed);
    //                    AxisItems[i].Speed = speed;

    //                    uint homeStatus = 0;
    //                    _controller.ZAux_Direct_GetHomeStatus(i, ref homeStatus);
    //                    AxisItems[i].HomeStatus = homeStatus;
    //                }

    //                AppendLog($"轴状态更新: Dpos=[{string.Join(",", dposStatus.Select(v => v.ToString("F2")))}], Idle=[{string.Join(",", idleStatus)}]");
    //            }
    //            else
    //            {
    //                AppendLog($"获取轴状态失败: {result.ErrMessage}");
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            AppendLog($"读取状态异常: {ex.Message}");
    //        }
    //    }

    //    private void BtnJogForward_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    //    {
    //        if (_isClosing) return;
    //        Button btn = sender as Button;
    //        if (btn == null) return;

    //        int axis = (int)btn.Tag;
    //        StartJog(axis, 1);
    //        System.Windows.Input.Mouse.Capture(btn);
    //    }

    //    private void BtnJogReverse_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    //    {
    //        if (_isClosing) return;
    //        Button btn = sender as Button;
    //        if (btn == null) return;

    //        int axis = (int)btn.Tag;
    //        StartJog(axis, -1);
    //        System.Windows.Input.Mouse.Capture(btn);
    //    }

    //    private void BtnJog_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    //    {
    //        if (_isClosing) return;
    //        Button btn = sender as Button;
    //        if (btn == null) return;

    //        int axis = (int)btn.Tag;
    //        StopJog(axis);
    //        System.Windows.Input.Mouse.Capture(null);
    //    }

    //    private void StartJog(int axis, int direction)
    //    {
    //        if (_isClosing || !this.IsLoaded || !this.IsVisible) return;
    //        if (!_isConnected || _controller == null)
    //        {
    //            return;
    //        }

    //        lock (_lockObj)
    //        {
    //            if (_joggingFlags[axis]) return;
    //            _joggingFlags[axis] = true;
    //        }

    //        var axisItem = AxisItems[axis];
    //        var result = _controller.ZAux_Direct_Single_Vmove(axis, axisItem.Param, direction);

    //        if (!result.IsSuccess)
    //        {
    //            MessageBox.Show(result.ErrMessage);
    //            lock (_lockObj)
    //            {
    //                _joggingFlags[axis] = false;
    //            }
    //        }
    //    }

    //    private void StopJog(int axis)
    //    {
    //        if (!_isConnected || _controller == null) return;

    //        lock (_lockObj)
    //        {
    //            if (!_joggingFlags[axis]) return;
    //            _joggingFlags[axis] = false;
    //        }

    //        _controller.ZAux_Direct_Single_Cancel(axis, 0);
    //    }

    //    private void BtnStop_Click(object sender, RoutedEventArgs e)
    //    {
    //        Button btn = sender as Button;
    //        if (btn == null) return;

    //        int axis = (int)btn.Tag;
    //        StopJog(axis);
    //    }

    //    private void BtnHome_Click(object sender, RoutedEventArgs e)
    //    {
    //        if (!_isConnected || _controller == null)
    //        {
    //            MessageBox.Show("请先连接控制器");
    //            return;
    //        }

    //        Button btn = sender as Button;
    //        if (btn == null) return;

    //        int axis = (int)btn.Tag;
    //        var result = _controller.ZAux_Direct_Single_Datum(axis, 1);
    //        if (result.IsSuccess)
    //        {
    //            AppendLog($"轴{axis}开始回零");
    //        }
    //        else
    //        {
    //            AppendLog($"轴{axis}回零失败: {result.ErrMessage}");
    //            MessageBox.Show(result.ErrMessage);
    //        }
    //    }

    //    private string GetCurrentPosString()
    //    {
    //        if (AxisItems == null || AxisItems.Count < 4) return "X:0.00 Y:0.00 Z:0.00 R:0.00";
    //        return $"X:{AxisItems[0].Mpos:F2} Y:{AxisItems[1].Mpos:F2} Z:{AxisItems[2].Mpos:F2} R:{AxisItems[3].Mpos:F2}";
    //    }

    //    private void BtnCalPickPos_Click(object sender, RoutedEventArgs e)
    //    {
    //        CalPickPosText = GetCurrentPosString();
    //        AppendLog($"标定取销钉位置: {CalPickPosText}");
    //    }

    //    private void BtnCalDrivePos_Click(object sender, RoutedEventArgs e)
    //    {
    //        CalDrivePosText = GetCurrentPosString();
    //        AppendLog($"标定打销钉位置: {CalDrivePosText}");
    //    }

    //    private void BtnCalRotatePos_Click(object sender, RoutedEventArgs e)
    //    {
    //        CalRotatePosText = GetCurrentPosString();
    //        AppendLog($"标定旋转位置: {CalRotatePosText}");
    //    }

    //    private void BtnCalReturnPos_Click(object sender, RoutedEventArgs e)
    //    {
    //        CalReturnPosText = GetCurrentPosString();
    //        AppendLog($"标定回来位置: {CalReturnPosText}");
    //    }

    //    private void AppendLog(string message)
    //    {
    //        if (txtLog != null)
    //        {
    //            txtLog.Dispatcher.BeginInvoke(new Action(() =>
    //            {
    //                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss.fff}] {message}\n");
    //                txtLog.ScrollToEnd();
    //            }));
    //        }
    //    }

    //    private void BtnSaveParams_Click(object sender, RoutedEventArgs e)
    //    {
    //        try
    //        {
    //            var config = new
    //            {
    //                IpAddress = txtIpAddress.Text,
    //                AxisParams = AxisItems.Select(item => new
    //                {
    //                    AxisNumber = item.AxisNumber,
    //                    Param = new
    //                    {
    //                        item.Param.Units,
    //                        item.Param.Lspeed,
    //                        item.Param.Speed,
    //                        item.Param.Accel,
    //                        item.Param.Decel,
    //                        item.Param.Sramp
    //                    }
    //                }).ToList()
    //            };

    //            string json = System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    //            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
    //            string directory = System.IO.Path.GetDirectoryName(exePath);
    //            string path = System.IO.Path.Combine(directory, "axis_params.json");
    //            System.IO.File.WriteAllText(path, json);

    //            AppendLog($"参数已保存到: {path}");
    //        }
    //        catch (Exception ex)
    //        {
    //            AppendLog($"保存参数失败: {ex.Message}");
    //        }
    //    }

    //    private void LoadParamsOnStartup()
    //    {
    //        try
    //        {
    //            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
    //            string directory = System.IO.Path.GetDirectoryName(exePath);
    //            string path = System.IO.Path.Combine(directory, "axis_params.json");
    //            if (!System.IO.File.Exists(path))
    //            {
    //                return;
    //            }

    //            string json = System.IO.File.ReadAllText(path);
    //            var config = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json);

    //            if (config.TryGetProperty("AxisParams", out var axisParamsElement) && axisParamsElement.ValueKind == System.Text.Json.JsonValueKind.Array)
    //            {
    //                foreach (var axisConfig in axisParamsElement.EnumerateArray())
    //                {
    //                    if (axisConfig.TryGetProperty("AxisNumber", out var axisNumElement))
    //                    {
    //                        int axisNum = axisNumElement.GetInt32();
    //                        if (axisNum >= 0 && axisNum < AxisItems.Count)
    //                        {
    //                            if (axisConfig.TryGetProperty("Param", out var paramElement))
    //                            {
    //                                if (paramElement.TryGetProperty("Units", out var unitsElement))
    //                                    AxisItems[axisNum].Param.Units = (float)unitsElement.GetDouble();
    //                                if (paramElement.TryGetProperty("Lspeed", out var lsElement))
    //                                    AxisItems[axisNum].Param.Lspeed = (float)lsElement.GetDouble();
    //                                if (paramElement.TryGetProperty("Speed", out var speedElement))
    //                                    AxisItems[axisNum].Param.Speed = (float)speedElement.GetDouble();
    //                                if (paramElement.TryGetProperty("Accel", out var accelElement))
    //                                    AxisItems[axisNum].Param.Accel = (float)accelElement.GetDouble();
    //                                if (paramElement.TryGetProperty("Decel", out var decelElement))
    //                                    AxisItems[axisNum].Param.Decel = (float)decelElement.GetDouble();
    //                                if (paramElement.TryGetProperty("Sramp", out var srampElement))
    //                                    AxisItems[axisNum].Param.Sramp = (float)srampElement.GetDouble();
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //            if (config.TryGetProperty("IpAddress", out var ipElement))
    //            {
    //                txtIpAddress.Text = ipElement.GetString();
    //            }
    //        }
    //        catch { }
    //    }

    //    private void BtnLoadParams_Click(object sender, RoutedEventArgs e)
    //    {
    //        try
    //        {
    //            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
    //            string directory = System.IO.Path.GetDirectoryName(exePath);
    //            string path = System.IO.Path.Combine(directory, "axis_params.json");
    //            if (!System.IO.File.Exists(path))
    //            {
    //                AppendLog("参数文件不存在");
    //                return;
    //            }

    //            string json = System.IO.File.ReadAllText(path);
    //            var config = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json);

    //            if (config.TryGetProperty("AxisParams", out var axisParamsElement) && axisParamsElement.ValueKind == System.Text.Json.JsonValueKind.Array)
    //            {
    //                foreach (var axisConfig in axisParamsElement.EnumerateArray())
    //                {
    //                    if (axisConfig.TryGetProperty("AxisNumber", out var axisNumElement))
    //                    {
    //                        int axisNum = axisNumElement.GetInt32();
    //                        if (axisNum >= 0 && axisNum < AxisItems.Count)
    //                        {
    //                            if (axisConfig.TryGetProperty("Param", out var paramElement))
    //                            {
    //                                if (paramElement.TryGetProperty("Units", out var unitsElement))
    //                                    AxisItems[axisNum].Param.Units = (float)unitsElement.GetDouble();
    //                                if (paramElement.TryGetProperty("Lspeed", out var lsElement))
    //                                    AxisItems[axisNum].Param.Lspeed = (float)lsElement.GetDouble();
    //                                if (paramElement.TryGetProperty("Speed", out var speedElement))
    //                                    AxisItems[axisNum].Param.Speed = (float)speedElement.GetDouble();
    //                                if (paramElement.TryGetProperty("Accel", out var accelElement))
    //                                    AxisItems[axisNum].Param.Accel = (float)accelElement.GetDouble();
    //                                if (paramElement.TryGetProperty("Decel", out var decelElement))
    //                                    AxisItems[axisNum].Param.Decel = (float)decelElement.GetDouble();
    //                                if (paramElement.TryGetProperty("Sramp", out var srampElement))
    //                                    AxisItems[axisNum].Param.Sramp = (float)srampElement.GetDouble();
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //            if (config.TryGetProperty("IpAddress", out var ipElement))
    //            {
    //                txtIpAddress.Text = ipElement.GetString();
    //            }

    //            AppendLog("参数加载成功");
    //        }
    //        catch (Exception ex)
    //        {
    //            AppendLog($"加载参数失败: {ex.Message}");
    //        }
    //    }

    //    protected override void OnClosing(CancelEventArgs e)
    //    {
    //        if (_isClosing)
    //        {
    //            base.OnClosing(e);
    //            return;
    //        }
    //        _isClosing = true;
    //        e.Cancel = true;
    //        this.Visibility = Visibility.Hidden;
    //        System.Windows.Input.Mouse.Capture(null);
    //        this.Dispatcher.BeginInvoke(new Action(() =>
    //        {
    //            _statusTimer.Stop();
    //            _controller?.DisConnect();
    //            this.Close();
    //        }), System.Windows.Threading.DispatcherPriority.Render);
    //    }
    //}


}
