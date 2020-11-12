namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class product_fields
    {
        public product_fields()
        {
            customer_fields = new HashSet<customer_fields>();
            customer_image_fields = new HashSet<customer_image_fields>();
        }

        [Key]
        public int product_field_id { get; set; }

        public int product_id { get; set; }

        [Required]
        [StringLength(100)]
        public string field_name { get; set; }

        public int print_field_type_id { get; set; }

        public decimal? X { get; set; }

        public decimal? Y { get; set; }

        public decimal? width { get; set; }

        public decimal? height { get; set; }

        [StringLength(50)]
        public string font { get; set; }

        public int? font_size { get; set; }

        public string mapped_name { get; set; }

        public bool editable { get; set; }

        public bool deleted { get; set; }

        [StringLength(100)]
        public string label { get; set; }

        public int max_length { get; set; }

        public virtual ICollection<customer_fields> customer_fields { get; set; }

        public virtual ICollection<customer_image_fields> customer_image_fields { get; set; }

        public virtual issuer_product issuer_product { get; set; }

        public virtual print_field_types print_field_types { get; set; }
    }
}
