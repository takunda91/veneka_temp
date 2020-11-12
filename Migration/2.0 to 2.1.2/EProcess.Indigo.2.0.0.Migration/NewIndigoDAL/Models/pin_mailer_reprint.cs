namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_mailer_reprint
    {
        [Key]
        public long pin_mailer_reprint_id { get; set; }

        public long card_id { get; set; }

        public long user_id { get; set; }

        public int pin_mailer_reprint_status_id { get; set; }

        public DateTime status_date { get; set; }

        [StringLength(1000)]
        public string comments { get; set; }

        public virtual cards cards { get; set; }

        public virtual pin_mailer_reprint_statuses pin_mailer_reprint_statuses { get; set; }

        public virtual user user { get; set; }
    }
}
