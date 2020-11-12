namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class user_password_history
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long user_id { get; set; }

        [Required]
        public byte[] password_history { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime date_changed { get; set; }

        public virtual user user { get; set; }
    }
}
