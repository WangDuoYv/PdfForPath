
using PdfForPath;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfForPath
{
    class GetPatientInfo:HslLogs
    {
        public static string getPdfInfo(string filename)
        {
            try
            {
                PdfDocument document = new PdfDocument();
                document.LoadFromFile(filename);
                StringBuilder content = new StringBuilder();
                content.Append(document.Pages[0].ExtractText());
                return content.ToString();
            }
            catch (Exception ex)
            {
                InfoLog.WriteError("解析文件数据", ex.Message);
                return "";
            }
        }
        public static string getPdfInfo(string filename,int page)
        {
            try
            {
                PdfDocument document = new PdfDocument();
                document.LoadFromFile(filename);
                StringBuilder content = new StringBuilder();
                content.Append(document.Pages[page].ExtractText());
                return content.ToString();
            }
            catch (Exception ex)
            {
                InfoLog.WriteError("解析文件数据", ex.Message);
                return "";
            }
        }

        public static string BIECG_getinfo(string filename)
        {
            try
            {
                string content = getPdfInfo(filename);
                string[] examcode = content.ToString().Split(new string[] { "病历号:", "姓名:" }, StringSplitOptions.RemoveEmptyEntries);
                InfoLog.WriteDebug("解析文件数据", content.ToString().Trim());
                if (examcode[1].Replace(" ", "").Replace("\r\n", "").Length>30)
                {
                    return "";
                }
                return examcode[1].Replace(" ", "").Replace("\r\n", "");
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string MLBP_getinfo(string filename)
        {
            try
            {
                string content = getPdfInfo(filename);
                string[] examcode = content.ToString().Split(new string[] { "患者ID", "开始记录" }, StringSplitOptions.RemoveEmptyEntries);
                InfoLog.WriteDebug("解析文件数据", content.ToString().Trim());
                if (examcode[1].Replace(" ", "").Replace("\r\n", "").Length > 30)
                {
                    return "";
                }
                return examcode[1].Replace(" ", "").Replace("\r\n", "");
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string BIHolter_getinfo(string filename)
        {
            try
            {
                string content = getPdfInfo(filename);
                string[] examcode = content.ToString().Split(new string[] { "病例号:", "记录器:" }, StringSplitOptions.RemoveEmptyEntries);
                InfoLog.WriteDebug("解析文件数据", content.ToString().Trim());
                if (examcode[1].Replace(" ", "").Replace("\r\n", "").Length > 30)
                {
                    return "";
                }
                return examcode[1].Replace(" ", "").Replace("\r\n", "");
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// 待定 取值有问题取值有问题取值有问题
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string DMHolter_getinfo(string filename)
        {
            try
            {
                string content = getPdfInfo(filename,1);
                string[] examcode = content.ToString().Split(new string[] { " (门门门:", ") 24 小小小小小小" }, StringSplitOptions.RemoveEmptyEntries);
                InfoLog.WriteDebug("解析文件数据", content.ToString().Trim());
                if (examcode[1].Replace(" ", "").Replace("\r\n", "").Length > 30)
                {
                    return "";
                }
                return examcode[1].Replace(" ", "").Replace("\r\n", "");
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string MLHolter_getinfo(string filename)
        {
            try
            {
                string content = getPdfInfo(filename);
                string[] examcode = content.ToString().Split(new string[] { "ID #: ", "年龄" }, StringSplitOptions.RemoveEmptyEntries);
                InfoLog.WriteDebug("解析文件数据", content.ToString().Trim());
                if (examcode[1].Replace(" ", "").Replace("\r\n", "").Length > 30)
                {
                    return "";
                }
                return examcode[1].Replace(" ", "").Replace("\r\n", "");
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string MLStree_getinfo(string filename)
        {
            try
            {
                string content = getPdfInfo(filename);
                string[] examcode = content.ToString().Split(new string[] { "ID:", "Second ID:" }, StringSplitOptions.RemoveEmptyEntries);
                InfoLog.WriteDebug("解析文件数据", content.ToString().Trim());
                if (examcode[1].Replace(" ", "").Replace("\r\n", "").Length > 30)
                {
                    return "";
                }
                return examcode[1].Replace(" ", "").Replace("\r\n", "");
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string FGHolter_getinfo(string filename)
        {
            try
            {
                string content = getPdfInfo(filename);
                string[] examcode = content.ToString().Split(new string[] { "患者编号:", "科室:" }, StringSplitOptions.RemoveEmptyEntries);
                InfoLog.WriteDebug("解析文件数据", content.ToString().Trim());
                if (examcode[1].Replace(" ", "").Replace("\r\n", "").Length > 30)
                {
                    return "";
                }
                return examcode[1].Replace(" ", "").Replace("\r\n", "");
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string STDLBP_getinfo(string filename)
        {
            try
            {
                string content = getPdfInfo(filename);
                string[] examcode = content.ToString().Split(new string[] { "ID号：", "门诊号：" }, StringSplitOptions.RemoveEmptyEntries);
                InfoLog.WriteDebug("解析文件数据", content.ToString().Trim());
                if (examcode[1].Replace(" ", "").Replace("\r\n", "").Length > 30)
                {
                    return "";
                }
                return examcode[1].Replace(" ", "").Replace("\r\n", "");
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string PY_getinfo(string filename)
        {
            try
            {
                string name= System.IO.Path.GetFileNameWithoutExtension(filename);
                return System.Text.RegularExpressions.Regex.Replace(name, @"[^a-zA-Z0-9]+", "");
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
