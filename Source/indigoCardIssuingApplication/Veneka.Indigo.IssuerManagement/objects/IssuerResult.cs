using System;
using System.Collections.Generic;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.IssuerManagement.objects
{
    public sealed class IssuerResult
    {
        public issuer Issuer { get; set; }
        public List<issuer_interface> ProdInterfaces { get; set; }
        public List<issuer_interface> IssueInterfaces { get; set; }
        public string pin_notification_message { get; set; }
        public int max_number_of_pin_send { get; set; }
        public int pin_block_delete_days { get; set; }

    }
}