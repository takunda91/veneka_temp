using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Veneka.Indigo.Renewal.Entities;
using Veneka.Indigo.COMS.DataSource;
using Veneka.Indigo.Renewal;
using Veneka.Indigo.Renewal.dal;
using Veneka.Indigo.Common.Models;
using Common.Logging;
using System.Configuration;
using System.Reflection;
using Veneka.Indigo.Common.License;

namespace Veneka.Indigo.Renewal.Incoming
{
    public class RenewalExtractor
    {

        string positionCardHeader = "cardnumber";
        string positionEmbosingName = "embossingname";
        string positionCustomerName = "customername";
        string positionBranchHeader = "branch";
        string positionProductHeader = "product";
        string positionBranchTotalText = "branchtotal";
        string positionProductTotalText = "producttotal";

        IRenewalDataAccess _renewalDataAccess = new RenewalDataAccess();
        IRenewalOperations _renewalOperations = new RenewalOperations();
        ILog logger = LogManager.GetLogger("Veneka.Indigo.Renewal.Importer");

        public RenewalExtractor()
        {

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

        public void ExtractRenewalFiles()
        {
            string baseDir = ConfigurationManager.AppSettings["baseDir"].ToString();
            string[] files = Directory.GetFiles($"{baseDir}renewals");
            foreach (var file in files)
            {
                if (ExtractRenewalFile(file))
                {
                    ArchiveFile($"{baseDir}renewals", file);
                }
            }
        }

        private void ArchiveFile(string baseDir, string filename)
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

        private bool ExtractRenewalFile(string file)
        {
            bool complete = false;
            logger.Trace($"Extracting File {file}");

            try
            {

                FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read);

                var excelReader = file.ToLower().Contains(".xlsx") ? ExcelReaderFactory.CreateOpenXmlReader(stream) : ExcelReaderFactory.CreateBinaryReader(stream);
                DataTable worksheet = excelReader.AsDataSet().Tables[0];

                int branchCount = 0;
                int cardCount = 0;
                List<RenewalDetail> details = new List<RenewalDetail>();

                bool positionFinderCardHeaderFound = false;
                bool positionFinderEmbossingHeaderFound = false;
                bool positionFinderCustomerNameFound = false;
                bool positionFinderProductFound = false;

                string branchCode;
                string branchName;

                int branchId = 0;
                int productId = 0;
                int currentCardRow = 0;

                RenewalDetail currentDetail = new RenewalDetail();
                List<branch> branches = _renewalDataAccess.GetBranches().ToList();
                List<issuer_product> products = _renewalDataAccess.GetProducts().ToList();

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
                                    var branchText = worksheet.Rows[row][7];
                                    if (branchText != null && !string.IsNullOrEmpty(branchText.ToString()))
                                    {
                                        branchCode = branchText.ToString().Substring(1, 3).Trim();
                                        branchName = branchText.ToString().Substring(5).Trim();
                                        var checkBranch = branches.Where(p => p.emp_branch_code == branchCode).Select(p => new { Key = p.branch_id, Value = p.branch_code }).FirstOrDefault();
                                        if (checkBranch != null)
                                        {
                                            branchId = checkBranch.Key;
                                        }
                                        else
                                        {
                                            throw new Exception("Card Renewal: Branch not found.  Ensure the branch is configured.");
                                        }
                                    }
                                    branchCount++;
                                }

                                if (clearCell == positionProductHeader)
                                {
                                    positionFinderProductFound = true;
                                    currentCardRow = row + 1;
                                    //if (worksheet.Rows[row][7] != null && !string.IsNullOrEmpty(worksheet.Rows[row][7].ToString()))
                                    //{
                                    //    productName = worksheet.Rows[row][7].ToString().Trim().ToLower();

                                    //    var checkProduct = products.Where(p => p.product_name.Trim().ToLower() == productName).Select(p => new { Key = p.product_id, Value = p.product_name }).FirstOrDefault();
                                    //    if (checkProduct != null)
                                    //    {
                                    //        productId = checkProduct.Key;
                                    //    }
                                    //    else
                                    //    {
                                    //        //throw new Exception("Card Renewal: Product not found.  Ensure the product is configured.");
                                    //    }
                                    //}
                                }
                                if (clearCell == positionProductTotalText)
                                {
                                    positionFinderProductFound = false;
                                }
                                if (clearCell == positionBranchTotalText)
                                {
                                    positionFinderProductFound = false;
                                    productId = 0;
                                    branchId = 0;
                                }

                                if (positionFinderProductFound)
                                {
                                    if (row == currentCardRow)
                                    {
                                        //line 1
                                        currentDetail = new RenewalDetail()
                                        {
                                            ProductId = productId,
                                            BranchId = branchId,
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
                                        try
                                        {
                                            string cardNumber = currentDetail.CardNumber;
                                            int lastDash = cardNumber.LastIndexOf('-');
                                            if (lastDash + 4 > cardNumber.Length)
                                            {
                                                currentDetail.CardNumber = cardNumber.Substring(0, lastDash);
                                                try
                                                {
                                                    currentDetail.MBR = cardNumber.Substring(lastDash + 1, (cardNumber.Length - currentDetail.CardNumber.Length) - 1);
                                                }
                                                catch (Exception)
                                                {
                                                }
                                            }
                                        }
                                        catch (Exception exp)
                                        {
                                            throw exp;
                                        }
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
                                        logger.Debug($"{currentDetail.CardNumber}, {currentDetail.ContractNumber} , {currentDetail.CustomerName}, {currentDetail.ProductId}");
                                        currentCardRow = row + 1;
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

                //check for products based on BIN
                if (products.Count > 0)
                {
                    var licensedBinCodes = LicenseManager.ValidateAffiliateKey(products.FirstOrDefault().issuer_id, 1, "SYSTEM").Select(p => p.Trim().ToUpper()).ToList();

                    foreach (var item in details)
                    {
                        var finder = FindRelavantProduct(item, products);
                        if (finder == null)
                        {
                            throw new Exception("Card Renewal: Product not found.  Ensure the product is configured.");
                        }
                        else
                        {
                            if (ProductLicensed(finder, licensedBinCodes))
                            {
                                item.ProductId = finder.product_id;
                            }
                            else
                            {
                                throw new Exception("Card Renewal: Product not licensed.  Ensure the product is licensed.");
                            }
                        }
                    }

                    List<string> duplicates = new List<string>();
                    //check the cards PAN and MBR
                    foreach (var item in details)
                    {
                        int mbr = Convert.ToInt32(item.MBR);
                        if (_renewalOperations.CardPANMBRExists(item.CardNumber.Trim(), mbr) || _renewalOperations.RenewalPANMBRExists(item.CardNumber.Trim(), mbr))
                        {
                            duplicates.Add($"PAN = {item.CardNumber}: MRB = {item.MBR}");
                        }
                    }

                    if (duplicates.Count > 0)
                    {
                        throw new Exception($"Some duplicates have been found.  See the list below{Environment.NewLine}{ string.Join(Environment.NewLine, duplicates)} ");
                    }

                    cardCount = details.Count;
                    //create a new batch
                    RenewalFile renewalFile = new RenewalFile()
                    {
                        CreateDate = DateTime.Now,
                        CreatorId = 1,
                        DateUploaded = DateTime.Now,
                        Details = details,
                        FileName = Path.GetFileName(file),
                        Id = 0,
                        Status = RenewalStatusType.Loaded
                    };
                    _renewalOperations.Create(renewalFile, 1, "SYSTEM");
                }
                else
                {
                    throw new Exception();
                }
                complete = true;
            }
            catch (Exception exp)
            {
                complete = false;
                logger.Error($"Unknown exception in {MethodBase.GetCurrentMethod().Name}", exp);
            }
            return complete;
        }

        private issuer_product FindRelavantProduct(RenewalDetail item, List<issuer_product> products)
        {
            issuer_product found = null;
            string found_derived_bin = string.Empty;
            foreach (var product in products)
            {
                string product_bin = product.product_bin_code.Trim().Replace("-", string.Empty);
                if (!string.IsNullOrWhiteSpace(product.sub_product_code))
                {
                    product_bin = $"{product.product_bin_code.Trim()}{product.sub_product_code.Trim()}".Replace("-", string.Empty);
                }

                string derivedBin = item.CardNumber.Replace("-", string.Empty).Trim().Substring(0, product_bin.Length);
                if (derivedBin == product_bin)
                {
                    //check that you don't have a product already identified
                    if (found == null)
                    {
                        found = product;
                        found_derived_bin = derivedBin;
                    }
                    else
                    {
                        if (derivedBin.Length > found_derived_bin.Length)
                        {
                            //replace because we have a more accurate link
                            found = product;
                            found_derived_bin = derivedBin;
                        }
                    }
                }
            }
            return found;
        }

        private bool ProductLicensed(issuer_product finder, List<string> licensedBinCodes)
        {
            string product_bin = string.Empty;
            if (!string.IsNullOrEmpty(finder.sub_product_code) && finder.sub_product_code.Length > 0)
            {
                product_bin = finder.product_bin_code + finder.sub_product_code;
            }
            else
            {
                product_bin = finder.product_bin_code;
            }
            product_bin = product_bin.Trim().ToUpper();
            var entryLicensed = licensedBinCodes.Where(p => p == product_bin).FirstOrDefault();
            if (entryLicensed != null)
            {
                return true;
            }
            else
            {
                return false;
            }
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