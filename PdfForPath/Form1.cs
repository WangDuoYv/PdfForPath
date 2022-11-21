using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FluentFTP;


namespace PdfForPath
{
    public partial class Form1 : Form
    {
        //监控路径
        private static string FilePath = ConfigurationManager.AppSettings["path"];
        //文件类型
        private static string Type = ConfigurationManager.AppSettings["type"];
        //处理方式
        //private static string handleType = ConfigurationManager.AppSettings["handleType"];

        private static string ip = ConfigurationManager.AppSettings["ftpIp"];
        private static string ftpUserName = ConfigurationManager.AppSettings["ftpUserName"];
        private static string ftpPassword = ConfigurationManager.AppSettings["ftpPassword"];
        private static string FtpPath = ConfigurationManager.AppSettings["FtpPath"];

        delegate void AddFileList(string type, string str);
        private static object locker = new object();//创建锁

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //注意判断关闭事件Reason来源于窗体按钮，否则用菜单退出时无法退出!
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;    //取消"关闭窗口"事件
                this.WindowState = FormWindowState.Minimized;    //使关闭时窗口向右下角缩小的效果
                this.ShowInTaskbar = false;
                //notifyIcon1.Visible = true;
                this.Hide();
                return;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            this.Show();
            WindowState = FormWindowState.Normal;
            //this.Focus();
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //notifyIcon1.Visible = false;
            this.ShowInTaskbar = true;
            this.Show();
            WindowState = FormWindowState.Normal;
            //this.Focus();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(" 程序退出后服务将不可用,确认退出? ", " 确认退出 ", MessageBoxButtons.YesNo))
            {
                Environment.Exit(0);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            fileSystemWatcher1.Path = FilePath;
            fileSystemWatcher1.Filter = "*.pdf";
            this.notifyIcon1.ShowBalloonTip(30);


        }

        private void fileSystemWatcher1_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            //创建线程执行操作pdf文件和listBox控件 优化体验
            ThreadStart starter = delegate { Watcher(e); };
            new Thread(starter).Start();
        }

