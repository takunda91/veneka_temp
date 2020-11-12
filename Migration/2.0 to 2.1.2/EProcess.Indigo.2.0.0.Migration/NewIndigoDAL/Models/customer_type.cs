namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class customer_type
    {
        public customer_type()
        {
            customer_account = new HashSet<customer_account>();
            customer_type_language = new HashSet<customer_type_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int customer_type_id { get; set; }

        [Required]
        [StringLength(100)]
        public string customer_type_name { get; set; }

        public virtual ICollection<customer_account> customer_account { get; set; }

        public virtual ICollection<customer_type_language> customer_type_language { get; set; }
    }
}
