namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class languages
    {
        public languages()
        {
            audit_action_language = new HashSet<audit_action_language>();
            branch_card_codes_language = new HashSet<branch_card_codes_language>();
            branch_card_statuses_language = new HashSet<branch_card_statuses_language>();
            branch_statuses_language = new HashSet<branch_statuses_language>();
            card_issue_reason_language = new HashSet<card_issue_reason_language>();
            customer_account_type_language = new HashSet<customer_account_type_language>();
            customer_residency_language = new HashSet<customer_residency_language>();
            customer_title_language = new HashSet<customer_title_language>();
            customer_type_language = new HashSet<customer_type_language>();
            dist_batch_statuses_language = new HashSet<dist_batch_statuses_language>();
            dist_card_statuses_language = new HashSet<dist_card_statuses_language>();
            file_statuses_language = new HashSet<file_statuses_language>();
            flex_response_values_language = new HashSet<flex_response_values_language>();
            interface_type_language = new HashSet<interface_type_language>();
            issuer = new HashSet<issuer>();
            issuer_statuses_language = new HashSet<issuer_statuses_language>();
            load_batch_statuses_language = new HashSet<load_batch_statuses_language>();
            load_card_statuses_language = new HashSet<load_card_statuses_language>();
            user_roles_language = new HashSet<user_roles_language>();
            user_status_language = new HashSet<user_status_language>();
            user = new HashSet<user>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Required]
        public string language_name { get; set; }

        [Required]
        [StringLength(100)]
        public string language_name_fr { get; set; }

        [Required]
        [StringLength(100)]
        public string language_name_pt { get; set; }

        [Required]
        [StringLength(100)]
        public string language_name_sp { get; set; }

        public virtual ICollection<audit_action_language> audit_action_language { get; set; }

        public virtual ICollection<branch_card_codes_language> branch_card_codes_language { get; set; }

        public virtual ICollection<branch_card_statuses_language> branch_card_statuses_language { get; set; }

        public virtual ICollection<branch_statuses_language> branch_statuses_language { get; set; }

        public virtual ICollection<card_issue_reason_language> card_issue_reason_language { get; set; }

        public virtual ICollection<customer_account_type_language> customer_account_type_language { get; set; }

        public virtual ICollection<customer_residency_language> customer_residency_language { get; set; }

        public virtual ICollection<customer_title_language> customer_title_language { get; set; }

        public virtual ICollection<customer_type_language> customer_type_language { get; set; }

        public virtual ICollection<dist_batch_statuses_language> dist_batch_statuses_language { get; set; }

        public virtual ICollection<dist_card_statuses_language> dist_card_statuses_language { get; set; }

        public virtual ICollection<file_statuses_language> file_statuses_language { get; set; }

        public virtual ICollection<flex_response_values_language> flex_response_values_language { get; set; }

        public virtual ICollection<interface_type_language> interface_type_language { get; set; }

        public virtual ICollection<issuer> issuer { get; set; }

        public virtual ICollection<issuer_statuses_language> issuer_statuses_language { get; set; }

        public virtual ICollection<load_batch_statuses_language> load_batch_statuses_language { get; set; }

        public virtual ICollection<load_card_statuses_language> load_card_statuses_language { get; set; }

        public virtual ICollection<user_roles_language> user_roles_language { get; set; }

        public virtual ICollection<user_status_language> user_status_language { get; set; }

        public virtual ICollection<user> user { get; set; }
    }
}
