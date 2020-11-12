namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class product_service_requet_code1
    {
        public product_service_requet_code1()
        {
            issuer_product = new HashSet<issuer_product>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int src1_id { get; set; }

        [Required]
        [StringLength(100)]
        public string name { get; set; }

        public virtual ICollection<issuer_product> issuer_product { get; set; }
    }
}
