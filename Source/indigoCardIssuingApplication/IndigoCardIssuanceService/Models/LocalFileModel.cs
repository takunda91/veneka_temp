using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndigoCardIssuanceService.Models
{
    public class LocalFileModel
    {
        public string AccountNumber { get; set; }

        public string CustomerId { get; set; }

        public byte[] Content { get; set; }

        public string FileName { get; set; }
    }
}