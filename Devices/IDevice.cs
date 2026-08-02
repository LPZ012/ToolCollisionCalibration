using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLibrary.ComSerialPort;
using WPFLibrary.Scanner;
using WPFLibrary.Zmotion;

namespace ToolCollisionCalibration.Devices
{
    public interface IDevice
    {
        IPulseADIOControler IPulseADIOControler { get; set; }
        IScanner<byte[]> Scanner { get; set; }
        void InitDevices();
    }
}
