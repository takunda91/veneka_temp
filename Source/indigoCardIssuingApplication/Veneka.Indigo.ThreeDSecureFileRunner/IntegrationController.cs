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
using Veneka.Indigo.COMS.DataSource;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.ThreeDSecure.Data;

namespace Veneka.Indigo.ThreeDSecureFileRunner
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
            string auditWorkstation = "3D Secure File Runner";

            //For each issuer
            var issuers = configDal.GetIssuersForInterface(11, null, null, 1, auditUserId, auditWorkstation);

            log.InfoFormat("Running 3D Secure batch registration for {0} issuers", issuers.Count);

            foreach (var issuerId in issuers)
            {
                foreach (var batchRegister in _i3DSecureRegistration)
                {
                    // get config                    
                    var configs = configDal.GetProductInterfaceConfigsByIssuer(issuerId, 11, 1,
                        batchRegister.Metadata.IntegrationGUID, auditUserId, auditWorkstation);

                    foreach (var config in configs)
                    {
                        try
                        {
                            // create batch and get cards
                            var details = dataAccess.GetUnregisteredCards(issuerId,
                                batchRegister.Metadata.IntegrationGUID, false, 0, auditUserId, auditWorkstation);

                            if (details.Count <= 0)
                            {
                                log.InfoFormat("No batches to process.");
                                break;
                            }
                            else
                            {
                                log.InfoFormat("Processing batch {0} with {1} records.", details[0].ThreedsBatchId,
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
                                    ContactEmail = detail.ContactEmail
                                });
                            }

                            string responseMessage;
                            this.create_xml(fileDetails);

                            // run
                            batchRegister.Value.DataSource = localDataSource;
                            batchRegister.Value.BaseFileDir = baseDir;
                            batchRegister.Value.SQLConnectionString = SQLConnection;
                            var resp = batchRegister.Value.Generate3DSecureFiles(fileDetails, config, 0, auditUserId,
                                auditWorkstation, out responseMessage);

                            if (resp)
                            {
                                if (dataAccess.Update3DSecureBatchRegistered(details[0].ThreedsBatchId, 0, auditUserId,
                                    auditWorkstation))
                                {
                                    log.InfoFormat("Done running for issuer id: {0}", issuerId);
                                }
                                else
                                {
                                    log.ErrorFormat("Issue updating batch to registed {0}", details[0].ThreedsBatchId);
                                }
                            }
                            else
                            {
                                log.ErrorFormat("Issuer processing for issuer id: {0} - {1]", issuerId,
                                    responseMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }
                }
            }

            log.Info("Finished 3D Secure batch registration");
        }

        /// <summary>
        /// Gets all batches in recreate status and writes them out.
        /// </summary>
        internal void RunFileRecreate()
        {
            I3DSDataAccess dataAccess = new ThreeDSecure.Data.SQLServer.ThreeDsSQLData(SQLConnection);
            ConfigDAL configDal = new ConfigDAL(SQLConnection);

            long auditUserId = -2;
            string auditWorkstation = "3D Secure File Runner";

            //For each issuer
            var issuers = configDal.GetIssuersForInterface(11, null, null, 1, auditUserId, auditWorkstation);

            log.InfoFormat("Running 3D Secure batch re-create for {0} issuers", issuers.Count);

            foreach (var issuerId in issuers)
            {
                foreach (var batchRegister in _i3DSecureRegistration)
                {
                    // get config                    
                    var configs = configDal.GetProductInterfaceConfigsByIssuer(issuerId, 11, 1,
                        batchRegister.Metadata.IntegrationGUID, auditUserId, auditWorkstation);

                    foreach (var config in configs)
                    {
                        try
                        {
                            // get all recreate batches
                            var batches = dataAccess.GetRecreateBatches(issuerId,
                                batchRegister.Metadata.IntegrationGUID, 0, auditUserId, auditWorkstation);

                            if (batches.Count <= 0)
                            {
                                log.InfoFormat("No batches to re-create.");
                                break;
                            }

                            // for each batch get details then create files
                            foreach (var batch in batches)
                            {
                                log.InfoFormat("Re-creating batch {0} with {1} records.", batch.BatchReference,
                                    batch.NumberOfCards);
                                var cards = dataAccess.GetCards(batch.ThreedsBatchId, false, 0, auditUserId,
                                    auditWorkstation);

                                List<Objects.ThreeDSecureCardDetails> fileDetails =
                                    new List<Objects.ThreeDSecureCardDetails>();

                                foreach (var detail in cards)
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
                                        ContactEmail = detail.ContactEmail
                                    });
                                }

                                string responseMessage;

                                //new function to dump in path
                                this.create_xml(fileDetails);

                                // run
                                batchRegister.Value.DataSource = localDataSource;
                                batchRegister.Value.BaseFileDir = baseDir;
                                batchRegister.Value.SQLConnectionString = SQLConnection;
                                var resp = batchRegister.Value.Generate3DSecureFiles(fileDetails, config, 0,
                                    auditUserId, auditWorkstation, out responseMessage);

                                if (resp)
                                {
                                    if (dataAccess.UpdateBatchStatus(batch.ThreedsBatchId, 1,
                                        "Generated by " + auditWorkstation, 0, auditUserId, auditWorkstation))
                                    {
                                        log.InfoFormat("Done running for issuer id: {0}", issuerId);
                                    }
                                    else
                                    {
                                        log.ErrorFormat("Issue updating batch to registered {0}", batch.BatchReference);
                                    }
                                }
                                else
                                {
                                    log.ErrorFormat("Issue writing file for batch: {0} - {1]", batch.BatchReference,
                                        responseMessage);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }
                }
            }

            log.Info("Finished 3D Secure batch re-create");
        }

        internal void create_xml(List<Objects.ThreeDSecureCardDetails> fileDetails)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");
            settings.CloseOutput = true;
            settings.OmitXmlDeclaration = true;
            string timeStamp = DateTime.Now.ToString("ddMMyyyyHHmmss");
            StreamReader file_path =
                new StreamReader(Path.Combine(_file_path, @"CustomerData"+ timeStamp + ".xml"));
            var finaL_path = file_path.ReadToEnd();
            using (XmlWriter writer = XmlWriter.Create(finaL_path, settings))
            {
                writer.WriteStartElement("root");
                foreach (var data in fileDetails)
                {
                    writer.WriteStartElement("record");
                    writer.WriteElementString("FIO", data.CustomerFirstName + data.CustomerLastName);
                    //writer.WriteElementString("CARD NUMBER", data.CardNumber);
                    writer.WriteElementString("CORADDRESS", data.ContactEmail);
                    writer.WriteElementString("CELLPHONE", data.ContactNumber);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.Flush();
            }
        }
    }
}
