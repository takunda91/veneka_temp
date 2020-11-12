namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class product_fee_scheme
    {
        public product_fee_scheme()
        {
            issuer_product = new HashSet<issuer_product>();
            product_fee_detail = new HashSet<product_fee_detail>();
        }

        [Key]
        public int fee_scheme_id { get; set; }

        public int issuer_id { get; set; }

        [Required]
        [StringLength(100)]
        public string fee_scheme_name { get; set; }

        public bool deleted_yn { get; set; }

        public virtual issuer issuer { get; set; }

        public virtual ICollection<issuer_product> issuer_product { get; set; }

        public virtual ICollection<product_fee_detail> product_fee_detail { get; set; }
    }
}
