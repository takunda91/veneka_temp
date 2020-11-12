using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.ThreeDSecure.Data;
using Veneka.Indigo.ThreeDSecure.Data.Objects;

namespace Veneka.Indigo.ThreeDSecureTests.TestAsset
{
    class TestDAL : I3DSDataAccess
    {
        public List<ThreeDSecureCardDetails> GetCards(long threedBatchId, bool checkMasking, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<ThreeDSecureBatch> GetRecreateBatches(int issuerId, string interfaceGuid, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        //public List<CardDetails> GetCardDetails()
        //{
        //    string expdate = "28/08/2021";
        //    DateTime expirtdate = Convert.ToDateTime(expdate);
        //    List<CardDetails> rtn = new List<CardDetails>();
        //    rtn.Add(new CardDetails() { Card_Number = "12345", Contact_Number = "222334455", Card_Expiry_Date = expirtdate });


        //    rtn.Add(new CardDetails());
        //    return rtn;
        //}

        public List<ThreeDSecureCardDetails> GetUnregisteredCards(int issuerId, string interfaceGuid, bool checkMasking, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public bool Update3DSecureBatchRegistered(long threedsBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public bool UpdateBatchStatus(long threedsBatchId, int statusId, string statusNote, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }
    }
}
