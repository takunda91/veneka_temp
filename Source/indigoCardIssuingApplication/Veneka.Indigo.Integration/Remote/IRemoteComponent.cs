using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Veneka.Indigo.Integration.Remote
{    
    [ServiceContract(Namespace = Constants.IndigoRemoteComponentURL)]
    public interface IRemoteComponent
    {
        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        RemoteCardUpdates GetPendingCardUpdates(string token);

        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response CardUpdateResults(string token, RemoteCardUpdatesResponse remoteCardUpdatesResponse);
    }
}