        public void Watcher(FileSystemEventArgs e)
        {
            lock (locker)
            {
                Thread.Sleep(1000);
                while (!FileHelper.FileIsValid(e.FullPath))
                {
                    Thread.Sleep(500);
                }
                SetList("1", e.Name);
                string[] files = Directory.GetFiles(FilePath);//获取文件夹所有文件
                foreach (string file in files)
                {
                    if (File.Exists(file) && file.Contains(".pdf") && e.FullPath != file)//先执行之前的报告
                    {
                        SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "检测到还有其他文件未上传,正在重新上传，请稍后");
                        HslLogs.InfoLog.WriteError("检测到之前有文件未上传,正在重新上传，请稍后");
                        PdfHandler(System.IO.Path.GetFileName(file), Type, "2");
                    }
                }
                Thread.Sleep(2000);
                PdfHandler(e.Name, Type, "1");
                ClearMemory();
            }
        }
        public void PdfHandler(string fileName, string Type, string state)//监控到的pdf全路径  报告文件厂家类型  用来判断是为本次上传还是之前未成功的上传 1为本次 2为之前
        {
            try
            {
                string pdfName = FilePath + fileName;
                if (File.Exists(pdfName))//如果有文件 继续执行 
                {
                    SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "检测到文件:" + fileName);
                    HslLogs.InfoLog.WriteError("检测到文件:" + fileName);
                    string num = string.Empty;
                    //BIECG BIHolter MLBP MLHolter MLStree  DMHolter.....
                    switch (Type)
                    {
                        case "BIECG":
                            num = GetPatientInfo.BIECG_getinfo(pdfName);
                            break;
                        case "BIHolter":
                            num = GetPatientInfo.BIHolter_getinfo(pdfName);
                            break;
                        case "MLBP":
                            num = GetPatientInfo.MLBP_getinfo(pdfName);
                            break;
                        case "MLHolter":
                            num = GetPatientInfo.MLHolter_getinfo(pdfName);
                            break;
                        case "MLStree":
                            num = GetPatientInfo.MLStree_getinfo(pdfName);
                            break;
                        case "DMHolter":
                            num = GetPatientInfo.DMHolter_getinfo(pdfName);
                            break;
                        case "PY":
                            num = GetPatientInfo.PY_getinfo(pdfName);
                            break;
                        case "FGHolter":
                            num = GetPatientInfo.FGHolter_getinfo(pdfName);
                            break;
                        case "STDLBP":
                            num = GetPatientInfo.STDLBP_getinfo(pdfName);
                            break;
                        default:
                            SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "获取类型失败，请检查配置文件type");
                            //记录日志
                            HslLogs.InfoLog.WriteError("配置文件报告类型有误。");
                            num = "";
                            break;
                    }
                    if (num != "")//取到医嘱号
                    {
                        SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "开始操作文件" + fileName);
                        HslLogs.InfoLog.WriteInfo("开始操作文件" + fileName);
                        string FileName = num + "-" + Type + ".pdf";
                        SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "文件更名为：" + FileName);
                        HslLogs.InfoLog.WriteInfo("文件更名为：" + FileName);
                        string NewPdfPathName = FilePath + "\\" + FileName;
                        if (File.Exists(NewPdfPathName))//新名字文件是否存在
                        {
                            if (fileName != FileName)//前后名字是否一致
                            {
                                File.Delete(NewPdfPathName);//
                                File.Move(pdfName, NewPdfPathName);//更改文件名不换路径
                            }
                        }
                        else
                        {
                            File.Move(pdfName, NewPdfPathName);//更改文件名不换路径
                        }
                        //ftp上传
                        UploadFile(ip, @FtpPath, ftpUserName, ftpPassword, @NewPdfPathName);
                        if (Type == "PY")
                        {
                            PY_delete(FilePath);
                        }
                        if (state == "1")
                        {
                            this.notifyIcon1.BalloonTipText = "报告文件上传成功";
                            this.notifyIcon1.ShowBalloonTip(300);
                        }

                        SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "操作文件完成,继续执行监听");
                        SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "--------------------------------------------\n");
                        HslLogs.InfoLog.WriteError("操作文件结束,继续执行监听");
                        HslLogs.InfoLog.WriteInfo("--------------------------------------------\n");
                    }
                    else//未取到医嘱号
                    {
                        SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "未获取到病历号" + fileName);
                        HslLogs.InfoLog.WriteInfo("未获取到病历号:" + fileName);
                        string FileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_unknow-" + Type + ".pdf";
                        SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "文件更名为：" + FileName);
                        HslLogs.InfoLog.WriteInfo("文件更名为：" + FileName);
                        SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "开始操作文件" + fileName);
                        HslLogs.InfoLog.WriteError("开始操作文件" + fileName);
                        string NewPdfPathName = FilePath + "\\" + FileName;
                        if (File.Exists(NewPdfPathName))
                        {
                            if (fileName != FileName)
                            {
                                File.Delete(NewPdfPathName);
                                File.Move(pdfName, NewPdfPathName);//更改文件名不换路径
                            }
                        }
                        else
                        {
                            File.Move(pdfName, NewPdfPathName);//更改文件名不换路径
                        }
                        //ftp上传
                        UploadFile(ip, @FtpPath, ftpUserName, ftpPassword, @NewPdfPathName);
                        if (Type == "PY")
                        {
                            PY_delete(FilePath);
                        }
                        if (state == "1")
                        {
                            this.notifyIcon1.BalloonTipText = "未取到登记号，可能未输入或类型错误";
                            this.notifyIcon1.ShowBalloonTip(300);
                        }
                        SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "操作文件完成,继续执行监听");
                        SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "--------------------------------------------\n");
                        HslLogs.InfoLog.WriteError("操作文件结束,继续执行监听");
                        HslLogs.InfoLog.WriteInfo("--------------------------------------------\n");
                    }
                }
            }
            catch (Exception e)
            {

                if (Type == "PY")
                {
                    PY_delete(FilePath);
                }
                if (state == "1")
                {
                    this.notifyIcon1.BalloonTipText = "上传出现了异常";
                    this.notifyIcon1.ShowBalloonTip(3000);
                }
                SetList("2", "[" + DateTime.Now.ToString() + "] 异常" + e.Message.ToString() + "\n" + e.StackTrace.ToString());
                HslLogs.InfoLog.WriteError("处理文件异常：", e.Message.ToString() + "\n位置:" + e.StackTrace.ToString());
                HslLogs.InfoLog.WriteInfo("--------------------------------------------\n");
                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// ftp上传方法
        /// </summary>
        /// <param name="ip">ftp的ip</param>
        /// <param name="FtpPath">要上传的文件在ftp的相对位置  / 是根目录</param>
        /// <param name="userName">ftp登录名</param>
        /// <param name="Password">ftp登录密码</param>
        /// <param name="filename">要上传的文件的路径</param>
        public void UploadFile(string ip, string FtpPath, string userName, string Password, string file)
        {
            try
            {
                HslLogs.InfoLog.WriteInfo("开始执行ftp上传操作");
                SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "开始执行ftp上传操作");
                FtpClient ftp = new FtpClient();
                ftp.Host = ip;
                ftp.Port = 21;//ftp端口号 默认21
                ftp.Credentials = new NetworkCredential(userName, Password);
                ftp.Connect();
                using (var fileStream = File.OpenRead(file))
                using (var ftpstream = ftp.OpenWrite(FtpPath + Path.GetFileName(file)))//要保存文件的相对地址     要保存的文件名包括后缀名
                {
                    var buffer = new byte[8 * 1024];
                    int count;
                    while ((count = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ftpstream.Write(buffer, 0, count);
                    }
                    HslLogs.InfoLog.WriteInfo("上传成功，开始执行删除操作");
                    SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "上传成功，开始执行删除操作");
                }
                if (DeleteFile(file))
                {
                    HslLogs.InfoLog.WriteInfo("删除文件成功");
                    SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "删除文件成功");
                }
                else
                {
                    SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "删除文件失败");
                    HslLogs.InfoLog.WriteInfo("删除文件失败");
                }
            }
            catch (Exception e)
            {
                this.notifyIcon1.BalloonTipText = "上传文件失败，请检查网络或配置文件";
                this.notifyIcon1.ShowBalloonTip(300);
                HslLogs.InfoLog.WriteError("上传失败：" + e.Message.ToString() + e.StackTrace.Trim().ToString());
                SetList("2", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + "上传失败：\n" + e.Message.ToString());
                throw;
            }
        }
        /// <summary>
        /// 删除文件操作
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public static bool DeleteFile(string FilePath)
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                    return true;
                }
                else
                {
                    HslLogs.InfoLog.WriteError("删除文件不存在");
                    return false;
                }
            }
            catch (Exception e)
            {
                HslLogs.InfoLog.WriteError("删除操作时出现了异常:  " + e.Message.ToString() + e.StackTrace.ToString());
                return false;
            }
        }
        //鹏阳的单独处理删除多余的文件
        public static void PY_delete(string Path)
        {
            try
            {
                Thread.Sleep(3000);
                System.IO.DirectoryInfo directoryInfo = new DirectoryInfo(Path);
                directoryInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
                System.IO.File.SetAttributes(Path, System.IO.FileAttributes.Normal);
                if (Directory.Exists(Path))
                {
                    foreach (string f in Directory.GetFileSystemEntries(Path))
                    {
                        if (File.Exists(f))
                        {
                            if (!f.Contains(".pdf"))
                            {
                                File.Delete(f);
                            }
                        }
                        else
                        {
                            //PY_delete(f);
                            Directory.Delete(f, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HslLogs.InfoLog.WriteError("删除PY类型文件目录出错：" + e.Message.ToString() + e.StackTrace.Trim().ToString());
                throw;
            }
        }
        /// <summary>
        /// 设置listBox值 记录日志
        /// </summary>
        /// <param name="type"></param>
        /// <param name="str"></param>
        public void SetList(string type, string str)
        {
            AddFileList d = new AddFileList(AddFileListItem);
            label1.Invoke(d, new object[] { type, str });
        }
        public void AddFileListItem(string type, string str)
        {
            if (type == "1")
            {
                listBox1.Items.Add(str);
            }
            else
            {
                listBox2.Items.Add(str);
            }
        }
        #region 内存回收
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Form1.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion
    }
}
