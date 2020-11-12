using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EProcess.Indigo._2._0._0.Migration.Common.Models
{
    /// <summary>
    /// A class to model the card returned by the get cards query script
    /// </summary>
    public class Card
    {
        public long card_id { get; set; }
        public string card_number { get; set; }
        public string branch_name { get; set; }
        public string product_name { get; set; }
    }
}
