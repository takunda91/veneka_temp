namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class connection_parameter_type
    {
        public connection_parameter_type()
        {
            connection_parameter_type_language = new HashSet<connection_parameter_type_language>();
            connection_parameters = new HashSet<connection_parameters>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int connection_parameter_type_id { get; set; }

        [Required]
        [StringLength(50)]
        public string connection_parameter_type_name { get; set; }

        public virtual ICollection<connection_parameter_type_language> connection_parameter_type_language { get; set; }

        public virtual ICollection<connection_parameters> connection_parameters { get; set; }
    }
}
