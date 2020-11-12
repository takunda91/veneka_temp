namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_reissue_statuses
    {
        public pin_reissue_statuses()
        {
            pin_reissue_status = new HashSet<pin_reissue_status>();
            pin_reissue_statuses_language = new HashSet<pin_reissue_statuses_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int pin_reissue_statuses_id { get; set; }

        [Required]
        [StringLength(100)]
        public string pin_reissue_statuses_name { get; set; }

        public virtual ICollection<pin_reissue_status> pin_reissue_status { get; set; }

        public virtual ICollection<pin_reissue_statuses_language> pin_reissue_statuses_language { get; set; }
    }
}
