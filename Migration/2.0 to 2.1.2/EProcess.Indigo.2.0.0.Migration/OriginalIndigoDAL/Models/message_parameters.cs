namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class message_parameters
    {
        public message_parameters()
        {
            message_parameters_has_interface = new HashSet<message_parameters_has_interface>();
        }

        public int id { get; set; }

        [Required]
        public string interface_message { get; set; }

        [Required]
        public string indigo_message { get; set; }

        public virtual ICollection<message_parameters_has_interface> message_parameters_has_interface { get; set; }
    }
}
