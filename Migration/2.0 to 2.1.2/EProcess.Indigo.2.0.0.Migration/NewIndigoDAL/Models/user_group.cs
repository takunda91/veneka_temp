namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class user_group
    {
        public user_group()
        {
            branch = new HashSet<branch>();
            user = new HashSet<user>();
        }

        [Key]
        public int user_group_id { get; set; }

        public int user_role_id { get; set; }

        public int issuer_id { get; set; }

        public bool can_create { get; set; }

        public bool can_read { get; set; }

        public bool can_update { get; set; }

        public bool can_delete { get; set; }

        public bool all_branch_access { get; set; }

        [Required]
        [StringLength(50)]
        public string user_group_name { get; set; }

        public bool mask_screen_pan { get; set; }

        public bool mask_report_pan { get; set; }

        public virtual issuer issuer { get; set; }

        public virtual user_roles user_roles { get; set; }

        public virtual ICollection<branch> branch { get; set; }

        public virtual ICollection<user> user { get; set; }
    }
}
