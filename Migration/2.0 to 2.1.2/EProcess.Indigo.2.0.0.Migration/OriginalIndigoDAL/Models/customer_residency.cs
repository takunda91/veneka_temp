namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class customer_residency
    {
        public customer_residency()
        {
            customer_account = new HashSet<customer_account>();
            customer_residency_language = new HashSet<customer_residency_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int resident_id { get; set; }

        [Required]
        [StringLength(100)]
        public string residency_name { get; set; }

        public virtual ICollection<customer_account> customer_account { get; set; }

        public virtual ICollection<customer_residency_language> customer_residency_language { get; set; }
    }
}
