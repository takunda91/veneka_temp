namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class branch_statuses
    {
        public branch_statuses()
        {
            branch = new HashSet<branch>();
            branch_statuses_language = new HashSet<branch_statuses_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int branch_status_id { get; set; }

        [Required]
        [StringLength(15)]
        public string branch_status { get; set; }

        public virtual ICollection<branch> branch { get; set; }

        public virtual ICollection<branch_statuses_language> branch_statuses_language { get; set; }
    }
}
