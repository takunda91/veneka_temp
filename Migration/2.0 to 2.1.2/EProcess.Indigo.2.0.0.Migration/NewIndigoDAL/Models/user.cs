namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("user")]
    public partial class user
    {
        public user()
        {
            audit_control = new HashSet<audit_control>();
            branch_card_status = new HashSet<branch_card_status>();
            branch_card_status1 = new HashSet<branch_card_status>();
            customer_account = new HashSet<customer_account>();
            dist_batch_status = new HashSet<dist_batch_status>();
            export_batch_status = new HashSet<export_batch_status>();
            load_batch_status = new HashSet<load_batch_status>();
            pin_batch_status = new HashSet<pin_batch_status>();
            pin_mailer_reprint = new HashSet<pin_mailer_reprint>();
            pin_reissue = new HashSet<pin_reissue>();
            pin_reissue1 = new HashSet<pin_reissue>();
            user_password_history = new HashSet<user_password_history>();
            user_group = new HashSet<user_group>();
        }

        [Key]
        public long user_id { get; set; }

        public int user_status_id { get; set; }

        public int user_gender_id { get; set; }

        [Required]
        [MaxLength(256)]
        public byte[] username { get; set; }

        [Required]
        [MaxLength(256)]
        public byte[] first_name { get; set; }

        [Required]
        [MaxLength(256)]
        public byte[] last_name { get; set; }

        [Required]
        [MaxLength(256)]
        public byte[] password { get; set; }

        [Required]
        [StringLength(100)]
        public string user_email { get; set; }

        public bool online { get; set; }

        [MaxLength(256)]
        public byte[] employee_id { get; set; }

        public DateTime? last_login_date { get; set; }

        public DateTime? last_login_attempt { get; set; }

        public int? number_of_incorrect_logins { get; set; }

        public DateTime? last_password_changed_date { get; set; }

        [StringLength(50)]
        public string workstation { get; set; }

        public int? language_id { get; set; }

        [MaxLength(20)]
        public byte[] username_index { get; set; }

        public int? ldap_setting_id { get; set; }

        [MaxLength(256)]
        public byte[] instant_authorisation_pin { get; set; }

        public DateTime? last_authorisation_pin_changed_date { get; set; }

        public virtual ICollection<audit_control> audit_control { get; set; }

        public virtual ICollection<branch_card_status> branch_card_status { get; set; }

        public virtual ICollection<branch_card_status> branch_card_status1 { get; set; }

        public virtual ICollection<customer_account> customer_account { get; set; }

        public virtual ICollection<dist_batch_status> dist_batch_status { get; set; }

        public virtual ICollection<export_batch_status> export_batch_status { get; set; }

        public virtual languages languages { get; set; }

        public virtual ldap_setting ldap_setting { get; set; }

        public virtual ldap_setting ldap_setting1 { get; set; }

        public virtual ICollection<load_batch_status> load_batch_status { get; set; }

        public virtual ICollection<pin_batch_status> pin_batch_status { get; set; }

        public virtual ICollection<pin_mailer_reprint> pin_mailer_reprint { get; set; }

        public virtual ICollection<pin_reissue> pin_reissue { get; set; }

        public virtual ICollection<pin_reissue> pin_reissue1 { get; set; }

        public virtual user_status user_status { get; set; }

        public virtual ICollection<user_password_history> user_password_history { get; set; }

        public virtual user_gender user_gender { get; set; }

        public virtual ICollection<user_group> user_group { get; set; }
    }
}
