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
    [InheritedExport(typeof(ICardFileProcessor))]
    public interface ICardFileProcessor : ICommon
    {
        string BaseFileDir { get; set; }

        string SQLConnectionString { get; set; }
        //TODO: Should maybe look at using reflection to check the interfaces guid instead of passing it in the below methods.

        bool ProcessCardsFiles(int? issuerId, int? productId, string interfaceGuid, int languageId, long auditUserId, string auditWorkStation);

        bool GenerateFiles(int issuerId, int? productId, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation);

        bool ProcessBulkRequestFiles(int? issuerId, int? productId, string interfaceGuid, int languageId, long auditUserId, string auditWorkStation);
    }
}
