using System;
using System.IO;

namespace IndigoFileLoader.utility
{
    public static class FileLogger
    {
        public static void Write(string message)
        {
            StreamWriter sw;

            try
            {
                if (!Directory.Exists(@".\FileLoadLog\"))
                {
                    Directory.CreateDirectory(@".\FileLoadLog\");
                }

                if (File.Exists(@".\FileLoadLog\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt"))
                {
                    sw = File.AppendText(@".\FileLoadLog\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                }
                else
                {
                    sw = File.CreateText(@".\FileLoadLog\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                }

                try
                {
                    sw.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " : " + message);

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