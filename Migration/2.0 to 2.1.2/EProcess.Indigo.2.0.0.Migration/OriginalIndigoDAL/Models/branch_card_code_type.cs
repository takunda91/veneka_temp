namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class branch_card_code_type
    {
        public branch_card_code_type()
        {
            branch_card_codes = new HashSet<branch_card_codes>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int branch_card_code_type_id { get; set; }

        [Required]
        [StringLength(50)]
        public string branch_card_code_name { get; set; }

        public virtual ICollection<branch_card_codes> branch_card_codes { get; set; }
    }
}
