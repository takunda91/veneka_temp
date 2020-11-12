namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class customer_title
    {
        public customer_title()
        {
            customer_account = new HashSet<customer_account>();
            customer_title_language = new HashSet<customer_title_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int customer_title_id { get; set; }

        [Required]
        [StringLength(100)]
        public string customer_title_name { get; set; }

        public virtual ICollection<customer_account> customer_account { get; set; }

        public virtual ICollection<customer_title_language> customer_title_language { get; set; }
    }
}
