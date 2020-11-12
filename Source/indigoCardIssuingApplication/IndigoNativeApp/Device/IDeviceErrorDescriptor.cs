using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DesktopApp.Device
{
    /// <summary>
    /// Interface to map error codes to a usable Description of the error and a helpful hint to the user on how to resolve the error.
    /// </summary>
    public interface IDeviceErrorDescriptor
    {
        int Code { get; }
        string Description { get; }
        string HelpfulHint { get; }
    }
}
