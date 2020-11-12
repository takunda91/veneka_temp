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
    class WcfTerminalDAL: ITerminalDAL
    {

        private readonly IComsCallback _proxy;
        public WcfTerminalDAL(IComsCallback proxy)
        {
            _proxy = proxy;
        }
        public TerminalMK GetTerminalMasterKey(string deviceId, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetTerminalMasterKey(deviceId, auditUserId, auditWorkStation);
        }

        public ZoneMasterKey GetZoneMasterKey(int issuerId, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetZoneMasterKey(issuerId, auditUserId, auditWorkStation);
        }
    }
}
