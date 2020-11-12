namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class masterkeys
    {
        public masterkeys()
        {
            terminals = new HashSet<terminals>();
        }

        [Key]
        public int masterkey_id { get; set; }

        [Required]
        [StringLength(250)]
        public string masterkey_name { get; set; }

        [Required]
        public byte[] masterkey { get; set; }

        public int issuer_id { get; set; }

        public DateTime? date_created { get; set; }

        public DateTime? date_changed { get; set; }

        public virtual issuer issuer { get; set; }

        public virtual ICollection<terminals> terminals { get; set; }
    }
}
