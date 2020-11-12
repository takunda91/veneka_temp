using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndigoMigrationToolkit.Utils
{
    public class WindowLogging
    {
        private readonly TextBox _logWindow;
        private bool isFirstLine = true;

        public WindowLogging(TextBox textbox)
        {
            _logWindow = textbox;
        }

        public void Write(string text)
        {
            string start = Environment.NewLine;

            if (isFirstLine)
            {
                start = String.Empty;
                isFirstLine = false;
            }                

            _logWindow.BeginInvoke(new Action(() =>
            {
                _logWindow.AppendText(String.Format("{0}{1} - {2}", start, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), text));
                _logWindow.SelectionStart = _logWindow.Text.Length - _logWindow.Lines[_logWindow.Lines.Length-1].Length;
                _logWindow.ScrollToCaret();
            }));            
        }        

        public void WriteFormat(string format, params object[] args)
        {
            Write(String.Format(format, args));
        }
    }
}
