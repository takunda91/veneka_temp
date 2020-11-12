using Common.Logging;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Renewal.dal;
using Veneka.Indigo.Renewal.Entities;

namespace Veneka.Indigo.Renewal.Integration.Receive.TMB
{
    public class ReceiveResponse : IReceiveResponse
    {
        ILog logger = LogManager.GetLogger("Veneka.Indigo.Renewal.Importer");

        string positionCardHeader = "cardnumber";
        string positionEmbosingName = "embossingname";
        string positionCustomerName = "customername";
        string positionBranchHeader = "branch";
        string positionProductHeader = "product";
        string positionBranchTotalText = "branchtotal";
        string positionProductTotalText = "producttotal";

        public List<RenewalResponseDetail> ExtractFile(string sourceFolder)
        {
            List<RenewalResponseDetail> incomingData = new List<RenewalResponseDetail>();
            List<string> createdFiles = new List<string>();
            if (!Directory.Exists(sourceFolder))
            {
                throw new Exception("Source folder is not configured or accessible.");
            }

            string[] files = Directory.GetFiles(sourceFolder);

            foreach (var file in files)
            {
                incomingData.AddRange(ExtractRenewalFile(file));
                
                ArchiveFile(sourceFolder, file);
            }
            return incomingData;
        }


        private bool UselessRow(string firstMan, string eightMan)
        {
            if (string.IsNullOrWhiteSpace(firstMan))
            {
                return true;
            }

            string firstManText = firstMan.Trim().ToLower().Replace(" ", "");
            if (firstManText == positionBranchHeader)
            {
                if (string.IsNullOrWhiteSpace(eightMan))
                {
                    return true;
                }
            }

            List<string> exceptionList = new List<string>() { "cardsreadyforrenewal", "(detail)", "grandtotal", "total" };
            if (exceptionList.Contains(firstManText))
            {
                return true;
            }
            return false;
        }

