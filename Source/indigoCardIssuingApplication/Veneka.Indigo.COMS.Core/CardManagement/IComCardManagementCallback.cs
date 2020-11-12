using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.Core.CardManagement
{
    public interface IComCardManagementCallBack
    {
        ///// <summary>
        ///// Raised if long running upload happens
        ///// </summary>
        //event EventHandler<DistEventArgs> OnUploadCompleted;

        //bool OnUploadCompletedSubscribed { get; set; }

        [OperationContract]
        void CardUploadComplete();
    }

    public class DistEventArgs : EventArgs
    {
        public DistEventArgs(long distBatchId, bool success, string responseMessage, Exception processException)
        {
            DistBatchId = distBatchId;
            Success = success;
            ResponseMessage = responseMessage;
            ProcessException = processException;
        }

        public DistEventArgs(long distBatchId, bool success, string responseMessage)
                : this(distBatchId, success, responseMessage, null)
        { }

        public long DistBatchId { get; private set; }
        public bool Success { get; private set; }
        public string ResponseMessage { get; private set; }
        public Exception ProcessException { get; private set; }
    }
}
