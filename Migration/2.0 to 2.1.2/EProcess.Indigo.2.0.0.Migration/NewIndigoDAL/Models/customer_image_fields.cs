namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class customer_image_fields
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long customer_account_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int product_field_id { get; set; }

        [Column(TypeName = "image")]
        [Required]
        public byte[] value { get; set; }

        public virtual customer_account customer_account { get; set; }

        public virtual product_fields product_fields { get; set; }
    }
}
