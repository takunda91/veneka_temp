using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.CardManagement.dal
{
   public class PrintJobManagementDAL : IPrintJobManagementDAL
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        public PrintJobManagementDAL (DatabaseConnectionObject connectionObject)
        {
            _dbObject = connectionObject;
        }
        public PrintJobManagementDAL()
        {
           
        }
        public SystemResponseCode InsertPrintJob(string serialNo, long customerId, long cardId, int printJobStatusesId, long auditUserId, string auditWorkstation, out string PrintJobId)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            ObjectParameter print_job_id = new ObjectParameter("print_job_id", typeof(int));
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_create_print_jobs(serialNo, cardId, customerId, printJobStatusesId, auditUserId,auditWorkstation, print_job_id,ResultCode);
            }
            PrintJobId = print_job_id.Value.ToString();
            return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
        }

        public PrintJobStatus GetPrintJobStatus(string printJobId,int languageId, long auditUserId, string auditWorkstation)
        {        
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
             var result =  context.usp_get_print_job_status(printJobId, languageId, auditUserId, auditWorkstation);

                foreach(PrintJobStatus item in result)
                {
                    return item;
                }
            }
            return null;
        }
        public SystemResponseCode RegisterPrinter(Printer printer,string printJobId, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            ObjectParameter PrinterId = new ObjectParameter("new_printer_id", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_insert_printer(printer.SerialNo, printer.Manufacturer,printer.Model,printer.FirmwareVersion, printer.BranchId, printer.TotalPrints,printer.NextClean,long.Parse(printJobId), auditUserId, auditWorkstation, PrinterId, ResultCode);
            }
            return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
        }

        public SystemResponseCode UpdateCardDetails(string cardNumber, long cardId, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_update_card_details(cardId, cardNumber, auditUserId, auditWorkstation);
            }
            return SystemResponseCode.SUCCESS;
        }

        public SystemResponseCode UpdatePrintCount(string serialNo, int totalPrints, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_update_printer_prints(serialNo, totalPrints.ToString(), auditUserId, auditWorkstation, ResultCode);
            }
            return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
        }

        public SystemResponseCode UpdatePrintJobStatus(string printerJobId, int printJobStatusesId, string comments, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_update_print_jobs_status(long.Parse(printerJobId), printJobStatusesId, comments, auditUserId, auditWorkstation);
            }
            return SystemResponseCode.SUCCESS;
        }
    }
}
