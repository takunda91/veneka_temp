using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.API
{

    [DataContract]
    public class UpdatePrintBatchDetails : PrintBatchData
    {
        private bool successful;
        [DataMember]
        public string notes { get ; set; }
        [DataMember]
        public List<string> Cardstobespoiled { get; set; }
        [DataMember]
        public bool Successful
        {
            get { return successful; }
            set
            {
                successful = value;
            }
        }

        List<RequestDetails> requestDetails;
        [DataMember]
        public List<RequestDetails> RequestDetails
        {
            get { return requestDetails; }
            set
            {
                requestDetails = value;
            }
        }

    }
        [DataContract]
    public class GetPrintBatchDetails : PrintBatchData
    {

        private int noOfRequests;
       
        private DateTime createdDateTime;
        private string createdBy;

        [DataMember]
        public string CreatedBy
        {
            get { return createdBy; }
            set
            {
                createdBy = value;
            }
        }

        [DataMember]
        public int NoOfRequests
        {
            get { return noOfRequests; }
            set
            {
                noOfRequests = value;
            }
        }


        [DataMember]
        public DateTime CreatedDateTime
        {
            get { return createdDateTime; }
            set
            {
                createdDateTime = value;
            }
        }
      


    }

    [DataContract]
    public class PrintBatchData 
    {
        private long? printBatchId;
        private int? printBatchStatusId;

        private string printBatchStatus;

        private string printBatchReference;

        private int? productId;

        [DataMember]
        public string PrintBatchReference
        {
            get { return printBatchReference; }
            set
            {
                CheckNullSet<string>(printBatchReference);
                printBatchReference = value;
            }
        }


        [DataMember]
        public int? PrintBatchStatusId
        {
            get { return printBatchStatusId; }
            set
            {
                CheckNullSet<int?>(printBatchStatusId);
                printBatchStatusId = value;
            }
        }
        [DataMember]
        public int? ProductId
        {
            get { return productId; }
            set
            {
                CheckNullSet<int?>(productId);
                productId = value;
            }
        }

      
        [DataMember]
        public long? PrintBatchId
        {
            get { return printBatchId; }
            set
            {
                CheckNullSet<long?>(printBatchId);
                printBatchId = value;
            }
        }

       

        public void CheckNullSet<T>(T field)
        {
            if (EqualityComparer<T>.Default.Equals(field, default(T)))
            {
                return;
            }

            throw new Exception("Property already set.");
        }

        [DataMember]
        public string Print_Batch_Status
        {
            get { return printBatchStatus; }
            set
            {
                printBatchStatus = value;
            }
        }

        
    }


    [DataContract]
    public class RequestDetails 
    {

        private long requestId;
        private string requestReference;
        private string requestStatus;
        private string pan;
        private int? requestStatusId;
        private List<ProductTemplate> prodTemplateList;

        public void CheckNullSet<T>(T field)
        {
            if (EqualityComparer<T>.Default.Equals(field, default(T)))
            {
                return;
            }

            throw new Exception("Property already set.");
        }
        [DataMember]
        public long RequestId
        {
            get { return requestId; }
            set
            {
                requestId = value;
            }
        }
        [DataMember]
        public List<ProductTemplate> ProdTemplateList
        {
            get { return prodTemplateList; }
            set
            {
                prodTemplateList = value;
            }
        }

        [DataMember]
        public int? RequestStatuesId
        {
            get { return requestStatusId; }
            set
            {
                requestStatusId = value;
            }
        }
        

        [DataMember]
        public string RequestStatus
        {
            get { return requestStatus; }
            set
            {
                requestStatus = value;
            }
        }
        [DataMember]
        public string PAN
        {
            get { return pan; }
            set
            {
                pan = value;
            }
        }
        [DataMember]
        public string RequestReference
        {
            get { return requestReference; }
            set
            {
                requestReference = value;
            }
        }


        
    }
    [DataContract]
    public class ProductTemplateResult
    {
        [DataMember]
        public int ProductPrintFieldId { get; set; }

        [DataMember]
        public byte[] Value { get; set; }
    }
    [DataContract]
    public class ProductTemplate : ProductTemplateResult
    {
        [DataMember]
        public int productPrintFieldTypeId { get; set; }
        
        [DataMember]
        public float x { get; set; }
        [DataMember]
        public float y { get; set; }
        [DataMember]
        public string font { get; set; }
        [DataMember]
        public int font_size { get; set; }
        [DataMember]
        public string fontColourRGB { get; set; }
        [DataMember]
        public int PrintSide { get; set; }
        [DataMember]
        public string MappedName { get; set; }
    }


    }
