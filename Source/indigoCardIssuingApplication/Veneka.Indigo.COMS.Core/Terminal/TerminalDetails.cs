using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Config;

namespace Veneka.Indigo.COMS.Core.Terminal
{
    public class TerminalDetails
    {
        public int IssuerId { get; set; }
        public int BranchId { get; set; }
        public string IssuerName { get; set; }
        public string TerminalMasterKey { get; set; }
        public string ZoneMasterKey { get; set; }
        public string ZoneMasterKey2 { get; set; }
        public InterfaceInfo HSMConfig { get; set; }
        public InterfaceInfo CMSConfig { get; set; }
    }
}
