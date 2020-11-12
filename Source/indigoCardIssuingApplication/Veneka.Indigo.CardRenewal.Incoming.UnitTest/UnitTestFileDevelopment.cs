using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.Renewal.Entities;
using Veneka.Indigo.Renewal.Incoming;

namespace Veneka.Indigo.CardRenewal.Incoming.UnitTest
{
    [TestClass]
    public class UnitTestFileDevelopment
    {
        [TestMethod]
        public void FileExtraction()
        {
            string filePath = @"C:\veneka\samples\Card Renewal Flow_V1.0.xlsx";
            byte[] fileContent = File.ReadAllBytes(filePath);
            RenewalExtractor operations = new RenewalExtractor();

            Dictionary<int, string> productList = new Dictionary<int, string>();
            productList.Add(1, "Visa");
            productList.Add(2, "TOPINSTANT");
            productList.Add(3, "Electron");

            Dictionary<int, string> branchList = new Dictionary<int, string>();
            branchList.Add(-1, "");
            branchList.Add(1, "000");
            branchList.Add(2, "001");
            branchList.Add(3, "111");
            branchList.Add(4, "112");
            branchList.Add(5, "113");
            branchList.Add(6, "114");
            branchList.Add(7, "115");
            branchList.Add(8, "116");
            branchList.Add(9, "117");
            branchList.Add(10, "118");
            branchList.Add(11, "119");
            branchList.Add(12, "121");
            branchList.Add(13, "122");
            branchList.Add(14, "123");
            branchList.Add(15, "124");
            branchList.Add(16, "125");
            branchList.Add(17, "126");
            branchList.Add(18, "128");
            branchList.Add(19, "129");
            branchList.Add(20, "131");
            branchList.Add(21, "133");
            branchList.Add(22, "211");
            branchList.Add(23, "212");
            branchList.Add(24, "214");
            branchList.Add(25, "216");
            branchList.Add(26, "217");
            branchList.Add(27, "221");
            branchList.Add(28, "222");
            branchList.Add(29, "231");
            branchList.Add(30, "251");
            branchList.Add(31, "261");
            branchList.Add(32, "311");
            branchList.Add(33, "321");
            branchList.Add(34, "331");
            branchList.Add(35, "431");
            branchList.Add(36, "432");
            branchList.Add(37, "433");
            branchList.Add(38, "441");
            branchList.Add(39, "442");
            branchList.Add(40, "443");
            branchList.Add(41, "451");
            branchList.Add(42, "461");
            branchList.Add(43, "471");
            branchList.Add(44, "481");
            branchList.Add(45, "482");
            branchList.Add(46, "511");
            branchList.Add(47, "512");
            branchList.Add(48, "521");
            branchList.Add(49, "731");
            branchList.Add(50, "811");
            List<RenewalDetail> details = new List<RenewalDetail>();
            int x;
            int y;
            //operations.ExtractFile(fileContent, productList, branchList, out x, out y, out details);

        }
    }
}
