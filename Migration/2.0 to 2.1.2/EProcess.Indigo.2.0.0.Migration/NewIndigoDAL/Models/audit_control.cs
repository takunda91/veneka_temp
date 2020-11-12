namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class audit_control
    {
        [Key]
        public long audit_id { get; set; }

        public int audit_action_id { get; set; }

        public long user_id { get; set; }

        public DateTime audit_date { get; set; }

        [Required]
        [StringLength(100)]
        public string workstation_address { get; set; }

        public string action_description { get; set; }

        public int? issuer_id { get; set; }

        [StringLength(30)]
        public string data_changed { get; set; }

        public string data_before { get; set; }

        public string data_after { get; set; }

        public virtual audit_action audit_action { get; set; }

        public virtual user user { get; set; }
    }
}
