using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.DesktopApp.Config
{
    public enum DeviceSearchOption
    {
        OnlyInclude = 0,
        Include = 1,
        Exclude = 2
    }

    public static class ConfigReader
    {
        public static DeviceSearchOption GetDeviceSearchOption(string device)
        {
            DeviceSearchOption rtn = DeviceSearchOption.Include;
            
            if(ConfigurationManager.AppSettings.HasKeys())
            {
                var strOption = ConfigurationManager.AppSettings.Get(device);

                if (String.IsNullOrWhiteSpace(strOption))
                    return rtn;

                if (!Enum.TryParse(strOption, out rtn))
                    throw new Exception("'" + strOption + "' not a valid value for device. Must be OnlyInclude/Include/exclude");
            }

            return rtn;
        }
    }
}
