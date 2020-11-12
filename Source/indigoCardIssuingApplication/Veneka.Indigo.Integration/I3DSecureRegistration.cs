using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Objects;

namespace Veneka.Indigo.Integration
{
    [InheritedExport(typeof(I3DSecureRegistration))]
    public interface I3DSecureRegistration : ICommon
    {
        string BaseFileDir { get; set; }

        string SQLConnectionString { get; set; }

        //TODO: Should maybe look at using reflection to check the interfaces guid instead of passing it in the below methods.
        bool Generate3DSecureFiles(List<ThreeDSecureCardDetails> threeDSecureDetails, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);        
    }
}
