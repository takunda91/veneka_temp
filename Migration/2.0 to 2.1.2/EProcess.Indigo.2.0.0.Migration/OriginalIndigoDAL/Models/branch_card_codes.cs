namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class branch_card_codes
    {
        public branch_card_codes()
        {
            branch_card_codes_language = new HashSet<branch_card_codes_language>();
            branch_card_status = new HashSet<branch_card_status>();
            mod_response_mapping = new HashSet<mod_response_mapping>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int branch_card_code_id { get; set; }

        public int branch_card_code_type_id { get; set; }

        [Required]
        [StringLength(50)]
        public string branch_card_code_name { get; set; }

        public bool branch_card_code_enabled { get; set; }

        public bool spoil_only { get; set; }

        public bool is_exception { get; set; }

        public virtual branch_card_code_type branch_card_code_type { get; set; }

        public virtual ICollection<branch_card_codes_language> branch_card_codes_language { get; set; }

        public virtual ICollection<branch_card_status> branch_card_status { get; set; }

        public virtual ICollection<mod_response_mapping> mod_response_mapping { get; set; }
    }
}
