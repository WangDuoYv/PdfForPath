using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PdfForPath
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool runone;
            //这是我在测试大牛🐎啊
            System.Threading.Mutex run = new System.Threading.Mutex(true, "single_test", out runone);

            if (!runone)

            {
                MessageBox.Show("应用程序已经在运行中。。");
                Thread.Sleep(1000);
                System.Environment.Exit(0);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
