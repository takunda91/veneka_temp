using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    [DataContract]
    public class PrintJob : IPrintJob
    {
        [DataMember(IsRequired = true)]
        public string PrintJobId { get; set; }

        [DataMember(IsRequired = true)]
        public string ProductBin { get; set; }

        [DataMember(IsRequired = true)]
        public bool MustReturnCardData { get; set; }

        [DataMember(IsRequired = true)]
        public ProductField[] ProductFields { get; set; }

        [DataMember(IsRequired = true)]
        public AppOptions[] ApplicationOptions { get; set; }


        public Dictionary<string, string> AppOptionToDictionary()
        {
            Dictionary<string, string> options = new Dictionary<string, string>();

            foreach(var appOption in ApplicationOptions)
            {
                options.Add(appOption.Key, appOption.Value);
            }

            return options;
        }
    }
}
