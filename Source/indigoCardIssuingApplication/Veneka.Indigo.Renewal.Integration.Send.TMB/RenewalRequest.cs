using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Veneka.Indigo.Renewal.Entities;

namespace Veneka.Indigo.Renewal.Integration.Send.TMB
{
    public class RenewalRequest : IRenewalRequest
    {
        public List<string> BuildFile(List<RenewalDetailListModel> details, string destinationFolder, string newFileName)
        {
            List<string> createdFiles = new List<string>();
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }
            //create separate file for each product
            var products = details.GroupBy(p => p.ProductId).Select(p => p.Key).ToList();
            foreach (var productId in products)
            {
                string issuer = string.Empty;
                string issuerCode = string.Empty;
                List<CardRecordModel> records = new List<CardRecordModel>();
                foreach (var item in details.Where(p => p.ProductId == productId).ToList())
                {
                    string[] panComponents = item.CardNumber.Split('-');
                    string pan = panComponents[0];
                    int mbr = 0;
                    issuer = pan.Substring(0, 6);
                    if (issuer.Contains("*"))
                    {
                        issuer = issuer.Replace("*", "0");
                    }

                    int.TryParse(item.MBR, out mbr);

                    issuerCode = item.IssuerCode;
                    records.Add(new CardRecordModel()
                    {
                        cmCHANGEPAN = 0,
                        PAN = pan,
                        REASON = "5",
                        MBR = mbr
                    });
                }
                var xml = new XElement("root",
                    from c in records
                    select new XElement("record",
                            new XElement("PAN", c.PAN),
                            new XElement("mbr", c.MBR),
                            new XElement("REASON", c.REASON),
                            new XElement("xmCHANGEPAN", c.cmCHANGEPAN)
                            ));

                new XDocument(xml).Save(Path.Combine(destinationFolder, newFileName));
                createdFiles.Add(newFileName);
            }
            return createdFiles;
        }
    }
}
