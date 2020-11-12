namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class product_fee_detail
    {
        public product_fee_detail()
        {
            product_fee_charge = new HashSet<product_fee_charge>();
        }

        public int fee_scheme_id { get; set; }

        [Key]
        public int fee_detail_id { get; set; }

        [Required]
        [StringLength(100)]
        public string fee_detail_name { get; set; }

        public DateTime effective_from { get; set; }

        public bool fee_waiver_YN { get; set; }

        public bool fee_editable_YN { get; set; }

        public bool deleted_yn { get; set; }

        public DateTime? effective_to { get; set; }

        public virtual ICollection<product_fee_charge> product_fee_charge { get; set; }

        public virtual product_fee_scheme product_fee_scheme { get; set; }
    }
}
