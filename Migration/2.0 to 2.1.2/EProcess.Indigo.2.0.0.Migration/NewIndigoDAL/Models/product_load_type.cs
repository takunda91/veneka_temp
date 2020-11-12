namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class product_load_type
    {
        public product_load_type()
        {
            issuer_product = new HashSet<issuer_product>();
            product_load_type_language = new HashSet<product_load_type_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int product_load_type_id { get; set; }

        [Required]
        [StringLength(100)]
        public string product_load_type_name { get; set; }

        public virtual ICollection<issuer_product> issuer_product { get; set; }

        public virtual ICollection<product_load_type_language> product_load_type_language { get; set; }
    }
}
