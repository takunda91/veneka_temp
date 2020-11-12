namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class user_roles
    {
        public user_roles()
        {
            user_group = new HashSet<user_group>();
            user_roles_language = new HashSet<user_roles_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int user_role_id { get; set; }

        [Required]
        [StringLength(50)]
        public string user_role { get; set; }

        public bool allow_multiple_login { get; set; }

        public bool enterprise_only { get; set; }

        public virtual ICollection<user_group> user_group { get; set; }

        public virtual ICollection<user_roles_language> user_roles_language { get; set; }
    }
}
