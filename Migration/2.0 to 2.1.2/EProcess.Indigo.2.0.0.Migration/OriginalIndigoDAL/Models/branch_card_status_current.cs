namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class branch_card_status_current
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long branch_card_status_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long card_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int branch_card_statuses_id { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime status_date { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long user_id { get; set; }

        public long? operator_user_id { get; set; }

        public int? branch_card_code_id { get; set; }

        [StringLength(1000)]
        public string comments { get; set; }

        public int? Expr1 { get; set; }

        public int? branch_card_code_type_id { get; set; }

        [StringLength(50)]
        public string branch_card_code_name { get; set; }

        public bool? branch_card_code_enabled { get; set; }

        public bool? spoil_only { get; set; }

        public bool? is_exception { get; set; }
    }
}
