using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Integration
{
    [InheritedExport(typeof(ICardFileProcessor))]
    public interface ICardRenewalFileProcessor : ICommon
    {
        string BaseFileDir { get; set; }

        string SQLConnectionString { get; set; }

        bool ProcessCardsFiles(int? issuerId, string interfaceGuid, int languageId, long auditUserId, string auditWorkStation);
    }
}
