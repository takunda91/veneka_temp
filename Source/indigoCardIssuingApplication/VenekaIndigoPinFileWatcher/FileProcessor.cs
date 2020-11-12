using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.SqlServer.Server;
using Veneka.Indigo.Common;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.FileLoader;

namespace VenekaIndigoPinFileWatcher
{
    public class FileProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FileProcessor));
        private readonly string fileDirectory = ConfigurationManager.AppSettings["PathToFtp"].ToString();
        private readonly string baseDir = ConfigurationManager.AppSettings["BaseConfigDir"].ToString();
        private readonly string SQLConnection;
        private IDataSource localDataSource;

        public FileProcessor()
        {
            using (StreamReader sr = new StreamReader(Path.Combine(baseDir, @"config\Database.config")))
            {
                SQLConnection = sr.ReadToEnd();
            }

        }

        public void readPinFile(string name)
        {
            Random rnd = new Random();
            int myRandomNo = rnd.Next(10000000, 99999999);
            var header_ref = myRandomNo.ToString();
            string[] allFiles = System.IO.Directory.GetFiles(fileDirectory);
            foreach (string file in allFiles)
            {
                if (file.Contains(name))
                {
                    log.Info("File Found : " + file);

                    string[] lines = File.ReadAllLines(file);
                    if(name == "MAILER")
                    {
                      var mailer_response =  process_mailer(lines, header_ref);
                        if (mailer_response)
                        {
                            // create header here
                           
                            log.InfoFormat("File {0} has been uploaded to successfully...", file);
                        }
                        else
                        {
                            log.WarnFormat("No data was loaded for file {0}",file);
                        }
                    }
                    else if(name == "LINK")
                    {
                        var link_resp = process_link(lines);

                        if (link_resp)
                        {
                            log.InfoFormat("File {0} has been uploaded to successfully...", file);
                        }
                        else
                        {
                            log.WarnFormat("No data was loaded for file {0}", file);
                        }
                    }
               
                    File.Delete(file);
                    log.InfoFormat("File Type: {0}, Name: {1} has been uploaded and deleted . ", name, file);

                }
              //  else
                //  log.ErrorFormat("No {0} file has been found ", name );
            }

            //   Console.ReadLine();
            log.Info("End of File load ..... ");
        }

        private string FilePathLocation()
        {
            string value = String.Empty;
            string text = String.Empty;
            try
            {
                value = ConfigurationManager.AppSettings["PathToFtp"].ToString();
                text = String.Format("{0}{1}", value, @"\cardname.txt");


            }
            catch (Exception e)
            {
                //mplement logging in the future
                Console.WriteLine(e);

            }

            return text;



        }

        internal bool create_pin_mailer(string pin_block, string mask_pan, string product_name, string product_bin_code, string pan_last_four, DateTime expiry_period, string card_id, string header_ref)
        {
            ConfigDAL configDal = new ConfigDAL(SQLConnection);
            var response = configDal.usd_create_pin_mailer(pin_block, mask_pan, product_name, product_bin_code, pan_last_four, expiry_period, card_id, header_ref);

            if (response == SystemResponseCode.SUCCESS)
            { 
            var create_header = this.create_batch_header(header_ref);
                if (create_header)
                {
                    return true;
                }
                else
                {
                    log.ErrorFormat("failed to create header for batch ref {0}...", create_header);
                    return false;
                }
       
            }
            else if(response == SystemResponseCode.FILE_PARAMETER_ALREADY_IN_USE)
            {
                log.ErrorFormat("Failed to log data for card id {0}. Error: Duplicate Card id..." ,card_id);
                return false;
            }
            return false;
        }

        internal bool process_mailer(string[] lines, string header_ref)
        {
            try
            {

                var number_of_lines = lines.Length;
                var resp = false;
                for (int i = 0; i < number_of_lines; i++)
                {
                    string pin_block = String.Empty;
                    string masked_pan = String.Empty;
                    string product_name = String.Empty;
                    DateTime expiry_date;
                    string card_id = String.Empty;
                    string product_bin_code = String.Empty;
                    string last_four_of_pan = String.Empty;


                    if (i == 0 || i == number_of_lines - 1)
                        continue;
                    string[] col = lines[i].Split('|');
                    log.InfoFormat(String.Format("Pin Block: {0}, Masked Pan: {1}, Product Name: {2}, Exp Date: {3}, Card ID: {4}", col[20], col[5], col[4], col[19], col[6]));
                    pin_block = col[20];
                    masked_pan = col[5];
                    product_name = col[4];
                    expiry_date =  DateTime.ParseExact(col[19], "dd/MM/yyyy", CultureInfo.InvariantCulture); ;
                    card_id = col[6];

                    int index = masked_pan.IndexOf('*');
                    product_bin_code = masked_pan.Substring(0, index);

                    last_four_of_pan = masked_pan.Substring(masked_pan.LastIndexOf('*') + 1);

                    resp = create_pin_mailer(pin_block, masked_pan, product_name, product_bin_code, last_four_of_pan, expiry_date, card_id, header_ref);

                }

                if (resp)
                {
                    return true;
                }
                return false;
            }
            catch(ArgumentOutOfRangeException exc)
            {
                log.Error(exc);
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return false;
            }
        }

        // process link
        internal bool process_link(string[] lines)
        {
            try
            {

                var number_of_lines = lines.Length;
                var resp = false;
                for (int i = 0; i < number_of_lines; i++)
                {
                    
                    string card_id = String.Empty;
                    string dummy_pan = String.Empty;

                    //if (i == 0 || i == number_of_lines - 1)
                    //    continue;
                    string[] col = lines[i].Split('|');
                    log.InfoFormat(String.Format("Card id: {0}, Dummy Account Number: {1}", col[0], col[1]));
                  
                    card_id = col[0];
                    dummy_pan = col[1];

                    resp = create_pin_link(card_id, dummy_pan);

                }

                if (resp)
                {
                    return true;
                }
                return false;
            }
            catch (ArgumentOutOfRangeException exc)
            {
                log.Error(exc);
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return false;
            }
        }

        internal bool create_pin_link(string card_id, string dummy_pan)
        {
            ConfigDAL configDal = new ConfigDAL(SQLConnection);
            var response = configDal.usd_create_pin_link( card_id, dummy_pan);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }
            else if (response == SystemResponseCode.FILE_PARAMETER_ALREADY_IN_USE)
            {
                log.ErrorFormat("Failed to log data for card id {0}. Error: Duplicate Card id...", card_id);
                return false;
            }
            return false;
        }

        internal bool create_batch_header(string header_reference)
        {
            ConfigDAL configDal = new ConfigDAL(SQLConnection);
            var response = configDal.usp_create_pin_file_batch_header(header_reference);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }
            else if (response == SystemResponseCode.FILE_PARAMETER_ALREADY_IN_USE)
            {
               // log.ErrorFormat("Failed to log data for card id {0}. Error: Duplicate Card id...", card_id);
                return false;
            }
            return false;
        }


    }
}
