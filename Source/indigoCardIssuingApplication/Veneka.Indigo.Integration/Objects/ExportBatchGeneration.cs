using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Integration.Objects
{
    [Serializable]
    [DataContract]
    public class ExportBatchGeneration
    {
        public ExportBatchGeneration()
        {

        }
        public int ResultId { get; set; }
        public int[] ExportBatchIds { get; set; }
    }
}
