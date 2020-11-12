using Common.Logging;
using IndigoCardIssuanceService.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Models;

namespace IndigoCardIssuanceService.bll
{
    public class PrintJobController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PrintJobController));
        private readonly PrintJobManagementService _printjobman;

        public PrintJobController()
        {
            _printjobman = new PrintJobManagementService();
        }

        public PrintJobController(IPrintJobManagementDAL printmanagement, IResponseTranslator responseTranslator)
        {
            _printjobman = new PrintJobManagementService(printmanagement,responseTranslator);
        }

        public Response<PrintJobStatus> GetPrintJobStatus(string printJobId,int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
               
                return new Response<PrintJobStatus>(_printjobman.GetPrintJobStatus(printJobId, languageId, auditUserId, auditWorkstation), ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<PrintJobStatus>(null, ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        public Response<string> InsertPrintJob(string serialNo, long customerId,long cardId, int printJobStatusesId, long auditUserId, string auditWorkstation)
        {
            try
            {
                var response = _printjobman.InsertPrintJob(serialNo, customerId, cardId, printJobStatusesId, auditUserId, auditWorkstation, out string printJobId);
                if (response== SystemResponseCode.SUCCESS)
                {
                    return new Response<string>(printJobId, ResponseType.SUCCESSFUL, "Action Was Successful.", "");
                }

                return new Response<string>(null,ResponseType.UNSUCCESSFUL,"", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<string>(null,ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        public BaseResponse UpdatePrintCount(string serialNo,int totalPrints, long auditUserId, string auditWorkstation)
        {
            try
            {

                var response = _printjobman.UpdatePrintCount(serialNo, totalPrints,  auditUserId, auditWorkstation);
                if (response == SystemResponseCode.SUCCESS)
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, "Printer Inserted Successfully.", "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        public BaseResponse UpdatePrintJobStatus(string printerJobId, int printJobStatusesId,string comments, long auditUserId, string auditWorkstation)
        {
            try
            {

                var response = _printjobman.UpdatePrintJobStatus(printerJobId, printJobStatusesId, comments, auditUserId, auditWorkstation);
                if (response == SystemResponseCode.SUCCESS)
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, "Printer Inserted Successfully.", "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        public BaseResponse PrintingCompleted(string encryptedCardNumber, long cardId, string printJobId,int printJobStatusesId, long auditUserId, string auditWorkstation)
        {
            try
            {              

                // updating 
                var response = _printjobman.UpdateCardDetails(encryptedCardNumber, cardId, auditUserId, auditWorkstation);
                if (response == SystemResponseCode.SUCCESS)
                {
                    // updating printjob status to printed.
                    _printjobman.UpdatePrintJobStatus(printJobId, printJobStatusesId,"Print Completed.", auditUserId, auditWorkstation);
                    return new BaseResponse(ResponseType.SUCCESSFUL, "Printer Inserted Successfully.", "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        public BaseResponse RegisterPrinter(Printer printer,string printJobId ,long auditUserId, string auditWorkstation)
        {
            try
            {

                var response = _printjobman.RegisterPrinter(printer , printJobId, auditUserId, auditWorkstation);
                if (response == SystemResponseCode.SUCCESS)
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, "Printer Inserted Successfully.", "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

    }
}