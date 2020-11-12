namespace NewIndigoDAL.Models
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
            product_interface = new HashSet<product_interface>();
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

        public int connection_parameter_type_id { get; set; }

        public int? header_length { get; set; }

        public byte[] identification { get; set; }

        public int? timeout_milli { get; set; }

        public int? buffer_size { get; set; }

        [StringLength(1)]
        public string doc_type { get; set; }

        [StringLength(100)]
        public string name_of_file { get; set; }

        public bool? file_delete_YN { get; set; }

        public int? file_encryption_type_id { get; set; }

        public bool? duplicate_file_check_YN { get; set; }

        public virtual connection_parameter_type connection_parameter_type { get; set; }

        public virtual file_encryption_type file_encryption_type { get; set; }

        public virtual ICollection<issuer_interface> issuer_interface { get; set; }

        public virtual ICollection<product_interface> product_interface { get; set; }
    }
}
