using ToolCollisionCalibration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolCollisionCalibration.Devices;
using System.Collections.ObjectModel;

namespace ToolCollisionCalibration.Servers.Setting
{
    public interface ISettingServer:IDevice
    {

        SettingModel settingModel { get; set; }

        bool LoadSettingAsync();
        bool WriteSetting();
        bool GetParamsFromDB();
        void UpDateParamsToDB();
        void ResetParams(DBParams dBParams);
    }
}