        public void ArchiveFile(string baseDir, string filename)
        {
            try
            {
                string strTemp = string.Format("Moving file {0} to archive", filename);
                logger.Info(strTemp);
                string dir = $"{baseDir}\\archive";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                DirectoryInfo di = new DirectoryInfo(filename);

                string strDestDir = $"{dir}\\{DateTime.Today.ToString("yyyy_MM")}\\";
                if (!Directory.Exists(strDestDir))
                {
                    Directory.CreateDirectory(strDestDir);
                }
                Directory.Move(filename, $"{strDestDir}{DateTime.Now.ToString("yyyyMMdd_hhmmss")}_{di.Name}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<RenewalResponseDetail> ExtractRenewalFile(string file)
        {
            logger.Trace($"Extracting File {file}");
            List<RenewalResponseDetail> details = new List<RenewalResponseDetail>();

            try
            {

                FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read);

                var excelReader = file.ToLower().Contains(".xlsx") ? ExcelReaderFactory.CreateOpenXmlReader(stream) : ExcelReaderFactory.CreateBinaryReader(stream);
                DataTable worksheet = excelReader.AsDataSet().Tables[0];

                int branchCount = 0;
                int cardCount = 0;

                bool positionFinderCardHeaderFound = false;
                bool positionFinderEmbossingHeaderFound = false;
                bool positionFinderCustomerNameFound = false;
                bool positionFinderBranchFound = false;
                bool positionFinderProductFound = false;
                bool positionFinderBranchTotalFound = false;

                string branchCode;
                string branchName;
                string productName;

                int branchId = 0;
                int productId = 0;
                int currentCardRow = 0;
                bool endofPage = false;

                RenewalResponseDetail currentDetail = new RenewalResponseDetail();
                bool processingEntry = false;

                for (int row = 0; row < worksheet.Rows.Count; row++)
                {
                    var firstMan = worksheet.Rows[row][0].ToString();
                    var eightMan = worksheet.Rows[row][7].ToString();
                    if (!UselessRow(firstMan, eightMan))
                    {
                        var cell1 = worksheet.Rows[row][0].ToString();
                        if (!string.IsNullOrEmpty(cell1))
                        {
                            string clearCell = cell1.ToString().Trim().ToLower().Replace(" ", "");
                            if (clearCell == positionCardHeader)
                            {
                                positionFinderCardHeaderFound = true;
                            }
                            if (clearCell == positionEmbosingName)
                            {
                                positionFinderEmbossingHeaderFound = true;
                            }
                            if (clearCell == positionCustomerName)
                            {
                                positionFinderCustomerNameFound = true;
                            }
                            if (positionFinderCardHeaderFound && positionFinderEmbossingHeaderFound && positionFinderCustomerNameFound)
                            {
                                if (clearCell == positionBranchHeader)
                                {
                                    positionFinderBranchFound = true;
                                    var branchText = worksheet.Rows[row][7];
                                    if (branchText != null && !string.IsNullOrEmpty(branchText.ToString()))
                                    {
                                        branchCode = branchText.ToString().Substring(1, 3).Trim();
                                        branchName = branchText.ToString().Substring(5).Trim();
                                    }
                                    branchCount++;
                                }

                                if (clearCell == positionProductHeader)
                                {
                                    positionFinderProductFound = true;
                                    currentCardRow = row + 1;
                                    if (worksheet.Rows[row][7] != null && !string.IsNullOrEmpty(worksheet.Rows[row][7].ToString()))
                                    {
                                        productName = worksheet.Rows[row][7].ToString().Trim().ToLower();
                                    }
                                }
                                if (clearCell == positionProductTotalText)
                                {
                                    positionFinderProductFound = false;
                                }
                                if (clearCell == positionBranchTotalText)
                                {
                                    positionFinderBranchFound = false;
                                    positionFinderProductFound = false;
                                    productId = 0;
                                    branchId = 0;
                                }

                                if (positionFinderProductFound)
                                {
                                    if (row == currentCardRow)
                                    {
                                        //line 1
                                        currentDetail = new RenewalResponseDetail()
                                        {
                                            CardNumber = ExtractCellValue(worksheet.Rows[row][0]),
                                            PSAction = ExtractCellValue(worksheet.Rows[row][2]),
                                            Blocked = ExtractCellValue(worksheet.Rows[row][4]),
                                            ExpiryDate = ExtractCellValueDate(worksheet.Rows[row][5]),
                                            RenewalDate = ExtractCellValueDate(worksheet.Rows[row][6]),
                                            Status = ExtractCellValue(worksheet.Rows[row][7]),
                                            ContractNumber = ExtractCellValue(worksheet.Rows[row][8]),
                                            CurrencyCode = ExtractCellValue(worksheet.Rows[row][9]),
                                            ODAmount = ExtractCellDecimal(worksheet.Rows[row][10]),
                                            OLAmount = ExtractCellDecimal(worksheet.Rows[row][11]),
                                            LimitBalance = ExtractCellDecimal(worksheet.Rows[row][12]),
                                        };
                                        int lastDash = currentDetail.CardNumber.LastIndexOf('-');
                                        if (lastDash + 4 > currentDetail.CardNumber.Length)
                                        {
                                            currentDetail.CardNumber = currentDetail.CardNumber.Substring(0, lastDash);
                                            currentDetail.MBR = currentDetail.CardNumber.Substring(lastDash + 1, 100);
                                        }
                                        processingEntry = true;
                                    }
                                    if (row == currentCardRow + 1)
                                    {
                                        //line 2
                                        currentDetail.EmbossingName = ExtractCellValue(worksheet.Rows[row][0]);
                                        currentDetail.ClientId = ExtractCellValue(worksheet.Rows[row][4]);
                                        currentDetail.BirthDate = ExtractCellValueDate(worksheet.Rows[row][5]);
                                        currentDetail.InternalAccountNumber = ExtractCellValue(worksheet.Rows[row][6]);
                                        currentDetail.ExternalAccountNumber = ExtractCellValue(worksheet.Rows[row][7]);
                                        currentDetail.PassportIDNumber = ExtractCellValue(worksheet.Rows[row][8]);
                                        currentDetail.ContractStatus = ExtractCellValue(worksheet.Rows[row][10]);
                                        currentDetail.EmailAddress = ExtractCellValue(worksheet.Rows[row][11]);

                                    }
                                    if (row == currentCardRow + 2)
                                    {
                                        //line 3
                                        currentDetail.CustomerName = ExtractCellValue(worksheet.Rows[row][0]);
                                        currentDetail.CreationDate = ExtractCellValueDate(worksheet.Rows[row][5]);
                                        currentDetail.OnlineStatus = ExtractCellValue(worksheet.Rows[row][7]);
                                        currentDetail.ContactPhone = ExtractCellValue(worksheet.Rows[row][8]);
                                        currentDetail.MobilePhone = ExtractCellValue(worksheet.Rows[row][10]);
                                        details.Add(currentDetail);
                                        logger.Debug($"{currentDetail.CardNumber}, {currentDetail.ContractNumber} , {currentDetail.CustomerName}");
                                        currentCardRow = row + 1;
                                        processingEntry = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            currentCardRow++;
                        }
                    }
                    else
                    {
                        currentCardRow++;
                    }
                }
                excelReader.Close();
            }
            catch (Exception exp)
            {
                logger.Error($"Unknown exception in {MethodBase.GetCurrentMethod().Name}", exp);
            }
            return details;
        }


        private decimal? ExtractCellDecimal(object excelRange)
        {
            string value = ExtractCellValue(excelRange);
            decimal? result;
            try
            {
                result = Decimal.Parse(value);
            }
            catch (Exception exp)
            {
                result = null;
            }
            return result;
        }

        private DateTime? ExtractCellValueDate(object excelRange)
        {
            string value = ExtractCellValue(excelRange);
            DateTime? result;
            try
            {
                result = DateTime.Parse(value);
            }
            catch (Exception exp)
            {
                try
                {
                    result = DateTime.FromOADate(Convert.ToDouble(value));
                }
                catch (Exception)
                {
                    result = null;
                }
            }
            return result;
        }

        private string ExtractCellValue(object excelRange)
        {
            if (excelRange != null && excelRange.ToString() != null)
            {
                return excelRange.ToString();
            }
            return string.Empty;
        }
    }
}
