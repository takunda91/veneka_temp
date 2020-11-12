using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Veneka.Indigo.ThreeDSecure.Integration
{
    public class VBVFile: IVBVFile
    {
        public bool CreateVBVFile(List<Objects.VBVCardDetails> VBVCarddetails , string path)
        {
            //string path = "C:\\Users\\rishon.pious\\OneDrive - Veneka\\Documents\\Veneka\\Customer\\Ghana Home loan\\vbv";
            //string path = "Documents";
            if (VBVCarddetails != null)
            {
                XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("Record", from item in VBVCarddetails
                                           select
           new XElement("PAN", item.Card_Number,
           new XElement("CANCELDATE", item.Expiry_Date,
           new XElement("CRDSTAT", "1",
           new XElement("CELLPHONE", item.Contact_Number))))));

                xdoc.Save(path);
                return true;
            }
            else
            {
                throw new Exception("VBVCardDetails are empty");
               
            }
           
        }
    }
}
