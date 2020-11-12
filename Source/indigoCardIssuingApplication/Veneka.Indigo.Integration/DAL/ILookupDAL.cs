using System.ServiceModel;

namespace Veneka.Indigo.Integration.DAL
{
    [ServiceContract]
    public interface ILookupDAL
    {
        [OperationContract]
        string LookupBranchCode(int branchId);
        [OperationContract]
        string LookupBranchName(int branchId);
        [OperationContract(Name = "LookupCurrencyByCurrency")]
        string LookupCurrency(int currencyId);
        [OperationContract(Name = "LookupCurrencyByCCY")]
        int LookupCurrency(string ccy);
        [OperationContract]
        string LookupCurrencyISONumericCode(int currencyId);
        [OperationContract]
        string LookupEmpBranchCode(int branchId);
    }
}