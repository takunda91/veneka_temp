namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class user_gender
    {
        public user_gender()
        {
            user = new HashSet<user>();
        }

        [Key]
        public int user_gender_id { get; set; }

        [Required]
        [StringLength(15)]
        public string user_gender_text { get; set; }

        public virtual ICollection<user> user { get; set; }
    }
}
