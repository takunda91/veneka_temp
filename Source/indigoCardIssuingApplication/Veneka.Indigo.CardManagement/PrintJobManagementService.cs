using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.Security;

namespace Veneka.Indigo.CardManagement
{
   public class PrintJobManagementService 
    {
        private readonly IPrintJobManagementDAL _printjobDAL ;
        private readonly IResponseTranslator _translator ;

        public PrintJobManagementService()
        {
            _printjobDAL = new PrintJobManagementDAL();
            _translator = new ResponseTranslator();

        }
        public PrintJobManagementService(IPrintJobManagementDAL printmanagement,IResponseTranslator responseTranslator)
        {
            _printjobDAL =  printmanagement;
            _translator = responseTranslator;

        }
        public SystemResponseCode InsertPrintJob(string serialNo, long customerId,long cardId, int printJobStatusesId, long auditUserId, string auditWorkstation,out string printJobId)
        {
          return  _printjobDAL.InsertPrintJob(serialNo, customerId, cardId, printJobStatusesId, auditUserId, auditWorkstation, out printJobId);
        }
        public PrintJobStatus GetPrintJobStatus(string printJobId,int languageId, long auditUserId, string auditWorkstation)
        {
            return _printjobDAL.GetPrintJobStatus(printJobId, languageId, auditUserId, auditWorkstation);

        }

        public SystemResponseCode RegisterPrinter(Printer printer,string printJobId, long auditUserId, string auditWorkstation)
        {
            return _printjobDAL.RegisterPrinter(printer, printJobId ,auditUserId, auditWorkstation);
        }

        public SystemResponseCode UpdatePrintCount(string serialNo, int totalprints, long auditUserId, string auditWorkstation)
        {
            return _printjobDAL.UpdatePrintCount(serialNo, totalprints, auditUserId, auditWorkstation);
        }

        public SystemResponseCode UpdatePrintJobStatus(string printerJobId, int printJobStatusesId,string comments, long auditUserId, string auditWorkstation)
        {

            return _printjobDAL.UpdatePrintJobStatus(printerJobId, printJobStatusesId,  comments, auditUserId, auditWorkstation);
        }

        public SystemResponseCode UpdateCardDetails(string encryptedCardNumber, long cardId, long auditUserId, string auditWorkstation)
        {

            string clearPAN = EncryptionManager.DecryptString(encryptedCardNumber,
                                                                       StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                       StaticFields.EXTERNAL_SECURITY_KEY);
            return _printjobDAL.UpdateCardDetails(clearPAN, cardId, auditUserId, auditWorkstation);
        }
    }
}
