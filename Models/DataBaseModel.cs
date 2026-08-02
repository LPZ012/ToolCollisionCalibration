using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ToolCollisionCalibration.Models
{
    public class DataBaseModel : INotifyPropertyChanged
    {
        // --- 基础信息 ---
        [Name("订单号")]
        public string OrderNum { get; set; }

        [Name("工位")]
        public string WorkStation { get; set; }

        [Name("产品型号")]
        public string ProductModel { get; set; }

        [Name("条码编号")]
        public string BarCodeNumber { get; set; }

        [Name("SN码")]
        public string SN_Code { get; set; }

        [Name("生产线别")]
        public string ProdLineNo { get; set; }

        [Name("上传时间")]
        public string UploadTime { get; set; } = DateTime.Now.ToString("HH:mm:ss");

        [Name("测试结果")]
        public bool TestResult { get; set; } 

        [Name("报错步骤")]
        public string ErrorReportingStep { get; set; }

        [Name("报错信息")]
        public string ErrorReportingInformation { get; set; }

        [Name("自动水箱称重确认")]
        public string AutomaticWaterTankWeighingConfirmation { get; set; }

        [Name("扫码进入测试模式")]
        public string ScanCodeToEnterTestMode { get; set; }

        [Name("UI版本检查")]
        public string UIVersionCheck { get; set; }

        [Name("电源板版本检查")]
        public string PowerBoardVersionCheck { get; set; }

        [Name("水箱霍尔开关检查")]
        public string HallSwitchInspectionOfWaterTank { get; set; }

        [Name("豆仓微动开关检查")]
        public string InspectionOfMicroswitchInBeanBin { get; set; }

        [Name("称重开关检查")]
        public string WeightSwitchCheck { get; set; }

        [Name("手动阀微动开关检查")]
        public string ManualValveMicroswitchCheck { get; set; }

        [Name("奶棒微动开关检查")]
        public string CheckTheMicroSwitchOfTheMilkingBar { get; set; }

        [Name("电磁阀A开启")]
        public string SolenoidValveAIsOpen { get; set; }

        [Name("电磁阀A关闭")]
        public string SolenoidValveAIsClosed { get; set; }

        [Name("电磁阀B开启")]
        public string SolenoidValveBIsOpen { get; set; }

        [Name("电磁阀B关闭")]
        public string SolenoidValveBIsClosed { get; set; }

        [Name("电磁阀C开启")]
        public string SolenoidValveCIsOpen { get; set; }

        [Name("电磁阀C关闭")]
        public string SolenoidValveCIsClosed { get; set; }

        [Name("启动磨豆电机组件")]
        public string StartTheMotorOfTheBeanGrindingAssembly { get; set; }

        [Name("关闭磨豆电机组件")]
        public string TurnOffTheMotorOfBeanGrindingAssembly { get; set; }

        [Name("高压煮水指令")]
        public string HighPressureBoilingWaterInstruction { get; set; }

        [Name("锅炉NTC温度")]
        public string BoilerNTCTemperature { get; set; }

        [Name("主进水流量计")]
        public string MainInletFlowmeter { get; set; }

        [Name("高压锅炉功率")]
        public string HighPressureBoilerPower { get; set; }
        [Name("高压煮水前出水重量")]
        public string InitialHighPressureBoilerWaterOutletFlowValue { get; set; }

        [Name("高压煮水出水重量(煮水后 - 煮水前)")]
        public string HighPressureBoilerWaterOutletFlowValue { get; set; }

        [Name("高压煮水结束重量")]
        public string HighPressureBoilingWaterEndsWeighing { get; set; }
        [Name("高压煮水进水量")]
        public string HighPressureBoilingWaterInletFlowValue { get; set; }

        [Name("高压煮水进水量与出水量比对")]
        public string HighPressureBoilingWaterCompare { get; set; }

        [Name("热水")]
        public string HotWater { get; set; }

        [Name("锅炉NTC温度_热水")]
        public string BoilerNTCTemperature_Hot { get; set; }
        [Name("管口前温度")]
        public string ChannelTemperatureBefore { get; set; }
        [Name("管口后温度")]
        public string ChannelTemperatureAfter { get; set; }

        [Name("通道温度判定")]
        public string ChannelTemperDecide { get; set; }

        [Name("热水主进水流量")]
        public string MainInletFlowmeter_Hot { get; set; }

        [Name("热水锅炉功率")]
        public string HotWaterBoilerPower { get; set; }
        [Name("热水前出水重量")]
        public string InitialHotWaterOutletFlowValue { get; set; }
        [Name("热水出水重量(热水后 - 热水前)")]
        public string HotWaterOutletFlowValue { get; set; }
        [Name("热水进水量")]
        public string HotWaterInletFlowValue { get; set; }

        [Name("热水前重量(也是高压煮水后的重量)")]
        public string HotWaterWeighing_Hot_Before { get; set; }
        [Name("热水后重量")]
        public string HotWaterWeighing_Hot_After { get; set; }

        [Name("热水结束称重")]
        public string HotWaterEndWeighing_Hot { get; set; }

        [Name("热水对比")]
        public string HotWatercompare { get; set; }

        [Name("面板指示灯全亮")]
        public string AllLightsOnTheUIPanelAreLit { get; set; }

        [Name("关闭测试模式")]
        public string TurnOffTestMode { get; set; }

        // --- INotifyPropertyChanged 实现 ---
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
