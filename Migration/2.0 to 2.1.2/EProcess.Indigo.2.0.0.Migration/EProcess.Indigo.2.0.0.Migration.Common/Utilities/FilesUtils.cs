using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EProcess.Indigo._2._0._0.Migration.Common.Utilities
{
    public static class FilesUtils
    {

        //private static string _dirPath;

        public static string dirPath
        {
            get
            {
                string _dirPath = string.Empty;

                if (string.IsNullOrEmpty(_dirPath))
                {
                    _dirPath = ConfigurationManager.AppSettings["ScriptsDir"] == null
                        ? ""
                        : Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["ScriptsDir"].ToString());
                }// end if (string.IsNullOrEmpty(_dirPath))

                return _dirPath;
            }
            //set { _dirPath = value; }
        }

        /// <summary>
        /// Loads an SQL Script file and returns the SQL statements containded in the files as a string
        /// </summary>
        /// <param name="fileNo">The File to be loaded</param>
        /// <param name="scriptType">Script type, 'Export' by default</param>
        /// <returns></returns>
        public static Dictionary<int, FileInfo> GetSQLScript(string scriptType = "Export")
        {
            Dictionary<int, FileInfo> scripts = new Dictionary<int, FileInfo>();

            #region Load SQL Script File

            //For exporting data
            var localDirPath = dirPath;

            localDirPath = !localDirPath.Contains(scriptType)
                       ? Path.Combine(localDirPath, scriptType)
                       : localDirPath;

            if (Directory.Exists(localDirPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(localDirPath);

                var files = dirInfo.GetFiles("*.sql", SearchOption.TopDirectoryOnly);

                foreach(var file in files)
                {
                    var fileName = file.Name.Split(' ');
                    scripts.Add(int.Parse(fileName[0]), file);
                }
                //var files = filesArray == null
                //            ? DirectoryInfo .GetFiles(localDirPath)
                //            : filesArray;

                //var fileName = localDirPath + fileNo;

                //string script = files
                //    .SingleOrDefault
                //        (
                //            file => file.StartsWith(fileName)
                //        );


                //sqlString = File.ReadAllText(script[0].FullName);

            }// end if (Directory.Exists(dirPath))
            else
                throw new Exception("Scripts directory could not be located!");

            #endregion

            return scripts;
        }// end string GetSQLScript(string fileNo)

        public static bool CreateFile(string fileName, string fileContent, FileExtension fileExt = FileExtension.TXT, FileMode fileMode = FileMode.Append)
        {
            var isCreated = false;

            try
            {
                var filePath = (fileContent.Contains("CREATE DATABASE"))
                    ? dirPath
                    : dirPath.Replace("Scripts/", "");

                filePath += "Output/";

                var newDirPath = filePath + fileName.Substring(0, fileName.LastIndexOf("/"));

                if (!Directory.Exists(newDirPath))
                    Directory.CreateDirectory(newDirPath);

                var newFile = filePath + fileName + "." + fileExt.ToString().ToLower(); 
                
                if (!string.IsNullOrEmpty(fileContent))
                {

                    using (var fs = new FileStream(newFile, fileMode, FileAccess.Write))
                    {
                        using (var sw = new StreamWriter(fs))
                        {
                            sw.WriteLine(fileContent);
                            sw.Flush();
                        }// end using (var sw = new StreamWriter(fs))
                    }// end using (var fs = new FileStream(newFile, FileMode.Append, FileAccess.Write))

                    isCreated = true;
                }// end if (!string.IsNullOrEmpty(fileContent))
                else
                    throw new Exception(string.Format("Cannot create file '{0}' without any content!", newFile));

            }// end try
            catch
            {
                throw;
            }// end catch

            return isCreated;
        }// end method static bool CreateFile(string fileName, string fileExt = "txt", string fileContent = null)
    }


    public enum FileExtension
    {
        TXT,
        SQL,
        CSV
    }
}
