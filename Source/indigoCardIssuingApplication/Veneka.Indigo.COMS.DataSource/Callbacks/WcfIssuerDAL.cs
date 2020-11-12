using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.FileLoader.Objects;

namespace Veneka.Indigo.COMS.DataSource.Callbacks
{
    public class WcfIssuerDAL: IIssuerDAL
    {
        private readonly IComsCallback _proxy;
        public WcfIssuerDAL(IComsCallback proxy)
        {
            _proxy = proxy;
        }

        public Issuer GetIssuer(int issuerId, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetIssuer(issuerId, auditUserId, auditWorkStation);

        }
    }
}
