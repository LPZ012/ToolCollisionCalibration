using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ToolCollisionCalibration.Models;
using ToolCollisionCalibration.Servers.Message.ViewBToViewA;
using ToolCollisionCalibration.Servers.Setting;
using WPFLibrary.Logger;
using WPFLibrary.Logger.DataGridLog;
using WPFLibrary.Scanner;
using WPFLibrary.Zmotion;
namespace ToolCollisionCalibration.ViewModels
{
    public class ViewAModel:INotifyPropertyChanged
    {
        public ViewAModel(IEventAggregator eventAggregator, ISettingServer isettingServer, IDataGridLogHelper Log)
        {
            _IsettingServer = isettingServer;
            this.Log = Log;
            eventAggregator.GetEvent<ViewBToViewAServer>().Subscribe(ViewBToViewAEvent);
            Points.CollectionChanged += (s, e) => UpdateStatus();
            UpdateStatus();
            //InitializeTimer();

        }
        ///////////////////////////////设备/////////////////////////////////////////
        IPulseADIOControler PulseADIOControler => _IsettingServer.IPulseADIOControler;
        IScanner<byte[]> Scanner => _IsettingServer.Scanner;
        //////////////////////////////////////////////////////////////////////////
        DBParams dBParams => _IsettingServer.settingModel.DBParams;
        public DeviceValueModel deviceValueModel { get; set; } = new DeviceValueModel();
        
        public SettingModel settingModel => _IsettingServer.settingModel;
        /// <summary>
        /// 运行状态
        /// </summary>
        public bool LeftIsRunning { get; set; } = false;
        public bool RightIsRunning { get; set; } = false;
        /// <summary>
        /// 测试结果
        /// </summary>
        public string LeftTestResult { get; set; }
        public string RightTestResult { get; set; }
        
        public ISettingServer _IsettingServer { get; set; }
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILoggers Log;

        public event PropertyChangedEventHandler PropertyChanged;
        private DispatcherTimer _refreshTimer;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private List<uint> ListOldSinal = new List<uint>(new uint[15]);
        /// <summary>
        /// 输入口的各个点位状态
        /// </summary>
		private uint[] inputStatus = new uint[15];
		public DataBaseModel dataBaseModel { get; set; } = new DataBaseModel();
        private DateTime StartTime;
        public double LeftTestTime { get; set;  }
        public double RightTestTime { get; set; }

        private ViewBToViewAModel viewBToViewA = new ViewBToViewAModel();

        /// <summary>
        /// 初始化定时器
        /// </summary>
        private void InitializeTimer()
		{
			_refreshTimer = new DispatcherTimer();
			_refreshTimer.Interval = TimeSpan.FromMilliseconds(100);
			_refreshTimer.Tick += RefreshStatus;
            _refreshTimer.Start();

        }

