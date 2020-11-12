using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Objects;
using System.ComponentModel.Composition;
using Common.Logging;

namespace Veneka.Indigo.Integration
{
    /// <summary>
    /// This interfaces provides a contract between Indigo and the Hardware Security Module integration layer.
    /// </summary>
    [InheritedExport(typeof(ICardProductionSystem))]
    public interface ICardProductionSystem : ICommon
    {
        bool UploadToCardProduction(ref List<CardObject> cardObjects, External.ExternalSystemFields externalFields, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);                
    }    
}
