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
    public class WcfBranchDAL:IBranchDAL
    {
        private readonly IComsCallback _proxy;
        public WcfBranchDAL(IComsCallback proxy)
        {
            _proxy = proxy;
        }

       public BranchLookup GetBranch(string branchCode, string branchName, int issuerId)
        {
            return _proxy.GetBranch(branchCode, branchName, issuerId);
        }

        public BranchLookup GetBranchesForIssuer(int issuerId)
        {
            return _proxy.GetBranchesForIssuer(issuerId);
        }

        public List<BranchLookup> GetBranchesForIssuerByIssuerCode(string issuerCode)
        {
            return _proxy.GetBranchesForIssuerByIssuerCode(issuerCode);
        }

        public List<BranchLookup> GetCardCentreList(int issuerId)
        {
            return _proxy.GetCardCentreList(issuerId);
        }
    }
}
