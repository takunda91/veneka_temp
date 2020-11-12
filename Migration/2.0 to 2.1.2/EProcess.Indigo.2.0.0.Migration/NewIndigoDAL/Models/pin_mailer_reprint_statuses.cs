namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_mailer_reprint_statuses
    {
        public pin_mailer_reprint_statuses()
        {
            pin_mailer_reprint = new HashSet<pin_mailer_reprint>();
            pin_mailer_reprint_statuses_language = new HashSet<pin_mailer_reprint_statuses_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int pin_mailer_reprint_status_id { get; set; }

        [Required]
        [StringLength(100)]
        public string pin_mailer_reprint_status_name { get; set; }

        public virtual ICollection<pin_mailer_reprint> pin_mailer_reprint { get; set; }

        public virtual ICollection<pin_mailer_reprint_statuses_language> pin_mailer_reprint_statuses_language { get; set; }
    }
}
