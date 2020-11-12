using Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Veneka.Indigo.COMS.DataSource;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.ThreeDSecure.Data;

namespace Veneka.Indigo.ShowCustomers
{
    internal class IntegrationController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IntegrationController));
        private readonly string baseDir = ConfigurationManager.AppSettings["BaseConfigDir"].ToString();
        private readonly string _file_path = ConfigurationManager.AppSettings["BaseFileDir"].ToString();
        private readonly string SQLConnection;
        private IDataSource localDataSource = null;

        [ImportMany] private Lazy<I3DSecureRegistration, IIntegrationCapabilities>[] _i3DSecureRegistration;

        public IntegrationController()
        {
            using (StreamReader sr = new StreamReader(Path.Combine(baseDir, @"config\Database.config")))
            {
                SQLConnection = sr.ReadToEnd();
            }

            DirectoryCatalog catalog = new DirectoryCatalog(Path.Combine(baseDir, @"integration"), "*.dll");
            var container = new CompositionContainer(catalog);
            container.SatisfyImportsOnce(this);
            localDataSource = new LocalDataSource();

        }

        internal void RunFileProcessor()
        {
            I3DSDataAccess dataAccess = new ThreeDSecure.Data.SQLServer.ThreeDsSQLData(SQLConnection);
            ConfigDAL configDal = new ConfigDAL(SQLConnection);

            long auditUserId = -2;
            string auditWorkstation = "Customer NI Details Runner";

            //For each issuer
            var issuers = configDal.GetIssuersForInterface(1, null, null, 1, auditUserId, auditWorkstation);
            var issuer_id = issuers[0];
            log.InfoFormat("Running File Dump  for {0} issuers", issuers.Count);

           

           
                        try
                        {
                // create batch and get cards
                            var issued_products = dataAccess.GetIssuedProducts();
                if(issued_products.Count <= 0)
                {
                    log.Error("No cards were issued ");
                }
                else
                {
                    foreach (var product_id in issued_products)
                    {
                        // geneate xml file per product 
                        var details = dataAccess.GetUploadedCardsCustomers(product_id);
                        if (details.Count <= 0)
                        {
                            log.InfoFormat("No cards checked out.");

                        }
                        else
                        {
                            log.InfoFormat("Processing batch  with {0} cards.",
                                details.Count);
                        }


                        List<Objects.ThreeDSecureCardDetails> fileDetails =
                            new List<Objects.ThreeDSecureCardDetails>();

                        foreach (var detail in details)
                        {
                            fileDetails.Add(new Objects.ThreeDSecureCardDetails
                            {
                                CardExpiryDate = detail.CardExpiryDate,
                                CardNumber = detail.CardNumber,
                                ContactNumber = detail.ContactNumber,
                                CustoemrMiddleName = detail.CustoemrMiddleName,
                                CustomerFirstName = detail.CustomerFirstName,
                                CustomerLastName = detail.CustomerLastName,
                                CustomerTitle = detail.CustomerTitle,
                                NameOnCard = detail.NameOnCard,
                                ContactEmail = detail.ContactEmail,
                                CustomerAccountNumber = detail.CustomerAccountNumber
                            });
                        }

                        string responseMessage;
                        string issuer_code;
                        string product_prefix;
                        var header_info = dataAccess.GetFileHeaderInfo(product_id, issuer_id);
                      

                        if (this.create_xml(header_info[0].issuer_code,header_info[0].product_prefix,fileDetails))
                            log.InfoFormat("Done running for total of : {0} cards. Check in  {1}", details.Count, _file_path);

                    }
                }
                           

                           

                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
            

            log.Info("Finished Exracting records for XML");
        }

        /// <summary>
        /// Gets all batches in recreate status and writes them out.
        /// </summary>

// #FILE_RECREATE
        internal void RunFileRecreate()
        {
            //I3DSDataAccess dataAccess = new ThreeDSecure.Data.SQLServer.ThreeDsSQLData(SQLConnection);
            //ConfigDAL configDal = new ConfigDAL(SQLConnection);

            //long auditUserId = -2;
            //string auditWorkstation = "3D Secure File Runner";

            ////For each issuer
            //var issuers = configDal.GetIssuersForInterface(11, null, null, 1, auditUserId, auditWorkstation);

            //log.InfoFormat("Running 3D Secure batch re-create for {0} issuers", issuers.Count);

            //foreach (var issuerId in issuers)
            //{
            //    foreach (var batchRegister in _i3DSecureRegistration)
            //    {
            //        // get config                    
            //        var configs = configDal.GetProductInterfaceConfigsByIssuer(issuerId, 11, 1,
            //            batchRegister.Metadata.IntegrationGUID, auditUserId, auditWorkstation);

            //        foreach (var config in configs)
            //        {
            //            try
            //            {
            //                // get all recreate batches
            //                var batches = dataAccess.GetRecreateBatches(issuerId,
            //                    batchRegister.Metadata.IntegrationGUID, 0, auditUserId, auditWorkstation);

            //                if (batches.Count <= 0)
            //                {
            //                    log.InfoFormat("No batches to re-create.");
            //                    break;
            //                }

            //                // for each batch get details then create files
            //                foreach (var batch in batches)
            //                {
            //                    log.InfoFormat("Re-creating batch {0} with {1} records.", batch.BatchReference,
            //                        batch.NumberOfCards);
            //                    var cards = dataAccess.GetCards(batch.ThreedsBatchId, false, 0, auditUserId,
            //                        auditWorkstation);

            //                    List<Objects.ThreeDSecureCardDetails> fileDetails =
            //                        new List<Objects.ThreeDSecureCardDetails>();

            //                    foreach (var detail in cards)
            //                    {
            //                        fileDetails.Add(new Objects.ThreeDSecureCardDetails
            //                        {
            //                            CardExpiryDate = detail.CardExpiryDate,
            //                            CardNumber = detail.CardNumber,
            //                            ContactNumber = detail.ContactNumber,
            //                            CustoemrMiddleName = detail.CustoemrMiddleName,
            //                            CustomerFirstName = detail.CustomerFirstName,
            //                            CustomerLastName = detail.CustomerLastName,
            //                            CustomerTitle = detail.CustomerTitle,
            //                            NameOnCard = detail.NameOnCard,
            //                            ContactEmail = detail.ContactEmail
            //                        });
            //                    }

            //                    string responseMessage;

            //                    //new function to dump in path
            //                    this.create_xml(fileDetails);

            //                    // run
            //                    batchRegister.Value.DataSource = localDataSource;
            //                    batchRegister.Value.BaseFileDir = baseDir;
            //                    batchRegister.Value.SQLConnectionString = SQLConnection;
            //                    var resp = batchRegister.Value.Generate3DSecureFiles(fileDetails, config, 0,
            //                        auditUserId, auditWorkstation, out responseMessage);

            //                    if (resp)
            //                    {
            //                        if (dataAccess.UpdateBatchStatus(batch.ThreedsBatchId, 1,
            //                            "Generated by " + auditWorkstation, 0, auditUserId, auditWorkstation))
            //                        {
            //                            log.InfoFormat("Done running for issuer id: {0}", issuerId);
            //                        }
            //                        else
            //                        {
            //                            log.ErrorFormat("Issue updating batch to registered {0}", batch.BatchReference);
            //                        }
            //                    }
            //                    else
            //                    {
            //                        log.ErrorFormat("Issue writing file for batch: {0} - {1]", batch.BatchReference,
            //                            responseMessage);
            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                log.Error(ex);
            //            }
            //        }
            //    }
            //}

            //log.Info("Finished 3D Secure batch re-create");
        }

        internal bool create_xml(string issuer_code, string product_prefix, List<Objects.ThreeDSecureCardDetails> fileDetails)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");
            settings.CloseOutput = true;
            settings.OmitXmlDeclaration = true;
            string timeStamp = DateTime.Now.ToString("ddMMyyyyHHmmss");
          
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("Root", from item in fileDetails
                                     select

                        new XElement("Record",
                            new XElement("ACCOUNT", item.CustomerAccountNumber),
                            new XElement("CELLPHONE", item.ContactNumber),
                            new XElement("EMAIL", item.ContactEmail)
                            )));

            //FIID_4XXXXX_Update Mobile & Email By AccountNo (1)
            string filename = Path.Combine(_file_path + @"" + issuer_code + "_" + product_prefix + "_Update Mobile & Email By AccountNo" + ".xml");
                xdoc.Save(filename);
            return true;


        }
    }
}
