namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class file_history
    {
        public file_history()
        {
            load_batch = new HashSet<load_batch>();
        }

        [Key]
        public long file_id { get; set; }

        public int? issuer_id { get; set; }

        public int file_status_id { get; set; }

        public int file_type_id { get; set; }

        [Required]
        [StringLength(60)]
        public string name_of_file { get; set; }

        public DateTime file_created_date { get; set; }

        public int file_size { get; set; }

        public DateTime load_date { get; set; }

        [Required]
        [StringLength(110)]
        public string file_directory { get; set; }

        public int? number_successful_records { get; set; }

        public int? number_failed_records { get; set; }

        public string file_load_comments { get; set; }

        public int file_load_id { get; set; }

        public virtual file_load file_load { get; set; }

        public virtual file_types file_types { get; set; }

        public virtual file_statuses file_statuses { get; set; }

        public virtual issuer issuer { get; set; }

        public virtual ICollection<load_batch> load_batch { get; set; }
    }
}
