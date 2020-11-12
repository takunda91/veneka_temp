using System;
using System.Collections.Generic;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.CardManagement.dal
{
  public  interface IPrintJobManagementDAL
    {
        SystemResponseCode RegisterPrinter(Printer printer,string printJobId,long auditUserId, string auditWorkstation);

        SystemResponseCode UpdatePrintCount(string serialNo, int totalPrints, long auditUserId, string auditWorkstation);

      PrintJobStatus   GetPrintJobStatus(string printJobId,int languageId, long auditUserId, string auditWorkstation);

        SystemResponseCode InsertPrintJob(string serialNo, long customerId,long cardId,int printJobStatusesId, long auditUserId, string auditWorkstation, out string PrintJobId);

        SystemResponseCode UpdatePrintJobStatus(string printerJobId,int printJobStatusesId, string comments, long auditUserId, string auditWorkstation);

        SystemResponseCode UpdateCardDetails(string cardNumber,long cardId, long auditUserId, string auditWorkstation);
    }
}
