using HslCommunication.LogNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PdfForPath
{
    internal class HslLogs
    {
        //System.Windows.Forms.Application.StartupPath 可执行文件的所在目录
        public static ILogNet InfoLog = new LogNetDateTime(System.Windows.Forms.Application.StartupPath + "\\Logs", GenerateMode.ByEveryDay);
    }
}
