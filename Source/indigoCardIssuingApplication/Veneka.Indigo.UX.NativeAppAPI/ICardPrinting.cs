using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Veneka.Indigo.Integration.ProductPrinting;


namespace Veneka.Indigo.UX.NativeAppAPI
{
    [ServiceContract(Namespace = Constants.NativeAppApiUrl)]
    public interface ICardPrinting
    {
        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<PrintJob> GetPrintJob(Token printToken, PrinterInfo printer);

        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<string> PrintFailed(Token printToken,string comments);

        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<string> SendToPrinter(Token printToken);

        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<string> PrintingComplete(Token printToken, CardData cardData);

        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<string> PrinterAuditDetails(Token printToken, PrinterInfo printer);

        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<string> PrinterAnalytics(Token printToken);
    }
}
