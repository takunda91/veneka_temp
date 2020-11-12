namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("branch")]
    public partial class branch
    {
        public branch()
        {
            cards = new HashSet<cards>();
            customer_account = new HashSet<customer_account>();
            dist_batch = new HashSet<dist_batch>();
            cards1 = new HashSet<cards>();
            pin_batch = new HashSet<pin_batch>();
            pin_reissue = new HashSet<pin_reissue>();
            terminals = new HashSet<terminals>();
            user_group = new HashSet<user_group>();
        }

        [Key]
        public int branch_id { get; set; }

        public int branch_status_id { get; set; }

        public int issuer_id { get; set; }

        [Required]
        [StringLength(10)]
        public string branch_code { get; set; }

        [Required]
        [StringLength(30)]
        public string branch_name { get; set; }

        [StringLength(20)]
        public string location { get; set; }

        [StringLength(30)]
        public string contact_person { get; set; }

        [StringLength(30)]
        public string contact_email { get; set; }

        [StringLength(10)]
        public string card_centre { get; set; }

        public bool card_centre_branch_YN { get; set; }

        public virtual branch_statuses branch_statuses { get; set; }

        public virtual issuer issuer { get; set; }

        public virtual ICollection<cards> cards { get; set; }

        public virtual ICollection<customer_account> customer_account { get; set; }

        public virtual ICollection<dist_batch> dist_batch { get; set; }

        public virtual ICollection<cards> cards1 { get; set; }

        public virtual ICollection<pin_batch> pin_batch { get; set; }

        public virtual ICollection<pin_reissue> pin_reissue { get; set; }

        public virtual ICollection<terminals> terminals { get; set; }

        public virtual ICollection<user_group> user_group { get; set; }
    }
}
