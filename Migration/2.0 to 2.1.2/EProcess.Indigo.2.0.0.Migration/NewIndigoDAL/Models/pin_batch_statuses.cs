namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_batch_statuses
    {
        public pin_batch_statuses()
        {
            pin_batch_status = new HashSet<pin_batch_status>();
            pin_batch_statuses_language = new HashSet<pin_batch_statuses_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int pin_batch_statuses_id { get; set; }

        [Required]
        [StringLength(100)]
        public string pin_batch_statuses_name { get; set; }

        public virtual ICollection<pin_batch_status> pin_batch_status { get; set; }

        public virtual ICollection<pin_batch_statuses_language> pin_batch_statuses_language { get; set; }
    }
}
