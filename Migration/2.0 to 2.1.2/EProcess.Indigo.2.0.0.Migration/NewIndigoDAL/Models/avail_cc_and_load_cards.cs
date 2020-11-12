namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class avail_cc_and_load_cards
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long card_id { get; set; }
    }
}
