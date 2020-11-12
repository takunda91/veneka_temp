namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class user_status
    {
        public user_status()
        {
            user = new HashSet<user>();
            user_status_language = new HashSet<user_status_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int user_status_id { get; set; }

        [Required]
        [StringLength(20)]
        public string user_status_text { get; set; }

        public virtual ICollection<user> user { get; set; }

        public virtual ICollection<user_status_language> user_status_language { get; set; }
    }
}
