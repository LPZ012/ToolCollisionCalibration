using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLibrary.AngleDevice;
using WPFLibrary.Scanner;
using WPFLibrary.Torque;

namespace ToolCollisionCalibration.Devices
{
    public interface IDevice
    {
        MotionCard motionCard { get; set; }
        IScanner<byte[]> Scanner { get; set; }
        ITorqueDevice<double> TorqueDevice { get; set; }
        IAngleDevice<double> AngleDevice { get; set; }
        void InitDevices();
    }
}
