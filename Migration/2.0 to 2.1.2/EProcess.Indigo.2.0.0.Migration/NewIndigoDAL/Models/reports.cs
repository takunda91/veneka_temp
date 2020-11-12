namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class reports
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Reportid { get; set; }

        [StringLength(50)]
        public string ReportName { get; set; }
    }
}
