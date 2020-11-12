using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.BackOffice.API;
using Veneka.Indigo.ServicesAuthentication.API.DataContracts;

namespace Veneka.Indigo.BackOffice.Application.Testing
{
    public class TestBackOfficeAPI : IBackOfficeAPI
    {
        public Response<string> CheckStatus(string guid)
        {
            throw new NotImplementedException();
        }

        public Response<List<GetPrintBatchDetails>> GetApprovedPrintBatches(string token)
        {
            var resp = new Response<List<GetPrintBatchDetails>>();
            resp.Success = true;

            var values = new List<GetPrintBatchDetails>
            {
                new GetPrintBatchDetails{NoOfRequests = 10, PrintBatchReference="test1", PrintBatchId = 0, ProductId = 1 },
                new GetPrintBatchDetails{NoOfRequests = 20,PrintBatchReference="test2", PrintBatchId = 1, ProductId = 2 },
                new GetPrintBatchDetails{NoOfRequests = 30,PrintBatchReference="test3", PrintBatchId = 2, ProductId = 3 },
                new GetPrintBatchDetails{NoOfRequests = 40, PrintBatchReference="test4",PrintBatchId = 3, ProductId = 4 },
                new GetPrintBatchDetails{NoOfRequests = 50,PrintBatchReference="test5", PrintBatchId = 4, ProductId = 5 }
            };

            resp.Value = values;
            return resp;
        }

        public Response<List<ProductTemplate>> GetProductTemplate(int productId, string token)
        {
            throw new NotImplementedException();
        }

        public Response<List<RequestDetails>> GetRequestsforBatch(long printBatchId, int startIndex, int size, string token)
        {

            var resp = new Response<List<RequestDetails>>();
            resp.Success = true;
            var values = new List<RequestDetails>
            {
                //new RequestDetails{RequestId = 1, RequestStatuesId = 3, RequestReference="Request1", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test") } } },
                //new RequestDetails{RequestId = 2, RequestStatuesId = 3,RequestReference="Request2", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test1") } } },
                //new RequestDetails{RequestId = 3, RequestStatuesId = 3,RequestReference="Request3", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test1") } } },
                //new RequestDetails{RequestId = 4, RequestStatuesId = 3,RequestReference="Request4", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test2") } } },
                //new RequestDetails{RequestId = 5, RequestStatuesId = 3,RequestReference="Request5", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test3") } } },
                //new RequestDetails{RequestId = 6, RequestStatuesId = 3,RequestReference="Request6", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test4") } } },
                //new RequestDetails{RequestId = 7, RequestStatuesId = 3,RequestReference="Request7", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test5") } } },
                //new RequestDetails{RequestId = 8, RequestStatuesId = 3,RequestReference="Request8", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test6") } } },
                //new RequestDetails{RequestId = 9, RequestStatuesId = 3,RequestReference="Request9", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test6") } } },
                //new RequestDetails{RequestId = 10, RequestStatuesId = 3,RequestReference="Request10", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test6") } } },
                //new RequestDetails{RequestId = 11, RequestStatuesId = 3, RequestReference="Request11", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test") } } },
                //new RequestDetails{RequestId = 12, RequestStatuesId = 3,RequestReference="Request12", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test1") } } },
                //new RequestDetails{RequestId = 13, RequestStatuesId = 3,RequestReference="Request13", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test1") } } },
                //new RequestDetails{RequestId = 14, RequestStatuesId = 3,RequestReference="Request14", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test2") } } },
                //new RequestDetails{RequestId = 15, RequestStatuesId = 3,RequestReference="Request15", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test3") } } },
                //new RequestDetails{RequestId = 16, RequestStatuesId = 3,RequestReference="Request16", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test4") } } },
                //new RequestDetails{RequestId = 17, RequestStatuesId = 3,RequestReference="Request17", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test5") } } },
                //new RequestDetails{RequestId = 18, RequestStatuesId = 3,RequestReference="Request18", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test6") } } },
                //new RequestDetails{RequestId = 19, RequestStatuesId = 3,RequestReference="Request19", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test6") } } },
                //new RequestDetails{RequestId = 20, RequestStatuesId = 3,RequestReference="Request20", ProdTemplateList = new List<ProductTemplateResult>(){ new ProductTemplateResult {ProductPrintFieldId=1,Value= System.Text.ASCIIEncoding.ASCII.GetBytes("test6") } } }

            };

            
            resp.Value = values.GetRange(startIndex-1, size);
            return resp;
        }

      
        public Response<string[]> GetWorkStationKey(string workStation, int size, string token)
        {
            throw new NotImplementedException();
        }

        public Response<bool> InsertWorkStationKey(string workStation, string key, string token)
        {
            throw new NotImplementedException();
        }

        public AuthenticationResponse Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public AuthenticationResponse MultiFactor(int type, string mfToken, string authToken)
        {
            throw new NotImplementedException();
        }

        public Response<bool> updatePrintBatchStatus(UpdatePrintBatchDetails printBatch, string token)
        {
            return new Response<bool>(true, "", true, token);
        }
    }
}
