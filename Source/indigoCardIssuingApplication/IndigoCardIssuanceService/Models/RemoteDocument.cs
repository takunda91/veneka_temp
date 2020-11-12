using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndigoCardIssuanceService.Models
{
    public class RemoteDocument
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public string MimeType { get; set; }
    }
}