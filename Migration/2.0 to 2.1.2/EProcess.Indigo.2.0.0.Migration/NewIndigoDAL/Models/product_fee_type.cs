namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class product_fee_type
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fee_type_id { get; set; }

        [Required]
        [StringLength(50)]
        public string fee_type_name { get; set; }
    }
}
