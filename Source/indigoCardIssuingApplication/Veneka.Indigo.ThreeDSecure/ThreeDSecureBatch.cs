using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.ThreeDSecure.Data;
using Veneka.Indigo.ThreeDSecure.Data.Objects;
using Veneka.Indigo.ThreeDSecure.Integration;
using Veneka.Indigo.ThreeDSecure.Integration.Objects;

namespace Veneka.Indigo.ThreeDSecure
{
    public class ThreeDSecureBatch
    {
        private I3DSDataAccess _dataAccess;
        private IVBVFile _vbvfile;

        List<VBVCardDetails> VBVCards = new List<VBVCardDetails>();


        public ThreeDSecureBatch(I3DSDataAccess dataAccess)
        {
            
            _dataAccess = dataAccess;
        }

        //public List<VBVCardDetails> ReadVBVCardDetails()
        //{
          
            // Step 1: get issued card data            
            //var carddetails = _dataAccess.GetCardDetails();

            // Step 2: extract indigo data to 3dsecure data
         //return   GetDataForVBV(carddetails);
            // Step 3: call Integration layer
            
            
        //}

        public void GenerateFile(List<VBVCardDetails> VBVCards, string path)
        {
            _vbvfile.CreateVBVFile(VBVCards, path);
        }
        //public List<VBVCardDetails> GetDataForVBV(List<CardDetails> Carddetails)
        //{
        //    if (Carddetails != null)
        //    {
        //        foreach (var item in Carddetails)
        //        {
        //            VBVCards.Add(new VBVCardDetails
        //            {
        //                Card_Number = item.Card_Number,
        //                Expiry_Date = item.Card_Expiry_Date.ToString("dd/MM/yyyy"),
        //                Status = "1",
        //                Contact_Number = item.Contact_Number,
        //                Product_Id=item.Product_id


        //            });
        //        }
        //    }

        //    else
        //    {
        //        throw new Exception("CardDetails are empty");
        //    }
        //    return VBVCards;
        //}

    }
}
