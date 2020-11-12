namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_reissue
    {
        public pin_reissue()
        {
            pin_reissue_status = new HashSet<pin_reissue_status>();
        }

        public int issuer_id { get; set; }

        public int branch_id { get; set; }

        public int product_id { get; set; }

        [Required]
        public byte[] pan { get; set; }

        public DateTime reissue_date { get; set; }

        public long operator_user_id { get; set; }

        public long? authorise_user_id { get; set; }

        public bool failed { get; set; }

        [Required]
        [StringLength(500)]
        public string notes { get; set; }

        [Key]
        public long pin_reissue_id { get; set; }

        public byte[] primary_index_number { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime request_expiry { get; set; }

        public virtual branch branch { get; set; }

        public virtual issuer issuer { get; set; }

        public virtual issuer_product issuer_product { get; set; }

        public virtual ICollection<pin_reissue_status> pin_reissue_status { get; set; }

        public virtual user user { get; set; }

        public virtual user user1 { get; set; }
    }
}
