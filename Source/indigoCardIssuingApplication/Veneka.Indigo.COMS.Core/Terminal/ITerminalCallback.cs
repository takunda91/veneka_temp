using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.Core.Terminal
{
    public interface ITerminalCallback
    {
        ComsResponse<TerminalDetails> GetDevice(string deviceId, AuditInfo auditInfo);
        ComsResponse<Product> GetProduct(int? issuerId, string searchBIN, AuditInfo auditInfo);
        void GetTerminalParameters();
        void GetZoneMasterKey();
    }
}
