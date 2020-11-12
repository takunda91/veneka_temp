namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class customer_account_type
    {
        public customer_account_type()
        {
            customer_account = new HashSet<customer_account>();
            customer_account_type_language = new HashSet<customer_account_type_language>();
            issuer_product = new HashSet<issuer_product>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int account_type_id { get; set; }

        [Required]
        [StringLength(100)]
        public string account_type_name { get; set; }

        public bool active_YN { get; set; }

        public virtual ICollection<customer_account> customer_account { get; set; }

        public virtual ICollection<customer_account_type_language> customer_account_type_language { get; set; }

        public virtual ICollection<issuer_product> issuer_product { get; set; }
    }
}
