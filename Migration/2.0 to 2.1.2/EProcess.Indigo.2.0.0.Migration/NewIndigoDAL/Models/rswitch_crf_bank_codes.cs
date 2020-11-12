namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class rswitch_crf_bank_codes
    {
        [Key]
        public int bank_id { get; set; }

        public int issuer_id { get; set; }

        [StringLength(2)]
        public string bank_code { get; set; }

        public virtual issuer issuer { get; set; }
    }
}
