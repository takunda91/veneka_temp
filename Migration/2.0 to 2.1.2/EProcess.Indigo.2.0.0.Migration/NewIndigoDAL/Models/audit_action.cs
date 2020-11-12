namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class audit_action
    {
        public audit_action()
        {
            audit_action_language = new HashSet<audit_action_language>();
            audit_control = new HashSet<audit_control>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int audit_action_id { get; set; }

        [Required]
        [StringLength(100)]
        public string audit_action_name { get; set; }

        public virtual ICollection<audit_action_language> audit_action_language { get; set; }

        public virtual ICollection<audit_control> audit_control { get; set; }
    }
}
