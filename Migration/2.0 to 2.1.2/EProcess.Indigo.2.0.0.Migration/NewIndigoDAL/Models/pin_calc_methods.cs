namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_calc_methods
    {
        public pin_calc_methods()
        {
            issuer_product = new HashSet<issuer_product>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int pin_calc_method_id { get; set; }

        [Required]
        [StringLength(100)]
        public string pin_calc_method_name { get; set; }

        public virtual ICollection<issuer_product> issuer_product { get; set; }
    }
}
