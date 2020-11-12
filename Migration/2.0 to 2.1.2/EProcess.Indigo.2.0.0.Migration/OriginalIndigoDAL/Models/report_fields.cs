namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class report_fields
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int reportfieldid { get; set; }

        [StringLength(50)]
        public string reportfieldname { get; set; }
    }
}
