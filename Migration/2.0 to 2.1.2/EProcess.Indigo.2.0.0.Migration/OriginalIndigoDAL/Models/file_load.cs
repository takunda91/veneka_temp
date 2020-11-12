namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class file_load
    {
        public file_load()
        {
            file_history = new HashSet<file_history>();
        }

        [Key]
        public int file_load_id { get; set; }

        public DateTime file_load_start { get; set; }

        public DateTime? file_load_end { get; set; }

        public int user_id { get; set; }

        public int files_to_process { get; set; }

        public virtual ICollection<file_history> file_history { get; set; }
    }
}
