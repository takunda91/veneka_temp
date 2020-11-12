namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class dist_batch_statuses_flow
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int issuer_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int dist_batch_type_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int dist_batch_statuses_id { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int card_issue_method_id { get; set; }

        public int user_role_id { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int flow_dist_batch_statuses_id { get; set; }

        public int flow_dist_batch_type_id { get; set; }

        public short? main_menu_id { get; set; }

        public short? sub_menu_id { get; set; }

        public short? sub_menu_order { get; set; }

        public int? reject_dist_batch_statuses_id { get; set; }

        public int? flow_dist_card_statuses_id { get; set; }

        public int? reject_dist_card_statuses_id { get; set; }

        public int? branch_card_statuses_id { get; set; }

        public int? reject_branch_card_statuses_id { get; set; }
    }
}
