using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfForPath
{
    internal class FileHelper
    {
        public static bool FileIsValid(string fileName)
        {
            bool result;
            if (!File.Exists(fileName))
            {
                result = false;
            }
            else
            {
                if (new FileInfo(fileName).Length != 0L)
                {
                    try
                    {
                        File.Move(fileName, fileName);
                    }
                    catch
                    {
                        return false;
                    }
                    return true;
                }
                result = false;
            }
            return result;
        }
    }
}
