using System;
using System.IO;

namespace Veneka.Indigo.Common.Utilities
{
    public static class LogFileWriter
    {
        public static void WriteWebServerError(string module, Exception exception)
        {
            WriteException(module, StaticNames.WEB_SERV_ERR_FILE_PATH, exception);
        }

        public static void WriteFileLoaderError(string module, Exception exception)
        {
            WriteException(module, StaticNames.FILE_LOAD_ERR_FILE_PATH, exception);
        }

        public static void WriteFileLoaderComment(string comment)
        {
            WriteComment(comment, StaticNames.FILE_LOAD_LOG_FILE_PATH);
        }

        private static void WriteException(string module, string logFile, Exception exception)
        {
            StreamWriter sw;
            try
            {
                if (!Directory.Exists(logFile))
                {
                    Directory.CreateDirectory(logFile);
                }

                if (File.Exists(logFile + DateTime.Now.ToString("yyyy-MM-dd") + ".txt"))
                {
                    sw = File.AppendText(logFile + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                }
                else
                {
                    sw = File.CreateText(logFile + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                }

                try
                {
                    sw.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " : " + module + " - " + exception);
                    sw.Flush();
                }
                finally
                {
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void WriteComment(string comment, string logFile)
        {
            StreamWriter sw;
            try
            {
                if (!Directory.Exists(logFile))
                {
                    Directory.CreateDirectory(logFile);
                }

                if (File.Exists(logFile + DateTime.Now.ToString("yyyy-MM-dd") + ".txt"))
                {
                    sw = File.AppendText(logFile + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                }
                else
                {
                    sw = File.CreateText(logFile + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                }

                try
                {
                    sw.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " : " + comment);
                    sw.Flush();
                }
                finally
                {
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}