        /// <summary>
        /// 定时刷新输入状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshStatus(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < 15; i++)
                {
                    _IsettingServer.IPulseADIOControler.ZAux_Direct_GetIn(i, ref inputStatus[i]);
                }
                //停止
                if (inputStatus[0] == 0)
                {
                    Log.Write("停止被按下。", LogType.提示);
                    cts.Cancel();
                }
                ///判断上升沿启动
                if (inputStatus[5] == 1 && ListOldSinal[5] == 0 && !LeftIsRunning)
                {
                    LeftIsRunning = true;
                    LeftStart();
                }
                if (inputStatus[7] == 1 && ListOldSinal[7] == 0 && !RightIsRunning)
                {
                    RightIsRunning = true;
                }
                for (int i = 0; i < 15; i++)
                {
                    ListOldSinal[i] = inputStatus[i];
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString(), LogType.错误);
            }
        }


        private void ViewBToViewAEvent(ViewBToViewAModel viewBToViewAModel)
        {
        }


        /// <summary>
        /// 测试前所有初始化
        /// </summary>
        /// <returns></returns>
        private async Task<bool> LeftStartInit()
        {
            LeftTestResult = "运行中";
            cts = new CancellationTokenSource();
            dataBaseModel = new DataBaseModel();
            //OK灯复位
            PulseADIOControler.ZAux_Direct_SetOp(2, 0);
            //NG灯复位
            PulseADIOControler.ZAux_Direct_SetOp(4, 0);
            //测试灯运行
            PulseADIOControler.ZAux_Direct_SetOp(3, 1);
            return true;
        }
        /// <summary>
        /// 测试结束初始化
        /// </summary>
        private void LeftEndInits()
        {
            //打开排水阀
            PulseADIOControler.ZAux_Direct_SetOp(1, 0);
            PulseADIOControler.ZAux_Direct_SetOp(3, 0);
            PulseADIOControler.ZAux_Direct_SetOp(12, 0);
            LeftIsRunning = false;
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(XMin) || propertyName == nameof(XMax) ||
                    propertyName == nameof(YMin) || propertyName == nameof(YMax))
            {
                UpdateStatus();
            }
        }

        private void LeftStart()
        {
            Task.Run(async () =>
            {
                try
                {
                }
                catch (OperationCanceledException)
                {
                    LeftTestResult = "NG";
                    Log.Write("人工强制停止运行!",LogType.提示);
                }
                catch (Exception ex)
                {
                    LeftTestResult = "NG";
                    Log.Write(ex.ToString(), LogType.错误);
                }
                finally
                {
                    LeftEndInits();
                }
            });

        }












        /// <summary>
        /// 可观察点集。用 ObservableCollection 而不是 List 的原因：
        ///   - Add / Remove / Clear 时自动抛出 CollectionChanged，
        ///     视图端的 ChartCanvas 通过依赖属性回调即可感知，自动重绘。
        ///   - 如用普通 List 必须手动 Raise，易漏触发刷新。
        /// 注意：这里不暴露为 private setter（set 私有，仅在构造初始化），避免误换实例导致订阅链断裂。
        /// </summary>
        public ObservableCollection<Point> Points { get; } = new ObservableCollection<Point>();

        // ---- 四个轴范围属性（对外只读：除构造和 ClearAll，不会减少/减少只会在 ClearAll 时重置）

        /// <summary>X 轴范围最小值（当前固定为 0）。OneWay 绑定到 ChartCanvas.XMin。</summary>
        public double XMin { get; private set; } = 0;

        /// <summary>
        /// X 轴范围最大值，超过当前值会被 NiceCeiling 扩展到易读刻度（1/2/5 系列），
        /// 只增不减：避免点缩小范围引起视图端反复重排（经验 151077：抖动问题的根源之一是缩小边界）。
        /// </summary>
        public double XMax { get; private set; } = 10;

        /// <summary>Y 轴范围最小值（当前固定为 0）。</summary>
        public double YMin { get; private set; } = 0;

        /// <summary>Y 轴范围最大值，同 XMax 的扩展逻辑。</summary>
        public double YMax { get; private set; } = 10;
        public string XInput { get; set; }

        /// <summary>Y 输入框文本。</summary>
        public string YInput { get; set; }

        /// <summary>
        /// 状态栏显示内容（点数 / X范围 / Y范围）。
        /// 点数变化或范围变化都会触发 UpdateStatus，避免散落各处更新字符串。
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// "添加点"的业务入口：从绑定的 XInput/YInput 解析出非负数值并追加到 Points。
        /// WHY 这里不做 CanExecute 拦截（如输入非法就直接 return）：
        ///   - 避免按钮灰掉让用户"为什么不能按"困扰；
        ///   - 非法输入（如空、负数）直接忽略是最直观的反馈。
        /// 如果之后想让按钮在非法输入时禁用，可把 AddCommand 构造时带 canExecute。
        /// </summary>
        private void AddPointFromInput(double x, double y)
        {
            if (x < 0) return;
            if (y < 0) return;

            // ObservableCollection.Add 会立即抛出 CollectionChanged，ChartCanvas 的
            // 依赖属性回调会收到通知并 InvalidateDraw，完成自动绘图
            Points.Add(new Point(x, y));

            // ---- 轴范围自动扩展：只扩不缩
            //   - 上界 + 余量（15% / 至少 1），然后取 NiceCeiling 使刻度好读
            //   - 下界不做：第一象限永远是正数，不小于 0
            if (x > XMax) XMax = NiceCeiling(x + Math.Max(1, Math.Abs(x) * 0.15));
            if (y > YMax) YMax = NiceCeiling(y + Math.Max(1, Math.Abs(y) * 0.15));
        }

        /// <summary>
        /// 清空点集并还原到初始 [0, 10] 范围，并触发 INPC 通知视图刷新。
        /// WHY 必须走 Set() 赋值：直接写字段不会发 PropertyChanged，ChartCanvas
        /// 的绑定不会知道范围变了。
        /// </summary>
        private void ClearAll()
        {
            // ObservableCollection.Clear() 抛出 CollectionChanged 即够视图清空所有点
            Points.Clear();
            XMin = 0; XMax = 10;
            YMin = 0; YMax = 10;
            // 点数归零后重新拼 Status
            UpdateStatus();
        }

        /// <summary>
        /// 更新状态栏文本："点数: N    X: [min ~ max]    Y: [min ~ max]"。
        /// 小数刻度不强行显示小数，整数刻度显示整数（符合用户偏好：整数不显示小数点）。
        /// xFmt / yFmt 根据 NiceStep 选择 "0" / "0.###"，整数步长不显示小数。
        /// </summary>
        private void UpdateStatus()
        {
            string xFmt = NiceStep((XMax - XMin) / 10) >= 1 ? "0" : "0.###";
            string yFmt = NiceStep((YMax - YMin) / 8) >= 1 ? "0" : "0.###";
            Status =
                $"点数: {Points.Count}    " +
                $"X: [{XMin.ToString(xFmt)} ~ {XMax.ToString(xFmt)}]    " +
                $"Y: [{YMin.ToString(yFmt)} ~ {YMax.ToString(yFmt)}]";
        }

        // ---- 刻度/范围"好读"算法：把任意数值对齐到 1 / 2 / 5 乘 10^n
        //
        // WHY 自己实现而不用 Math.Ceiling/Round：
        //   - 23 直接 Round/Ceil 到"好看"的上界结果不一定是 10 / 20 / 50，易出现 23→24 的奇怪刻度
        //   - 1/2/5 系列是人眼最习惯的好读刻度，几乎所有图表（Matplotlib / Excel 默认都选它）

        /// <summary>
        /// 根据"期望大概分成约 10 段"算出的粗略步长，对齐到 1/2/5 系列步长。
        /// 例：rough=1.7 → pow=1, norm=1.7 → nice=2 → 步长 2。
        /// 例：rough=7   → pow=1, norm=7   → nice=10 → 步长 10。
        /// </summary>
        private static double NiceStep(double rough)
        {
            if (rough <= 0) return 1; // 防御：0/负数直接返回 1，保证后续循环不被除零或死循环
            double pow = Math.Pow(10, Math.Floor(Math.Log10(rough)));
            double norm = rough / pow;
            double nice;
            if (norm < 1.5) nice = 1;
            else if (norm < 3) nice = 2;
            else if (norm < 7) nice = 5;
            else nice = 10;
            return nice * pow;
        }

        /// <summary>
        /// 把给定值向上取到最近的"好读"刻度上界。
        /// 例：v=23 → pow=10, norm=2.3 → nice=5 → 5*10 = 50。
        /// v=8 → pow=1, norm=8 → nice=10 → 10。
        /// v<=0 时返回 1：正数坐标系，不会出现负上界。
        /// </summary>
        private static double NiceCeiling(double v)
        {
            if (v <= 0) return 1;
            double pow = Math.Pow(10, Math.Floor(Math.Log10(v)));
            double norm = v / pow;
            double nice;
            if (norm <= 1) nice = 1;
            else if (norm <= 2) nice = 2;
            else if (norm <= 5) nice = 5;
            else nice = 10;
            return nice * pow;
        }


    }
}
