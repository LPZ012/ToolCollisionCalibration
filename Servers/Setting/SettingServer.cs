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
using WPFLibrary.Torque;
using WPFLibrary.AngleDevice;
using WPFLibrary.欧艾迪;
namespace ToolCollisionCalibration.Servers.Setting
{
    /// <summary>
    /// 加载本地配置
    /// </summary>
    public class SettingServer : ISettingServer
    {
        public SettingServer()
        {
            LoadSettingAsync();
            InitDevices();
        }
        public SettingModel settingModel { get; set; } = new SettingModel();
        
        public IScanner<byte[]> Scanner { get; set; }
        public ITorqueDevice<double> TorqueDevice { get; set; }
        public MotionCard motionCard { get; set; }
        public IAngleDevice<double> AngleDevice { get; set; }

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
            settingModel.localparams.ScannerModel ??= new SerialPortModel("扫码枪");
            settingModel.localparams.TorqueModel ??= new SerialPortModel("扭矩仪");
            settingModel.localparams.AngleModel ??= new SerialPortModel("角度仪");
            settingModel.localparams.AxisParamModels ??= new List<AxisParamModel>
            {
                new AxisParamModel(),
                new AxisParamModel(),
                new AxisParamModel(),
                new AxisParamModel()
            };
            for (int i = 0; i < 4; i++)
            {
                settingModel.AxisItems.Add(new AxisItem
                {
                    AxisNumber = i,
                    AxisName = $"轴{i}",
                    Dpos = 0,
                    Mpos = 0,
                    Speed = 0,
                    Status = -1,
                    Param = settingModel.localparams.AxisParamModels[i]
                });
            }
            //更新参数屏蔽
            //先从本地加载产品参数
            settingModel.DBParams = JsonHelper.ReadJson<DBParams>(FilePath.DBParamFolder,FilePath.DBParamFile) ?? new DBParams();
            //GetParamsFromDB();
            //更新参数屏蔽
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
            //Scanner = new ScanHome(settingModel.localparams.ScannerModel);
            //AngleDevice = new OID_R2_3806D_15S1S(settingModel.localparams.AngleModel,32768);
            motionCard = new MotionCard(new WPFLibrary.Sockets.TCPIP.TCPIPModel("192.168.0.11",1000,"motionCard"),settingModel.localparams.AxisParamModels);
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
        }

        private void UpdateMainView()
        {

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
