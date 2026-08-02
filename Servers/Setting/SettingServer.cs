using ToolCollisionCalibration.Devices;
using ToolCollisionCalibration.Models;
using ToolCollisionCalibration.Normal;
using System.Data;
using System.Windows;
using WPFLibrary.ComSerialPort;
using WPFLibrary.Json;
using WPFLibrary.Scanner;
using WPFLibrary.Scanner.COMScanner.ScanHome;
using WPFLibrary.Zmotion;
namespace ToolCollisionCalibration.Servers.Setting
{
    /// <summary>
    /// 加载本地配置
    /// </summary>
    public class SettingServer : ISettingServer
    {
        public SettingServer()
        {
            //LoadSettingAsync();
            //InitDevices();
        }
        public SettingModel settingModel { get; set; } = new SettingModel();
        
        public IScanner<byte[]> Scanner { get; set; }
        public IScanner<byte[]> RightScanner { get; set; }
        public IPulseADIOControler IOControler { get; set; }
        public AbsSerialPort LeftES3XXserialPort { get; set; }
        public AbsSerialPort RightES3XXserialPort { get; set; }
        public IPulseADIOControler IPulseADIOControler { get; set; }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        public bool LoadSettingAsync()
        {
            //加载串口等本地参数
            settingModel.localparams = JsonHelper.ReadJson<LocalParams>(FilePath.ParameterFolder, FilePath.ParameterJsonFileName) ?? new LocalParams();
            settingModel.localparams.DebugEnable = false; //正常模式
            settingModel.localparams.ScanEnable = false;//扫码启用
            settingModel.localparams.LeftScannerModel ??= new SerialPortModel("左扫码");
            settingModel.localparams.RightScannerModel ??= new SerialPortModel("右扫码");
            settingModel.localparams.LeftES3XXModel ??= new SerialPortModel("左咖啡");
            settingModel.localparams.RightES3XXModel ??= new SerialPortModel("右咖啡");
            settingModel.localparams.TemperatureModel ??= new SerialPortModel("温度表");
            settingModel.localparams.AddWaterWeightModel ??= new SerialPortModel("称重表");
            
            //更新参数屏蔽
            //先从本地加载产品参数
            settingModel.DBParams = JsonHelper.ReadJson<DBParams>(FilePath.DBParamFolder,FilePath.DBParamFile) ?? new DBParams();
            //GetParamsFromDB();
            //更新参数屏蔽
            if (settingModel.DBParams.ListLocalModels == null)
            {
                settingModel.DBParams.ListLocalModels = new List<LocalModel>
                {
                    new LocalModel(1, "水箱自动称重确认","1","1","WaterTankAutoWeighing"),
                    new LocalModel(2, "扫码并校验","1","1","ScanCodeVerify"),
                    new LocalModel(3, "电压范围","","","VoltageRange"),
                    new LocalModel(4, "进入测试模式","1","1","EnterDebugMode"),
                    new LocalModel(5, "UI版本检查","","","UIVersion"),
                    new LocalModel(6, "电源板版本检查","","","PowerVersion"),
                    new LocalModel(7, "水箱Hall开关检查","1","1","CheckWaterTankHallSwitch"),
                    new LocalModel(8, "豆仓微动开关检查","1","1","CheckBeanSwitch"),
                    new LocalModel(9, "称重微动开关检查","1","1","CheckWeightSwitch"),
                    new LocalModel(10, "手动阀微动开关检查","1","1","CheckHandValveSwitch"),
                    new LocalModel(11, "打奶棒微动开关检查","1","1","CheckMilkCandySwitch"),
                    new LocalModel(12, "电磁阀A开启","1","1","OpenAsolenoid"),
                    new LocalModel(13, "关闭电磁阀A工作","0","0","CloseAsolenoid"),
                    new LocalModel(14, "电磁阀B开启","1","1","OpenBsolenoid"),
                    new LocalModel(15, "关闭电磁阀B工作","0","0","CloseBsolenoid"),
                    new LocalModel(16, "电磁阀C开启","1","1","OpenCsolenoid"),
                    new LocalModel(17, "关闭电磁阀C工作","0","0","CloseCsolenoid"),
                    new LocalModel(18, "启动磨豆组件电机(功率)","55","120","StartGrinderMotorPower"),
                    new LocalModel(19, "关闭磨豆组件电机","0","0","StopGrinderMotor"),
                    new LocalModel(20, "高压煮水","1","1","BoilerWaterUnderHighPressure"),
                    new LocalModel(21, "锅炉NTC温度","95","120","BoilerNTCTemp"),
                    new LocalModel(22, "总进水口流量计","350","550","BoilerTotalInletFlowMeter"),
                    new LocalModel(23, "高压煮水锅炉功率","1518","1716","BoilerPower"),
                    new LocalModel(24, "高压煮水出水量","50","200","BoilerOutWaterWeight"),
                    new LocalModel(25, "高压煮水结束重量","80","220","WeightAfterBoilingWater"),
                    new LocalModel(26, "高压煮水进水量与出水量比对","0","1.5","BoilerComparisonOfInOutWaterWeight"),
                    new LocalModel(27, "热水","1","1","HotWater"),
                    new LocalModel(28, "锅炉NTC温度","","","HotWaterBoilerNTCTemp"),
                    new LocalModel(29, "管路温度判定","1","1","HotWaterPipeTempDeter"),
                    new LocalModel(30, "总进水口流量计","","","HotWaterBoilerTotalInletFlowMeter"),
                    new LocalModel(31, "热水煮水锅炉功率","","","HotWaterBoilerPower"),
                    new LocalModel(32, "热水出水量","","","HotWaterOutWaterWeight"),
                    new LocalModel(33, "热水结束称重","","","HotWaterWeightAfterBoilingWater"),
                    new LocalModel(34, "热水进水量与出水量比对","","","HotWaterComparisonOfInOutWaterWeight"),
                    new LocalModel(35, "UI面板所有灯点亮","1","1","UIPanelLightOn"),
                    new LocalModel(36, "关闭测试模式","0","0","TurnOffDebugMode")
                };
            }
            ResetParams(settingModel.DBParams);
            return true;
        }
        /// <summary>
        /// 写入配置
        /// </summary>
        /// <returns></returns>
        public bool WriteSetting()
        {
            try
            {
                if (string.IsNullOrEmpty(settingModel.DBParams.ProductModel)) 
                {
                    MessageBox.Show("型号不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                bool result = JsonHelper.WriteJson<LocalParams>(FilePath.ParameterFolder, FilePath.ParameterJsonFileName, settingModel.localparams);
                string modeljsonfilmname = FilePath.DBParamFolder + $@"\{settingModel.DBParams.ProductModel}.json";
                result &= JsonHelper.WriteJson<DBParams>(FilePath.DBParamFolder, modeljsonfilmname, settingModel.DBParams);
                if(!result)
                {
                    MessageBox.Show("写入本地配置文件失败","错误",MessageBoxButton.OK,MessageBoxImage.Error);
                }
                UpdateMainView();
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            
        }
        /// <summary>
        /// 初始化设备
        /// </summary>
        public void InitDevices()
        {
            Scanner = new ScanHome(settingModel.localparams.LeftScannerModel);
            RightScanner = new ScanHome(settingModel.localparams.RightScannerModel);
        }

        public bool GetParamsFromDB()
        {
            //try
            //{
            //    DBParams dBParams = _SqlClient.GetParameters(settingModel.localparams.ProdLineNo);
            //    if (dBParams == null)
            //    {
            //        MessageBox.Show("从数据库获取参数失败。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            //        return false;
            //    }
            //    ResetParams(dBParams);
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //   MessageBox.Show($"从数据库获取参数失败，异常信息：{ex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return false;
            //}
            return true;

        }
        public void ResetParams(DBParams dBParams)
        {
            settingModel.DBParams = dBParams;
            //settingModel.DefaultItemsModels.Clear();
            List<ItemsModel> itemsModels = new List<ItemsModel>();
            foreach (var item in settingModel.DBParams.ListLocalModels)
            {
                itemsModels.Add(new ItemsModel(item));
            }
            settingModel.DefaultItemsModels = itemsModels;
            UpdateMainView();
        }

        private void UpdateMainView()
        {
            settingModel.LeftES3XXItemsModels = JsonHelper.TransForm<List<ItemsModel>>(settingModel.DefaultItemsModels);
            settingModel.RightES3XXItemsModels = JsonHelper.TransForm<List<ItemsModel>>(settingModel.DefaultItemsModels);
            settingModel.LeftSelectItemModel = settingModel.LeftES3XXItemsModels.Where(x => x.localModel.IsFlag == true).OrderBy(p => p.localModel.ID).ToList();
            settingModel.RightSelectItemModel = settingModel.RightES3XXItemsModels.Where(x => x.localModel.IsFlag == true).OrderBy(p => p.localModel.ID).ToList();

        }

        public void UpDateParamsToDB()
        {
            //bool result = _SqlClient.UpDateParameters(settingModel.DBParams, settingModel.localparams.ProdLineNo);
            //if (result)
            //{
            //    MessageBox.Show("下发参数成功", "消息", MessageBoxButton.OK, MessageBoxImage.Information);

            //}
            //else
            //{
            //    MessageBox.Show("下发参数失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
    }
}
