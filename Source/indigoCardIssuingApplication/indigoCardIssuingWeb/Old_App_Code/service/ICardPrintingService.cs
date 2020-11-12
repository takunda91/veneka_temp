using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoCardIssuingWeb.Old_App_Code.service
{
    public interface ICardPrintingService
    {
        void CreatePrintJob(long customerId);
        void GetPrintJob(long printJobId);
        void CompletePrintJob(long printJobId);
    }
}
