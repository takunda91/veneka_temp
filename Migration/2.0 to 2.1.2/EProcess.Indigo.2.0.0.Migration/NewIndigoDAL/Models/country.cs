namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("country")]
    public partial class country
    {
        [Key]
        public int country_id { get; set; }

        [Required]
        [StringLength(100)]
        public string country_name { get; set; }

        [Required]
        [StringLength(3)]
        public string country_code { get; set; }

        [StringLength(100)]
        public string country_capital_city { get; set; }
    }
}
