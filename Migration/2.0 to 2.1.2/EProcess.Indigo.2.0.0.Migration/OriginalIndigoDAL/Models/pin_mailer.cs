namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_mailer
    {
        [StringLength(50)]
        public string pin_mailer_reference { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string batch_reference { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(25)]
        public string pin_mailer_status { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(19)]
        public string card_number { get; set; }

        [StringLength(4)]
        public string pvv_offset { get; set; }

        [StringLength(25)]
        public string encrypted_pin { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(25)]
        public string customer_name { get; set; }

        [StringLength(50)]
        public string customer_address { get; set; }

        public DateTime? printed_date { get; set; }

        public DateTime? reprinted_date { get; set; }

        public int? reprint_request_YN { get; set; }
    }
}
