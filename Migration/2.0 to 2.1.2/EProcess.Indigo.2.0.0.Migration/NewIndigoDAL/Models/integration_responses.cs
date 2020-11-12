namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class integration_responses
    {
        public integration_responses()
        {
            integration_responses_language = new HashSet<integration_responses_language>();
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int integration_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int integration_object_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int integration_field_id { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int integration_response_id { get; set; }

        [Required]
        public string integration_response_value { get; set; }

        public bool integration_response_valid_response { get; set; }

        public virtual integration_fields integration_fields { get; set; }

        public virtual ICollection<integration_responses_language> integration_responses_language { get; set; }
    }
}
