using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra.Error
{
    public class ZebraErrorFactory : IZebraErrorFactory
    {
        private readonly ResourceManager _rm = ZebraErrors.ResourceManager;

        public IDeviceErrorDescriptor GetErrorDescription(int errorCode)
        {
            return GetErrorDescription(errorCode, CultureInfo.CurrentCulture);
        }

        public IDeviceErrorDescriptor GetErrorDescription(int errorCode, CultureInfo cultureInfo)
        {
            var errorDesc = _rm.GetString(ErrorStringBuilder(errorCode), cultureInfo) ?? GetUnknownErrorDesc(errorCode, cultureInfo);
            var helpfulHint = _rm.GetString(HelpfulHintStringBuilder(errorCode), cultureInfo) ?? GetUnknownHelpfulHint(errorCode, cultureInfo);

            return new ZebraErrorDescription(errorCode, errorDesc, helpfulHint);
        }

        /// <summary>
        /// Returns the default message for unknown error description
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public string GetUnknownErrorDesc(int errorCode, CultureInfo cultureInfo)
        {
            return string.Format("{0} - {1}", _rm.GetString("zUnknown_desc", cultureInfo), errorCode);
        }

        /// <summary>
        /// Returns the default message for unknown helpful hints
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public string GetUnknownHelpfulHint(int errorCode, CultureInfo cultureInfo)
        {
            return string.Format("{0} - {1}", _rm.GetString("zUnknown_help", cultureInfo), errorCode);
        }

        /// <summary>
        /// Builds the lookup string for error description
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public string ErrorStringBuilder(int errorCode)
        {
            return string.Format("z{0}_desc", errorCode);
        }

        /// <summary>
        /// Builds the lookup string for helpful hint
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public string HelpfulHintStringBuilder(int errorCode)
        {
            return string.Format("z{0}_help", errorCode);
        }
    }
}
