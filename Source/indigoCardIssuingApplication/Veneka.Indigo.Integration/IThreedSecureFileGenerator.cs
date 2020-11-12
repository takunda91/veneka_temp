using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Objects;
using System.ComponentModel.Composition;
using Common.Logging;

namespace Veneka.Indigo.Integration
{
    [InheritedExport(typeof(IThreedSecureFileGenerater))]
    public interface IThreedSecureFileGenerater : ICommon
    {
        string BaseFileDir { get; set; }

        string SQLConnectionString { get; set; }
        //TODO: Should maybe look at using reflection to check the interfaces guid instead of passing it in the below methods.

        bool GenearteThreedSecureFiles(string interfaceGuid, int languageId, long auditUserId, string auditWorkStation);
    }
}
