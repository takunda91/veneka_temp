using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.DataSource.Callbacks
{
    public class WcfParametersDAL:IParametersDAL
    {
        private readonly IComsCallback _proxy;
        public WcfParametersDAL(IComsCallback proxy)
        {
            _proxy = proxy;
        }
        public Parameters GetParameterIssuerInterface(int issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetParameterIssuerInterface(issuerId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }

        public Parameters GetParameterProductInterface(int productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetParameterProductInterface(productId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }

        public List<Parameters> GetParametersIssuerInterface(int? issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetParametersIssuerInterface(issuerId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }

        public List<Parameters> GetParametersProductInterface(int? productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetParametersProductInterface(productId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }
    }
}
