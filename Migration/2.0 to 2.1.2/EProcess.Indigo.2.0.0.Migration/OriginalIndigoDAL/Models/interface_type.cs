namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class interface_type
    {
        public interface_type()
        {
            interface_type_language = new HashSet<interface_type_language>();
            issuer_interface = new HashSet<issuer_interface>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int interface_type_id { get; set; }

        [Required]
        [StringLength(100)]
        public string interface_type_name { get; set; }

        public virtual ICollection<interface_type_language> interface_type_language { get; set; }

        public virtual ICollection<issuer_interface> issuer_interface { get; set; }
    }
}
