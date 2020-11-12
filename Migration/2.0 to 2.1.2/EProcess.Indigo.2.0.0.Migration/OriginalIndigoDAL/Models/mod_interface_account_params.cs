namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class mod_interface_account_params
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(2)]
        public string BANK_C { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(2)]
        public string GROUPC { get; set; }

        public int issuer_id { get; set; }

        [Required]
        [StringLength(1)]
        public string STAT_CHANGE { get; set; }

        public decimal LIM_INTR { get; set; }

        public decimal NON_REDUCE_BAL { get; set; }

        public decimal CRD { get; set; }

        [Required]
        [StringLength(50)]
        public string CYCLE { get; set; }

        public int DEST_ACCNT_TYPE { get; set; }

        [Required]
        [StringLength(1)]
        public string REP_LANG { get; set; }

        public virtual issuer issuer { get; set; }
    }
}
