using System.Collections.Generic;
using System.ServiceModel;
using Veneka.Indigo.Integration.FileLoader.Objects;

namespace Veneka.Indigo.Integration.DAL
{
    [ServiceContract]
    public interface IBranchDAL
    {
        [OperationContract]
        BranchLookup GetBranch(string branchCode, string branchName, int issuerId);
        [OperationContract]
        BranchLookup GetBranchesForIssuer(int issuerId);
        [OperationContract]
        List<BranchLookup> GetCardCentreList(int issuerId);
        [OperationContract]
        List<BranchLookup> GetBranchesForIssuerByIssuerCode(string issuerCode);

    }
}