using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCollisionCalibration.Normal
{
    /// <summary>
    /// 文件保存的路径
    /// </summary>
    public class FilePath
    {
        private const string HomeFolder = @"C:\ToolCollisionCalibration"; //主文件夹
        public const string LogFolder = HomeFolder + @"\LogFolder";//左侧日志文件夹
        public const string ParameterFolder = HomeFolder + @"\ParameterFolder"; //参数文件夹路径
        public const string ParameterJsonFileName = ParameterFolder + @"\Parameter.json"; //参数文件名Json
        public const string DBParamFile = DBParamFolder + @"\DBParamFile.json";
        public const string DataFolder = HomeFolder + @"\DataFolder";//日志文件夹
        public const string DBParamFolder = HomeFolder + @"\DBParamFolder";//型号参数文件夹

    }
}
