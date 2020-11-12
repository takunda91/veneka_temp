namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class flex_affiliate_codes
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int issuer_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(15)]
        public string affiliate_code { get; set; }

        public virtual issuer issuer { get; set; }
    }
}
