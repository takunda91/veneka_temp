using Common.Logging;
using IndigoCardIssuanceService.bll;
using IndigoCardIssuanceService.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;
using Veneka.Indigo.BackOffice.API;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.Security;
using Veneka.Indigo.ServicesAuthentication.API.DataContracts;

namespace IndigoCardIssuanceService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BackOfficeAPI" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select BackOfficeAPI.svc or BackOfficeAPI.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(Namespace = Constants.BackOfficeApiUrl)]
    [AspNetCompatibilityRequirements(
        RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class BackOfficeAPI : IBackOfficeAPI
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(BackOfficeAPI));
        private readonly IndigoCardIssuanceService.bll.Action _Action = IndigoCardIssuanceService.bll.Action.PrintCard;

        private readonly IssuerManagementController _issuerManController = new IssuerManagementController();
        private readonly DistributionBatchController _distManController = new DistributionBatchController();
        private readonly IssueCardController _issueCardController = new IssueCardController();


        


        public Response<string> CheckStatus(string guid)
        {
            throw new NotImplementedException();
        }

        public int CheckStatusRest(string guid)
        {
            try
            {
                Guid sessionGuid;
                if (Guid.TryParse(guid, out sessionGuid))
                {

                    var status = BackOfficeAPIController.CheckStatusSession(sessionGuid);
                    _log.Debug(d => d("Status for " + guid + " is " + status));

                    return status;
                }
                else
                {
                    _log.Warn("GUID Format incorrect.");
                    HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.StatusCode = 500;
                    throw new HttpException(500, "GUID format incorrect");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.StatusCode = 500;
                throw new HttpException(500, ex.Message);
            }
        }

        public Response<List<GetPrintBatchDetails>> GetApprovedPrintBatches(string token)
        {
            string message = String.Empty;

            try
            {
                List<GetPrintBatchDetails> _list = new List<GetPrintBatchDetails>();
                Guid checkGuid; string sessionKey; long userId; string nextToken;
                if (BackOfficeAPIController.ValidateToken(token, _Action, 0, out checkGuid, out sessionKey, out userId, out nextToken))
                {

                    var result = _distManController.GetPrintBatchesForUser(null,null,null,1,null,1,null,null,0,20,1, userId, "test");
                    foreach (var item in result.Value)
                    {
                        _list.Add(new GetPrintBatchDetails() { PrintBatchId = item.print_batch_id, PrintBatchReference=item.print_batch_reference, ProductId = item.product_id, PrintBatchStatusId = item.print_batch_statuses_id, CreatedDateTime = (DateTime)item.date_created, NoOfRequests = (int)item.no_cards });
                    }
                    return new Response<List<GetPrintBatchDetails>>(true, message, _list, nextToken);
                }
                else
                {
                    message = "Invalid Token";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _log.Error(ex);
            }

            return new Response<List<GetPrintBatchDetails>>(false, message, null, "");
        }
        
            public Response<List<ProductTemplate>> GetProductTemplate(int productid, string token)
        {

            string message = String.Empty;

            try
            {
                List<Veneka.Indigo.BackOffice.API.ProductTemplate> _list = new List<Veneka.Indigo.BackOffice.API.ProductTemplate>();
                Guid checkGuid; string sessionKey; long userId; string nextToken;
                if (BackOfficeAPIController.ValidateToken(token, _Action, 0, out checkGuid, out sessionKey, out userId, out nextToken))
                {
                    var result = _issueCardController.GetPrintFieldsByProductid(productid);

                    foreach (var j in result.Value)
                    {
                        _list.Add(new ProductTemplate() { ProductPrintFieldId = j.ProductPrintFieldId, productPrintFieldTypeId = j.ProductPrintFieldTypeId, font_size = j.FontSize });
                    }


                    return new Response<List<Veneka.Indigo.BackOffice.API.ProductTemplate>>(true, message, _list, nextToken);
                }
                else
                {
                    message = "Invalid Token";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _log.Error(ex);
            }

            return new Response<List<Veneka.Indigo.BackOffice.API.ProductTemplate>>(false, message, null, "");
        }

        public Response<List<Veneka.Indigo.BackOffice.API.RequestDetails>> GetRequestsforBatch(long print_batch_id, int startindex, int size, string token)
        {
            string message = String.Empty;

            try
            {
                List<Veneka.Indigo.BackOffice.API.RequestDetails> _list = new List<Veneka.Indigo.BackOffice.API.RequestDetails>();
                Guid checkGuid; string sessionKey; long userId; string nextToken;
                if (BackOfficeAPIController.ValidateToken(token, _Action, 0, out checkGuid, out sessionKey, out userId, out nextToken))
                {

                    var result = _distManController.GetPrintBatchRequests(print_batch_id, startindex, size, userId,"API");
                    foreach (var item in result.Value)
                    {
                        Veneka.Indigo.BackOffice.API.RequestDetails _request_result = new Veneka.Indigo.BackOffice.API.RequestDetails() { RequestId = item.request_id, RequestReference = item.request_reference,RequestStatuesId=item.hybrid_request_statuses_id };
                        _request_result.ProdTemplateList = new List<ProductTemplate>();

                        foreach (var j in item.ProductFields)
                        {
                            _request_result.ProdTemplateList.Add(new ProductTemplate() { ProductPrintFieldId = j.ProductPrintFieldId,  Value = j.Value,  font = j.Font, fontColourRGB = j.FontColourRGB.ToString(), font_size = j.FontSize, x = j.X, y = (float)((j.Y)*(2.94)), productPrintFieldTypeId = j.ProductPrintFieldTypeId, PrintSide = j.PrintSide });
                        }
                        _list.Add(_request_result);
                    }

                    return new Response<List<Veneka.Indigo.BackOffice.API.RequestDetails>>(true, message, _list, nextToken);
                }
                else
                {
                    message = "Invalid Token";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _log.Error(ex);
            }

            return new Response<List<Veneka.Indigo.BackOffice.API.RequestDetails>>(false, message, null, "");
        }

        public Response<bool> updatePrintBatchStatus(UpdatePrintBatchDetails _printBatch, string token)
        {
            string message = String.Empty;

            try
            {

                Guid checkGuid; string sessionKey; long userId; string nextToken;
                if (BackOfficeAPIController.ValidateToken(token, _Action, 0, out checkGuid, out sessionKey, out userId, out nextToken))
                {
                    List<RequestData> targetList = _printBatch.RequestDetails
                                                  .Select(x => new RequestData() { request_id = x.RequestId, card_number = x.PAN, request_statues_id = x.RequestStatuesId ==null? 0 : (int)x.RequestStatuesId })
                      
                                                  .ToList();

                    var response= _distManController.UpdatePrintBatchRequestsStatus((long)_printBatch.PrintBatchId, (int)_printBatch.PrintBatchStatusId, _printBatch.Successful, targetList, _printBatch.Cardstobespoiled, _printBatch.notes, 0, userId, "API");
                    if (response.Value) 
                    return new Response<bool>(true, message, true, nextToken);


                }
                else
                {
                    message = "Invalid Token";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _log.Error(ex);
            }
            return new Response<bool>(true, message, false, "");
        }

    

        public Response<string[]> GetWorkStationKey(string WorkStation,int size,string token)
        {
            string message = String.Empty;

            try
            {

                Guid checkGuid; string sessionKey; long userId; string nextToken;
                if (BackOfficeAPIController.ValidateToken(token, _Action, 0, out checkGuid, out sessionKey, out userId, out nextToken))
                {


                    var response = _distManController.GetWorkStationKey(WorkStation,size);
                    if (response !=null)
                        return new Response<string[]>(true, message, response.Value, nextToken);


                }
                else
                {
                    message = "Invalid Token";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _log.Error(ex);
            }
            return new Response<string[]>(false, message, null, "");
        }
    }
}
