using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EProcess.Indigo._2._0._0.Migration.Common.Utilities
{
    public static class ErrorLogger
    {
        private static string fileName 
        {
            get
            {
                return string.Format("Logs/{0}"
                        , DateTime.Now.ToString("dd-MMM-yy"));
            }// end get
        }// end property static string fileName 

        /// <summary>
        /// Method Logs an error's message and the stacktrace to the log
        /// </summary>
        /// <param name="ex">Exception to log</param>
        public static void LogError(Exception ex)
        {
            string error = ex.Message,
                   stackStrace = ex.StackTrace.Trim();

            var msg = string.Format("Time Stamp: {0}{1}", DateTime.Now.ToString("hh:mm:ss"), Environment.NewLine);
                msg += string.Format("Error: {0}{1}", error, Environment.NewLine);
                
            if (ex.InnerException != null)
                msg += string.Format("Inner Error: {0}{1}", ex.InnerException, Environment.NewLine);

            msg += string.Format("Stacktrace: {0}{1}", stackStrace, Environment.NewLine);

            FilesUtils.CreateFile(fileName, msg);
        }// end method static void LogError(Exception ex)

        /// <summary>
        /// Logs an error's message to the log
        /// </summary>
        /// <param name="msg">Message to log</param>
        public static void LogError(string msg)
        {
            msg = string.Format("Time Stamp: {0}{2}Error: {1}{2}{2}"
                    , DateTime.Now.ToString("hh:mm:ss"), msg, Environment.NewLine);

            FilesUtils.CreateFile(fileName, msg);
        }// end method static void log(string fileName, string msg)

        /// <summary>
        /// Logs an info message to the log
        /// </summary>
        /// <param name="msg">Message to log</param>
        public static void LogInfo(string msg)
        {
            msg = string.Format("Time Stamp: {0}{2}Info: {1}{2}{2}"
                    , DateTime.Now.ToString("hh:mm:ss"), msg, Environment.NewLine);

            FilesUtils.CreateFile(fileName, msg);
        }// end method static void log(string fileName, string msg)
    }// end method static class ErrorLogger
}
