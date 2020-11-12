namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class connection_parameters
    {
        public connection_parameters()
        {
            issuer_interface = new HashSet<issuer_interface>();
        }

        [Key]
        public int connection_parameter_id { get; set; }

        [Required]
        [StringLength(100)]
        public string connection_name { get; set; }

        [Required]
        [StringLength(200)]
        public string address { get; set; }

        public int port { get; set; }

        [Required]
        [StringLength(200)]
        public string path { get; set; }

        [Required]
        [StringLength(50)]
        public string protocol { get; set; }

        public int auth_type { get; set; }

        [Required]
        public byte[] username { get; set; }

        [Required]
        public byte[] password { get; set; }

        public virtual ICollection<issuer_interface> issuer_interface { get; set; }
    }
}
