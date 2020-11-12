namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_batch_statuses_flow
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int pin_batch_type_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int pin_batch_statuses_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int card_issue_method_id { get; set; }

        public int user_role_id { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int flow_pin_batch_statuses_id { get; set; }

        public int flow_pin_batch_type_id { get; set; }

        public short? main_menu_id { get; set; }

        public short? sub_menu_id { get; set; }

        public short? sub_menu_order { get; set; }

        public int? reject_pin_batch_statuses_id { get; set; }

        public int? reject_pin_card_statuses_id { get; set; }

        public int? flow_pin_card_statuses_id { get; set; }

        public virtual card_issue_method card_issue_method { get; set; }

        public virtual pin_batch_type pin_batch_type { get; set; }
    }
}
