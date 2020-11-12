namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ldap_setting
    {
        public ldap_setting()
        {
            user = new HashSet<user>();
            user1 = new HashSet<user>();
        }

        [Key]
        public int ldap_setting_id { get; set; }

        [Required]
        [StringLength(100)]
        public string ldap_setting_name { get; set; }

        [Required]
        [StringLength(200)]
        public string hostname_or_ip { get; set; }

        [Required]
        [StringLength(200)]
        public string path { get; set; }

        [StringLength(100)]
        public string domain_name { get; set; }

        public byte[] username { get; set; }

        public byte[] password { get; set; }

        public virtual ICollection<user> user { get; set; }

        public virtual ICollection<user> user1 { get; set; }
    }
}
