namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class file_statuses
    {
        public file_statuses()
        {
            file_history = new HashSet<file_history>();
            file_statuses_language = new HashSet<file_statuses_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int file_status_id { get; set; }

        [Required]
        [StringLength(50)]
        public string file_status { get; set; }

        public virtual ICollection<file_history> file_history { get; set; }

        public virtual ICollection<file_statuses_language> file_statuses_language { get; set; }
    }
}
