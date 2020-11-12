namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class branch_card_status
    {
        [Key]
        public long branch_card_status_id { get; set; }

        public long card_id { get; set; }

        public int branch_card_statuses_id { get; set; }

        public DateTime status_date { get; set; }

        public long user_id { get; set; }

        public long? operator_user_id { get; set; }

        public int? branch_card_code_id { get; set; }

        [StringLength(1000)]
        public string comments { get; set; }

        public long? pin_auth_user_id { get; set; }

        public virtual branch_card_codes branch_card_codes { get; set; }

        public virtual branch_card_statuses branch_card_statuses { get; set; }

        public virtual cards cards { get; set; }

        public virtual user user { get; set; }

        public virtual user user1 { get; set; }
    }
}
