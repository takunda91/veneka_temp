namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class response_messages
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int system_response_code { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int system_area { get; set; }

        [Required]
        [StringLength(500)]
        public string english_response { get; set; }

        [Required]
        [StringLength(500)]
        public string french_response { get; set; }

        [Required]
        [StringLength(500)]
        public string portuguese_response { get; set; }

        [Required]
        [StringLength(500)]
        public string spanish_response { get; set; }
    }
}
