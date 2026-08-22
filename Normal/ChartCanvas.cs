using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ToolCollisionCalibration.Normal
{
    /// <summary>
    /// 绑定驱动的自定义 Canvas 图表控件（仅正数第一象限）。
    ///
    /// 设计要点（总结经验 151077 的最佳实践）：
    ///   1) 对外通过 5 个依赖属性 Points/XMin/XMax/YMin/YMax 接收数据，
    ///      属性变化只调用 InvalidateDraw 排队，不在 setter 回调里直接 Children.Clear + 画，
    ///      避免多属性同时变化（如一次 Add 触发 Points+XMax 两个回调）导致画两次 → 刷新风暴。
    ///   2) InvalidateDraw 用 _dirtyQueued + DispatcherPriority.Render 合并同一渲染帧的多次请求，
    ///      同一 UI 周期内再多点变化也只重画 1 次，解决抖动。
    ///   3) RedrawInternal 开头尺寸门禁：ActualWidth/plotW 必须 >0，
    ///      避免首次加载还未进入布局导致 0 尺寸画出怪内容。
    ///   4) 不手动 Measure/Arrange，完全交给 WPF 布局系统（经验教训：手动改父级布局分配会出 UI 错乱）。
    ///
    /// 用法：XAML 里
    ///   <local:ChartCanvas Points="{Binding Points}" XMin="{Binding XMin}" ... />
    /// </summary>
    public class ChartCanvas : Canvas
    {
        // ---- 绘图区边距：留出左侧 Y 刻度文字、下方 X 刻度文字、上方/右侧 轴标题文字
        //
        // 数值经验（11 号 Segoe UI 文本）：
        //   MarginLeft 54 足够装下 5 位整数（"12345"），不被 Y 轴切
        //   MarginBottom 40 足够装下 X 刻度数字 + 一点空白
        //   MarginTop 28 足够装下 Y 轴标题（如 "Y" 或 "位置(mm)"）
        //   MarginRight 60 足够装下 X 轴标题（如 "X" 或 "时间(ms)"）
        private const double MarginLeft   = 54;
        private const double MarginTop    = 28;
        private const double MarginRight  = 60;
        private const double MarginBottom = 40;

        // ============================================================
        //  依赖属性：供 XAML 绑定（ChartCanvas 对 ViewModel 的唯一接口）
        // ============================================================

        /// <summary>
        /// 绑定到 VM 的 ObservableCollection&lt;Point&gt;。
        /// WHY 用 IList&lt;Point&gt; + INotifyCollectionChanged 订阅：
        ///   - 既能接受 ObservableCollection（自动变化通知），也可接受 List（静态），
        ///   - 订阅 CollectionChanged 保证 Add/Clear 后都能 InvalidateDraw。
        /// 注册元数据回调 OnPointsChanged：负责切换集合时解挂旧集合的事件。
        /// </summary>
        public IList<Point> Points
        {
            get => (IList<Point>)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
                nameof(Points),
                typeof(IList<Point>),
                typeof(ChartCanvas),
                new PropertyMetadata(null, OnPointsChanged));

        public double XMin
        {
            get => (double)GetValue(XMinProperty);
            set => SetValue(XMinProperty, value);
        }
        public static readonly DependencyProperty XMinProperty =
            DependencyProperty.Register(nameof(XMin), typeof(double), typeof(ChartCanvas),
                new PropertyMetadata(0d, OnRangeChanged));

        public double XMax
        {
            get => (double)GetValue(XMaxProperty);
            set => SetValue(XMaxProperty, value);
        }
        public static readonly DependencyProperty XMaxProperty =
            DependencyProperty.Register(nameof(XMax), typeof(double), typeof(ChartCanvas),
                new PropertyMetadata(10d, OnRangeChanged));

        public double YMin
        {
            get => (double)GetValue(YMinProperty);
            set => SetValue(YMinProperty, value);
        }
        public static readonly DependencyProperty YMinProperty =
            DependencyProperty.Register(nameof(YMin), typeof(double), typeof(ChartCanvas),
                new PropertyMetadata(0d, OnRangeChanged));

        public double YMax
        {
            get => (double)GetValue(YMaxProperty);
            set => SetValue(YMaxProperty, value);
        }
        public static readonly DependencyProperty YMaxProperty =
            DependencyProperty.Register(nameof(YMax), typeof(double), typeof(ChartCanvas),
                new PropertyMetadata(10d, OnRangeChanged));

        /// <summary>
        /// X 轴标题文字（默认 "X"）。绑定到 VM.XAxisTitle，改变后自动重绘。
        /// 例：可设为 "时间(ms)" / "行程(mm)" 等业务语义文字。
        /// </summary>
        public string XAxisTitle
        {
            get => (string)GetValue(XAxisTitleProperty);
            set => SetValue(XAxisTitleProperty, value);
        }
        public static readonly DependencyProperty XAxisTitleProperty =
            DependencyProperty.Register(nameof(XAxisTitle), typeof(string), typeof(ChartCanvas),
                new PropertyMetadata("X", OnRangeChanged)); // 复用 OnRangeChanged：标题变了也只需重画一次

        /// <summary>Y 轴标题文字（默认 "Y"），同 XAxisTitle。</summary>
        public string YAxisTitle
        {
            get => (string)GetValue(YAxisTitleProperty);
            set => SetValue(YAxisTitleProperty, value);
        }
        public static readonly DependencyProperty YAxisTitleProperty =
            DependencyProperty.Register(nameof(YAxisTitle), typeof(string), typeof(ChartCanvas),
                new PropertyMetadata("Y", OnRangeChanged));

        // ---- 集合变更事件钩子：避免多次 GC 事件委托或被解绑
        //
        // 当 {Binding Points} 重新赋值一个新集合实例时，旧集合的事件必须 -=，否则
        // ChartCanvas 仍被旧集合引用，GC 不回收，会出现"集合已清空但仍收到旧集合的事件"。
        private INotifyCollectionChanged _pointsNotifier;

        /// <summary>
        /// Points 依赖属性变化回调：
        ///   - 旧实例：若实现 INotifyCollectionChanged，解绑 -= 事件
        ///   - 新实例：若实现 INotifyCollectionChanged，绑定 += 事件
        ///   - 之后一次 InvalidateDraw 合并刷新
        /// </summary>
        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (ChartCanvas)d;
            if (chart._pointsNotifier != null)
            {
                chart._pointsNotifier.CollectionChanged -= chart.OnPointsCollectionChanged;
                chart._pointsNotifier = null;
            }
            if (e.NewValue is INotifyCollectionChanged ncc)
            {
                chart._pointsNotifier = ncc;
                ncc.CollectionChanged += chart.OnPointsCollectionChanged;
            }
            chart.InvalidateDraw();
        }

        /// <summary>
        /// 四个轴范围属性共用一个元数据回调 → 单入口统一触发重绘。
        /// </summary>
        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((ChartCanvas)d).InvalidateDraw();

        /// <summary>
        /// 集合本身的 Add / Remove / Clear / Reset 会触发此事件，
        /// 同样统一 InvalidateDraw；单帧内 N 次 Add 只重绘 1 次。
        /// </summary>
        private void OnPointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            => InvalidateDraw();

        // ============================================================
        //  生命周期钩子：控件可见 + 尺寸稳定后首次画；窗口 Resize 重画
        // ============================================================

        public ChartCanvas()
        {
            // Loaded：第一次进入布局并赋予真实 ActualWidth/ActualHeight
            Loaded     += (s, e) => InvalidateDraw();
            // SizeChanged：用户拖动窗口大小、布局容器变化，需要重新映射坐标
            SizeChanged += (s, e) => InvalidateDraw();
            // ClipToBounds=true：边缘的点/线不画到外边（避免刻度文字与工具栏重叠）
            ClipToBounds = true;
        }

        // ============================================================
        //  刷新调度：合并刷新风暴
        // ============================================================

        /// <summary>
        /// true 表示本渲染帧已把重绘排进 Dispatcher 队列，
        /// 后续再来的属性变化只需等这次 RedrawInternal 一次搞定。
        /// </summary>
        private bool _dirtyQueued;

        /// <summary>
        /// 唯一刷新入口：任何数据/尺寸变化都只调用这里。
        /// WHY DispatcherPriority.Render：
        ///   - 比正常 DispatcherPriority.Normal 低，
        ///     保证所有属性绑定/布局计算完成后再画，
        ///     不会在布局前就用 0 尺寸画出半成品。
        /// WHY 不用 InvalidateVisual()：
        ///   - InvalidateVisual 会走 OnRender，我们用的是 Children 操作，
        ///     直接手动管理 Children 更灵活（TextBlock/Line/Ellipse 放 Children 比 DrawingContext 写代码方便）。
        /// </summary>
        private void InvalidateDraw()
        {
            if (_dirtyQueued) return;
            _dirtyQueued = true;
            Dispatcher.BeginInvoke(new Action(RedrawInternal),
                System.Windows.Threading.DispatcherPriority.Render);
        }

        // ============================================================
        //  核心绘制：只画一次
        // ============================================================

        /// <summary>
        /// 真正的绘制函数：
        ///   1) 尺寸门禁（经验 151077：硬条件判 0）
        ///   2) 清空子元素（Children 操作模式，非 OnRender DrawingContext）
        ///   3) 计算坐标映射 mapX / mapY（数据坐标 → 画布像素）
        ///   4) 网格线 + 刻度
        ///   5) 坐标轴（原点若在视图内则画 0 标签 + X/Y 轴 + 箭头 + X/Y 标签）
        ///   6) 连线
        ///   7) 点 + 最后一个点的坐标标签
        /// </summary>
        private void RedrawInternal()
        {
            _dirtyQueued = false;

            // 每次重绘先清空 Children。因为 Children 是追加式的，不清空会累积旧线/点。
            Children.Clear();

            // ---- 尺寸门禁 ----
            double w = ActualWidth, h = ActualHeight;
            if (w <= 0 || h <= 0) return; // 尚未布局 → 等下一次 SizeChanged/Loaded 即可

            double plotW = w - MarginLeft - MarginRight;
            double plotH = h - MarginTop  - MarginBottom;
            if (plotW <= 0 || plotH <= 0) return; // 边距太大导致绘图区无面积，跳过

            // 范围防 0/负，防止除 0 或线性映射崩溃
            double xRange = XMax - XMin; if (xRange <= 0) xRange = 1;
            double yRange = YMax - YMin; if (yRange <= 0) yRange = 1;

            // ---- 坐标映射 ----
            // X 轴：经典线性插值，MarginLeft 作为绘图区原点 X
            // Y 轴：WPF Canvas 原点在左上，而数学坐标系原点在左下 → plotH - (...) 反转
            Func<double, double> mapX = x => MarginLeft + (x - XMin) / xRange * plotW;
            Func<double, double> mapY = y => MarginTop  + plotH - (y - YMin) / yRange * plotH;

            // ---- 刻度步长 ----
            // X 轴按 10 段、Y 按 8 段（经验：多数图表 8~12 段密度合适，不挤也不空）
            double xStep = NiceStep(xRange / 10);
            double yStep = NiceStep(yRange / 8);
            // 整数步长不带小数（用户偏好"整数类型不显示小数点"）；小数步长保留必要精度（最多 3 位）
            string xFmt = xStep >= 1 ? "0" : "0.###";
            string yFmt = yStep >= 1 ? "0" : "0.###";

            // ---- 笔刷（直接构造而非资源，保持项目无 ResourceDictionary 也能跑）
            // 颜色选择：
            //   gridBrush  浅灰 #E6E6E6 → 不抢数据注意力
            //   axisBrush  深灰 #333     → 坐标轴明显
            //   labelBrush 中灰 #555     → 刻度可读但不抢
            //   lineBrush  #1E88E5       → Material Blue 600，连线清爽
            //   pointBrush #E53935       → Material Red 600，红点突出
            var gridBrush  = new SolidColorBrush(Color.FromRgb(0xE6, 0xE6, 0xE6));
            var axisBrush  = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));
            var labelBrush = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x55));
            var lineBrush  = new SolidColorBrush(Color.FromRgb(0x1E, 0x88, 0xE5));

            // ---- 竖直网格线 + X 刻度标签 ----
            // Math.Ceiling(XMin/xStep)*xStep 找到 >= XMin 的最小 xStep 倍数刻度起点
            for (double xv = Math.Ceiling(XMin / xStep) * xStep; xv <= XMax + 1e-9; xv += xStep)
            {
                double cx = mapX(xv);
                // 只在绘图区范围+0.5容差内画，避免贴着边的线看不到也浪费
                if (cx < MarginLeft - 0.5 || cx > MarginLeft + plotW + 0.5) continue;

                Children.Add(new Line
                {
                    X1 = cx, Y1 = MarginTop, X2 = cx, Y2 = MarginTop + plotH,
                    Stroke = gridBrush, StrokeThickness = 1
                });

                // 跳过 0 刻度：由下方统一的 "原点 0" 标签显示（避免左下角重复显示 0）
                if (Math.Abs(xv) < 1e-9) continue;
                var tb = MakeLabel(xv.ToString(xFmt), labelBrush);
                SetLeft(tb, cx - tb.DesiredSize.Width / 2);  // 居中对齐刻度
                SetTop(tb, MarginTop + plotH + 6);            // X 轴下方 6px
                Children.Add(tb);
            }

            // ---- 水平网格线 + Y 刻度标签 ----
            for (double yv = Math.Ceiling(YMin / yStep) * yStep; yv <= YMax + 1e-9; yv += yStep)
            {
                double cy = mapY(yv);
                if (cy < MarginTop - 0.5 || cy > MarginTop + plotH + 0.5) continue;
                Children.Add(new Line
                {
                    X1 = MarginLeft, Y1 = cy, X2 = MarginLeft + plotW, Y2 = cy,
                    Stroke = gridBrush, StrokeThickness = 1
                });

                if (Math.Abs(yv) < 1e-9) continue; // 同理 0 由原点标签统一
                var tb = MakeLabel(yv.ToString(yFmt), labelBrush);
                SetLeft(tb, MarginLeft - tb.DesiredSize.Width - 6);  // 右对齐 Y 轴
                SetTop(tb, cy - tb.DesiredSize.Height / 2);          // 垂直居中于刻度
                Children.Add(tb);
            }

            // ---- 坐标轴 X/Y（带箭头） ----
            // 仅当 "0"（原点）落在 [min, max] 范围内才画实际坐标轴
            // 在正数第一象限中，原点就在左下角
            double originX = double.NaN, originY = double.NaN;
            if (XMin <= 0 && XMax >= 0)
            {
                double cy = mapY(0);
                originY = cy;
                Children.Add(new Line
                {
                    X1 = MarginLeft, Y1 = cy, X2 = MarginLeft + plotW, Y2 = cy,
                    Stroke = axisBrush, StrokeThickness = 1.5
                });
                // 箭头：三条线（轴本身 + 三角形填充）更清晰
                Children.Add(new Polygon
                {
                    Points = new PointCollection
                    {
                        new Point(MarginLeft + plotW, cy),
                        new Point(MarginLeft + plotW - 9, cy - 5),
                        new Point(MarginLeft + plotW - 9, cy + 5)
                    },
                    Fill = axisBrush
                });
                // X 轴标题（粗体）：放在 X 刻度标签同一行，居中对齐到 X 轴右端
                var xTitle = XAxisTitle ?? string.Empty;
                if (!string.IsNullOrEmpty(xTitle))
                {
                    var xLab = MakeLabel(xTitle, axisBrush); xLab.FontWeight = FontWeights.Bold;
                    // SetLeft: X 轴右端偏右 10px；SetTop: 与 X 刻度标签对齐（在 X 轴下方 6px 处）
                    SetLeft(xLab, MarginLeft + plotW + 10);
                    SetTop(xLab, MarginTop + plotH + 6);
                    Children.Add(xLab);
                }
            }
            if (YMin <= 0 && YMax >= 0)
            {
                double cx = mapX(0);
                originX = cx;
                Children.Add(new Line
                {
                    X1 = cx, Y1 = MarginTop, X2 = cx, Y2 = MarginTop + plotH,
                    Stroke = axisBrush, StrokeThickness = 1.5
                });
                Children.Add(new Polygon
                {
                    Points = new PointCollection
                    {
                        new Point(cx, MarginTop),
                        new Point(cx - 5, MarginTop + 9),
                        new Point(cx + 5, MarginTop + 9)
                    },
                    Fill = axisBrush
                });
                // Y 轴标题（粗体）：放在 Y 刻度标签同一行，靠右对齐到 Y 轴
                var yTitle = YAxisTitle ?? string.Empty;
                if (!string.IsNullOrEmpty(yTitle))
                {
                    var yLab = MakeLabel(yTitle, axisBrush); yLab.FontWeight = FontWeights.Bold;
                    // SetLeft: Y 轴右侧偏右 6px；SetTop: 与 Y 刻度标签对齐
                    SetLeft(yLab, cx + 6);
                    SetTop(yLab, MarginTop - yLab.DesiredSize.Height - 4);
                    Children.Add(yLab);
                }
            }

            // ---- 原点 "0" 标记 ----
            // 在坐标轴与左下角外侧之间的位置显示，不与 X/Y 刻度重叠
            if (!double.IsNaN(originX) && !double.IsNaN(originY))
            {
                var o = MakeLabel("0", labelBrush);
                SetLeft(o, originX - o.DesiredSize.Width - 4);
                SetTop(o, originY + 4);
                Children.Add(o);
            }

            // ---- 连接线（数据顺序即连线顺序） ----
            var pts = Points;
            if (pts != null && pts.Count > 1)
            {
                for (int i = 1; i < pts.Count; i++)
                {
                    var p1 = pts[i - 1]; var p2 = pts[i];
                    Children.Add(new Line
                    {
                        X1 = mapX(p1.X), Y1 = mapY(p1.Y),
                        X2 = mapX(p2.X), Y2 = mapY(p2.Y),
                        Stroke = lineBrush, StrokeThickness = 2
                    });
                }
            }

            // ---- 不再绘制单独的点，只保留连线 ----
        }

        // ============================================================
        //  辅助工具
        // ============================================================

        /// <summary>
        /// 创建一个"已测量"的 TextBlock：在 SetLeft/SetTop 之前用 DesiredSize 计算居中/右对齐所需。
        /// WHY Measure(new Size(∞,∞))：不加入布局树前 TextBlock 无 DesiredSize，
        /// 手动 Measure 一次就能拿到真实渲染尺寸。
        /// </summary>
        private static TextBlock MakeLabel(string text, Brush brush)
        {
            var tb = new TextBlock
            {
                Text = text, Foreground = brush, FontSize = 11,
                FontFamily = new FontFamily("Segoe UI")
            };
            // 用无穷大测出来的就是"单行完整占用"的尺寸
            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return tb;
        }

        /// <summary>
        /// 刻度步长"好读化"：rough / 10^k → norm → 1/2/5 选最近 → nice * 10^k。
        /// 与 ChartViewModel 中的同名方法一致（但一个用于轴扩展、一个用于视图刻度步长），
        /// 故意保留两份而非静态共享：视图端的版本是"绘图算法"，VM 端是"数据逻辑"，
        /// 两者解耦，之后任何一方改算法都不影响对方。
        /// </summary>
        private static double NiceStep(double rough)
        {
            if (rough <= 0) return 1;
            double pow  = Math.Pow(10, Math.Floor(Math.Log10(rough)));
            double norm = rough / pow;
            double nice;
            if (norm < 1.5) nice = 1;
            else if (norm < 3) nice = 2;
            else if (norm < 7) nice = 5;
            else nice = 10;
            return nice * pow;
        }
    }
}
