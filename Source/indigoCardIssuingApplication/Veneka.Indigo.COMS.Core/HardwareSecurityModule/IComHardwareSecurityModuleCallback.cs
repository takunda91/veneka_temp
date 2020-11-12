using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Objects;


namespace Veneka.Indigo.COMS.Core.HardwareSecurityModule
{
    public interface IComHardwareSecurityModuleCallback
    {
        //event EventHandler<PinBatchEventArgs> OnPinPrintCompleted;

        [OperationContract]
        void PrintPinsComplete();
    }

    public class PinBatchEventArgs : EventArgs
    {
        public PinBatchEventArgs(long pinBatchId, bool success, string responseMessage, Exception processException)
        {
            PinBatchId = pinBatchId;
            Success = success;
            ResponseMessage = responseMessage;
            ProcessException = processException;
        }

        public PinBatchEventArgs(long pinBatchId, bool success, string responseMessage)
                : this(pinBatchId, success, responseMessage, null)
        { }

        public long PinBatchId { get; private set; }
        public bool Success { get; private set; }
        public string ResponseMessage { get; private set; }
        public Exception ProcessException { get; private set; }
    }
}
