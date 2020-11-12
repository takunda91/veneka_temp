using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.BackOffice.ApplicationTests.BackOfficeAPI;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Veneka.Indigo.Security;

namespace Veneka.Indigo.BackOffice.ApplicationTests
{
    /// <summary>
    /// Summary description for BackofficeAPITests
    /// </summary>
    [TestClass]
    public class BackofficeAPITests
    {
        public BackofficeAPITests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void LoginTest()
        {
            BackOfficeAPI.BackOfficeAPIClient client = new BackOfficeAPIClient();
            IgnoreUntrustedSSL();
           
            var response= client.Login("tmb_branch_prod_man", "Jungle01");
            string AuthToken=response.AuthToken;
            if (response.ResponseCode == "00")
            {
               var _p= client.GetApprovedPrintBatches(AuthToken);
                PrintBatchData[] list=_p.Value;

                var _p1 = client.GetRequestsforBatch(int.Parse(list[0].PrintBatchId.ToString()),1,20,AuthToken);
                RequestDetails[] _data = _p1.Value;

                UpdatePrintBatchDetails _pbdata=new UpdatePrintBatchDetails();
                _pbdata.PrintBatchStatusId= list[0].PrintBatchStatusId;
                _pbdata.Successful=true;
                _pbdata.PrintBatchId= int.Parse(list[0].PrintBatchId.ToString());
               
                List<RequestDetails> _requestDetails =new List<RequestDetails>();
                _requestDetails.Add(new RequestDetails() { PAN=_data[0].PAN,RequestReference=_data[0].RequestReference,RequestId=_data[0].RequestId,RequestStatuesId=_data[0].RequestStatuesId } );
                _pbdata.RequestDetails=_requestDetails.ToArray();
                _pbdata.Cardstobespoiled=(new List<string>()).ToArray();
                var response1 = client.updatePrintBatchStatus(_pbdata,AuthToken);
               

            }

        }

        private void IgnoreUntrustedSSL()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)3072 | SecurityProtocolType.Ssl3;

            ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate,
                                                                        X509Chain chain,
                                                                        SslPolicyErrors sslPolicyErrors) => true;
        }
    }

    internal class SecurityParameters
    {

    }
}
