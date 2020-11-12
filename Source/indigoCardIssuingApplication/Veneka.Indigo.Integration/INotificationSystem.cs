using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Objects;
using System.ComponentModel.Composition;
using Common.Logging;
using Veneka.Indigo.Integration.Common;

namespace Veneka.Indigo.Integration
{
    /// <summary>
    /// This interfaces provides a contract between Indigo and the notifications integration layer.
    /// </summary>
    [InheritedExport(typeof(INotificationSystem))]
    public interface INotificationSystem : ICommon
    {
        bool SMS(ref List<Notification> notifications, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);

        bool Email(ref List<Notification> notifications, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);
    }    
}
