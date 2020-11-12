namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_batch_type
    {
        public pin_batch_type()
        {
            pin_batch = new HashSet<pin_batch>();
            pin_batch_statuses_flow = new HashSet<pin_batch_statuses_flow>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int pin_batch_type_id { get; set; }

        [Required]
        [StringLength(100)]
        public string pin_batch_type_name { get; set; }

        public virtual ICollection<pin_batch> pin_batch { get; set; }

        public virtual ICollection<pin_batch_statuses_flow> pin_batch_statuses_flow { get; set; }
    }
}
