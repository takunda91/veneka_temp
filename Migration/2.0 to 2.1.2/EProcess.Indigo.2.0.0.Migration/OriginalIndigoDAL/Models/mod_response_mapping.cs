namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class mod_response_mapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int response_mapping_id { get; set; }

        public int branch_card_code_id { get; set; }

        [Required]
        [StringLength(400)]
        public string response_contains { get; set; }

        public virtual branch_card_codes branch_card_codes { get; set; }
    }
}
