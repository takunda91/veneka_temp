namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TEMP_dist_cards
    {
        [Key]
        [Column(Order = 0)]
        public long card_id { get; set; }

        [Key]
        [Column(Order = 1)]
        public byte[] card_number { get; set; }

        [StringLength(3)]
        public string seqeunce { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(25)]
        public string dist_batch_reference { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(25)]
        public string card_status { get; set; }

        public DateTime? date_issued { get; set; }

        [StringLength(20)]
        public string issued_by { get; set; }

        [StringLength(20)]
        public string customer_first_name { get; set; }

        [StringLength(20)]
        public string customer_last_name { get; set; }

        public byte[] customer_account { get; set; }

        [StringLength(10)]
        public string account_type { get; set; }

        [StringLength(30)]
        public string name_on_card { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int issuer_id { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(10)]
        public string branch_code { get; set; }

        [StringLength(50)]
        public string reason_for_issue { get; set; }

        [StringLength(10)]
        public string customer_title { get; set; }

        [StringLength(30)]
        public string assigned_operator { get; set; }
    }
}
