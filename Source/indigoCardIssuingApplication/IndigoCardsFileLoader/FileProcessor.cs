using System;
using System.Collections.Generic;
using Veneka.Indigo.Common.Utilities;
using IndigoFileLoader.bll;
using IndigoFileLoader.dal;
using IndigoFileLoader.objects;
using Veneka.Indigo.Common.Models;
using Common.Logging;

namespace IndigoFileLoader
{
    public class FileProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FileProcessor));
        //private readonly FileLoaderService _fileLoader = new FileLoaderService();
        private readonly IssuerConfigDAL configDal = new IssuerConfigDAL();

        public bool ProcessCardsFiles()
        {            
            //List<issuer> configs = configDal.GetIssuerConfiguration();
            //log.Debug( m => m("Number fo issuers to process: " + configs.Count));

            //foreach (issuer config in configs)
            //{
            //    log.Info(string.Format("Start processing files for issuer {0}({1}).", new object[] { config.issuer_name, config.issuer_code }));

            //    try
            //    {
            //        //check what functions are being used
            //        if (config.instant_card_issue_YN)
            //        {
                        //Check if the issuer has been licensed
                        //if (!String.IsNullOrWhiteSpace(config.license_key))
                        //{
                            //Validate Key, and get valid bins.
                           // var licensedBinCodes = Veneka.Indigo.Common.License.LicenseManager.ValidateAffiliateKey(config.license_key);


                                log.Debug(m => m("Process load file/s..."));
                                FileLoaderBLL fileLoader = new FileLoaderBLL();
                                //load card file for instant issue
                                fileLoader.LoadCardFile();
                            //}
                            //else
                            //{
                            //    log.Info(m => m("Issuer license invalid... Skipping issuer."));
                            //}
                        //}
                        //else
                        //{
                        //    log.Info(m => m("Issuer not licensed... Skipping issuer."));
                        //}
                //    }

                //    if (config.pin_mailer_printing_YN)
                //    {
                //        log.Debug(m => m("Process PIN file/s..."));
                //        //load pin file for instant issue
                //        //LoadPinFile(issuer);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    log.Error(ex);                    
                //}

                //log.Info(string.Format("Completed processing files for issuer {0}({1}).", new object[] { config.issuer_name, config.issuer_code }));               
            //}
            return true;
        }
    }
}