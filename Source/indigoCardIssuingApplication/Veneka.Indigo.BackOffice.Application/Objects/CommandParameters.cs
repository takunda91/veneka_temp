using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Veneka.Indigo.BackOffice.Application.Devices;

namespace Veneka.Indigo.BackOffice.Application.Objects
{
   public class CommandParameters
    {
        public string Text { get; set; }
        public IDevice Device { get; set; }
    }

    public  class CommandParametersConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CommandParameters parameters = new CommandParameters();
            foreach (var obj in values)
            {
                if (obj is string) parameters.Text = (string)obj;
                else if (obj is bool) parameters.Device = (IDevice)obj;
            }
            return parameters;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
