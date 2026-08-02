using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCollisionCalibration.Servers.Message.ViewBToViewA
{
    /// <summary>
    /// 用于视图B发布事件和视图A订阅事件的服务类,里面不需要写任何东西
    /// </summary>
    public class ViewBToViewAServer: PubSubEvent<ViewBToViewAModel>
    {

    }
}
