using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.COMS.Core.Indigo.DataContracts;

namespace Veneka.Indigo.COMS.Core.Indigo
{
    public interface IPINReissueCallback
    {
        ComsResponse<PINReissue> RequestPINReissue(int issuerId, int branchId, int productId, string PAN, byte[] requestId, long operatorId, AuditInfo auditInfo);

    }
}
