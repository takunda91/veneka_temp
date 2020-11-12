using System;

namespace Veneka.Indigo.AuditManagement.objects
{
    public class AuditSearchParameters
    {
        public string DataChanged { get; set; }

        public int Issuer { get; set; }

        public string UserAction { get; set; }

        public string KeyWord { get; set; }

        public string User { get; set; }

        public DateTime DateTo { get; set; }

        public DateTime DateFrom { get; set; }
    }
}