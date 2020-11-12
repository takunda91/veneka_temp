using System.ServiceModel;
using Veneka.Indigo.Integration.Common;

namespace Veneka.Indigo.Integration.DAL
{
    [ServiceContract]

    public interface ITransactionSequenceDAL
    {
        [OperationContract]
        void Dispose();
        [OperationContract]
        int NextSequenceNumber(string sequenceName, ResetPeriod resetPeriod);
        [OperationContract]
        long NextSequenceNumberLong(string sequenceName, ResetPeriod resetPeriod);
    }
}