namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class OldIndigoDataModel : DbContext
    {
        public OldIndigoDataModel()
            : base("name=OldIndigoDataModel")
        {
        }

        public virtual DbSet<audit_action> audit_action { get; set; }
        public virtual DbSet<audit_action_language> audit_action_language { get; set; }
        public virtual DbSet<audit_control> audit_control { get; set; }
        public virtual DbSet<branch> branch { get; set; }
        public virtual DbSet<branch_card_code_type> branch_card_code_type { get; set; }
        public virtual DbSet<branch_card_codes> branch_card_codes { get; set; }
        public virtual DbSet<branch_card_codes_language> branch_card_codes_language { get; set; }
        public virtual DbSet<branch_card_status> branch_card_status { get; set; }
        public virtual DbSet<branch_card_statuses> branch_card_statuses { get; set; }
        public virtual DbSet<branch_card_statuses_language> branch_card_statuses_language { get; set; }
        public virtual DbSet<branch_statuses> branch_statuses { get; set; }
        public virtual DbSet<branch_statuses_language> branch_statuses_language { get; set; }
        public virtual DbSet<card_issue_reason> card_issue_reason { get; set; }
        public virtual DbSet<card_issue_reason_language> card_issue_reason_language { get; set; }
        public virtual DbSet<cards> cards { get; set; }
        public virtual DbSet<connection_parameters> connection_parameters { get; set; }
        public virtual DbSet<country> country { get; set; }
        public virtual DbSet<currency> currency { get; set; }
        public virtual DbSet<customer_account> customer_account { get; set; }
        public virtual DbSet<customer_account_type> customer_account_type { get; set; }
        public virtual DbSet<customer_account_type_language> customer_account_type_language { get; set; }
        public virtual DbSet<customer_residency> customer_residency { get; set; }
        public virtual DbSet<customer_residency_language> customer_residency_language { get; set; }
        public virtual DbSet<customer_title> customer_title { get; set; }
        public virtual DbSet<customer_title_language> customer_title_language { get; set; }
        public virtual DbSet<customer_type> customer_type { get; set; }
        public virtual DbSet<customer_type_language> customer_type_language { get; set; }
        public virtual DbSet<dist_batch> dist_batch { get; set; }
        public virtual DbSet<dist_batch_cards> dist_batch_cards { get; set; }
        public virtual DbSet<dist_batch_status> dist_batch_status { get; set; }
        public virtual DbSet<dist_batch_statuses> dist_batch_statuses { get; set; }
        public virtual DbSet<dist_batch_statuses_language> dist_batch_statuses_language { get; set; }
        public virtual DbSet<dist_card_statuses> dist_card_statuses { get; set; }
        public virtual DbSet<dist_card_statuses_language> dist_card_statuses_language { get; set; }
        public virtual DbSet<file_history> file_history { get; set; }
        public virtual DbSet<file_load> file_load { get; set; }
        public virtual DbSet<file_statuses> file_statuses { get; set; }
        public virtual DbSet<file_statuses_language> file_statuses_language { get; set; }
        public virtual DbSet<file_types> file_types { get; set; }
        public virtual DbSet<flex_parameters> flex_parameters { get; set; }
        public virtual DbSet<flex_response_values> flex_response_values { get; set; }
        public virtual DbSet<flex_response_values_language> flex_response_values_language { get; set; }
        public virtual DbSet<flex_responses> flex_responses { get; set; }
        public virtual DbSet<interface_type> interface_type { get; set; }
        public virtual DbSet<interface_type_language> interface_type_language { get; set; }
        public virtual DbSet<issuer> issuer { get; set; }
        public virtual DbSet<issuer_interface> issuer_interface { get; set; }
        public virtual DbSet<issuer_product> issuer_product { get; set; }
        public virtual DbSet<Issuer_product_font> Issuer_product_font { get; set; }
        public virtual DbSet<issuer_statuses> issuer_statuses { get; set; }
        public virtual DbSet<issuer_statuses_language> issuer_statuses_language { get; set; }
        public virtual DbSet<languages> languages { get; set; }
        public virtual DbSet<ldap_setting> ldap_setting { get; set; }
        public virtual DbSet<load_batch> load_batch { get; set; }
        public virtual DbSet<load_batch_cards> load_batch_cards { get; set; }
        public virtual DbSet<load_batch_status> load_batch_status { get; set; }
        public virtual DbSet<load_batch_statuses> load_batch_statuses { get; set; }
        public virtual DbSet<load_batch_statuses_language> load_batch_statuses_language { get; set; }
        public virtual DbSet<load_card_failed> load_card_failed { get; set; }
        public virtual DbSet<load_card_statuses> load_card_statuses { get; set; }
        public virtual DbSet<load_card_statuses_language> load_card_statuses_language { get; set; }
        public virtual DbSet<mac_index_keys> mac_index_keys { get; set; }
        public virtual DbSet<message_parameters> message_parameters { get; set; }
        public virtual DbSet<message_parameters_has_interface> message_parameters_has_interface { get; set; }
        public virtual DbSet<mod_interface_account_params> mod_interface_account_params { get; set; }
        public virtual DbSet<mod_interface_cond_accnt> mod_interface_cond_accnt { get; set; }
        public virtual DbSet<mod_interface_general> mod_interface_general { get; set; }
        public virtual DbSet<mod_response_mapping> mod_response_mapping { get; set; }
        public virtual DbSet<pin_batch> pin_batch { get; set; }
        public virtual DbSet<report_fields> report_fields { get; set; }
        public virtual DbSet<report_reportfields> report_reportfields { get; set; }
        public virtual DbSet<reportfields_language> reportfields_language { get; set; }
        public virtual DbSet<reports> reports { get; set; }
        public virtual DbSet<response_messages> response_messages { get; set; }
        public virtual DbSet<user> user { get; set; }
        public virtual DbSet<user_gender> user_gender { get; set; }
        public virtual DbSet<user_group> user_group { get; set; }
        public virtual DbSet<user_roles> user_roles { get; set; }
        public virtual DbSet<user_roles_language> user_roles_language { get; set; }
        public virtual DbSet<user_status> user_status { get; set; }
        public virtual DbSet<user_status_language> user_status_language { get; set; }
        public virtual DbSet<BLK_ACCOUNTS> BLK_ACCOUNTS { get; set; }
        public virtual DbSet<cms_connection_config> cms_connection_config { get; set; }
        public virtual DbSet<flex_affiliate_codes> flex_affiliate_codes { get; set; }
        public virtual DbSet<pin_batch_status> pin_batch_status { get; set; }
        public virtual DbSet<pin_mailer> pin_mailer { get; set; }
        public virtual DbSet<system_parameters> system_parameters { get; set; }
        public virtual DbSet<TEMP_dist_cards> TEMP_dist_cards { get; set; }
        public virtual DbSet<user_password_history> user_password_history { get; set; }
        public virtual DbSet<branch_card_status_current> branch_card_status_current { get; set; }
        public virtual DbSet<dist_batch_status_current> dist_batch_status_current { get; set; }
        public virtual DbSet<load_batch_status_current> load_batch_status_current { get; set; }
        public virtual DbSet<user_group_branch_ex_ent> user_group_branch_ex_ent { get; set; }
        public virtual DbSet<user_roles_branch> user_roles_branch { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<audit_action>()
                .Property(e => e.audit_action_name)
                .IsUnicode(false);

            modelBuilder.Entity<audit_action>()
                .HasMany(e => e.audit_action_language)
                .WithRequired(e => e.audit_action)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<audit_action>()
                .HasMany(e => e.audit_control)
                .WithRequired(e => e.audit_action)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<audit_action_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<audit_control>()
                .Property(e => e.workstation_address)
                .IsUnicode(false);

            modelBuilder.Entity<audit_control>()
                .Property(e => e.action_description)
                .IsUnicode(false);

            modelBuilder.Entity<audit_control>()
                .Property(e => e.data_changed)
                .IsUnicode(false);

            modelBuilder.Entity<audit_control>()
                .Property(e => e.data_before)
                .IsUnicode(false);

            modelBuilder.Entity<audit_control>()
                .Property(e => e.data_after)
                .IsUnicode(false);

            modelBuilder.Entity<branch>()
                .Property(e => e.branch_code)
                .IsUnicode(false);

            modelBuilder.Entity<branch>()
                .Property(e => e.branch_name)
                .IsUnicode(false);

            modelBuilder.Entity<branch>()
                .Property(e => e.location)
                .IsUnicode(false);

            modelBuilder.Entity<branch>()
                .Property(e => e.contact_person)
                .IsUnicode(false);

            modelBuilder.Entity<branch>()
                .Property(e => e.contact_email)
                .IsUnicode(false);

            modelBuilder.Entity<branch>()
                .Property(e => e.card_centre)
                .IsUnicode(false);

            modelBuilder.Entity<branch>()
                .HasMany(e => e.cards)
                .WithRequired(e => e.branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<branch>()
                .HasMany(e => e.dist_batch)
                .WithRequired(e => e.branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<branch>()
                .HasMany(e => e.user_group)
                .WithMany(e => e.branch)
                .Map(m => m.ToTable("user_groups_branches").MapLeftKey("branch_id").MapRightKey("user_group_id"));

            modelBuilder.Entity<branch_card_code_type>()
                .Property(e => e.branch_card_code_name)
                .IsUnicode(false);

            modelBuilder.Entity<branch_card_code_type>()
                .HasMany(e => e.branch_card_codes)
                .WithRequired(e => e.branch_card_code_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<branch_card_codes>()
                .Property(e => e.branch_card_code_name)
                .IsUnicode(false);

            modelBuilder.Entity<branch_card_codes>()
                .HasMany(e => e.branch_card_codes_language)
                .WithRequired(e => e.branch_card_codes)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<branch_card_codes>()
                .HasMany(e => e.mod_response_mapping)
                .WithRequired(e => e.branch_card_codes)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<branch_card_codes_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<branch_card_status>()
                .Property(e => e.comments)
                .IsUnicode(false);

            modelBuilder.Entity<branch_card_statuses>()
                .Property(e => e.branch_card_statuses_name)
                .IsUnicode(false);

            modelBuilder.Entity<branch_card_statuses>()
                .HasMany(e => e.branch_card_status)
                .WithRequired(e => e.branch_card_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<branch_card_statuses>()
                .HasMany(e => e.branch_card_statuses_language)
                .WithRequired(e => e.branch_card_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<branch_card_statuses_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<branch_statuses>()
                .Property(e => e.branch_status)
                .IsUnicode(false);

            modelBuilder.Entity<branch_statuses>()
                .HasMany(e => e.branch)
                .WithRequired(e => e.branch_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<branch_statuses>()
                .HasMany(e => e.branch_statuses_language)
                .WithRequired(e => e.branch_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<branch_statuses_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<card_issue_reason>()
                .Property(e => e.card_issuer_reason_name)
                .IsUnicode(false);

            modelBuilder.Entity<card_issue_reason>()
                .HasMany(e => e.card_issue_reason_language)
                .WithRequired(e => e.card_issue_reason)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<card_issue_reason>()
                .HasMany(e => e.customer_account)
                .WithRequired(e => e.card_issue_reason)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<card_issue_reason_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<cards>()
                .HasMany(e => e.branch_card_status)
                .WithRequired(e => e.cards)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<cards>()
                .HasMany(e => e.customer_account)
                .WithRequired(e => e.cards)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<cards>()
                .HasMany(e => e.dist_batch_cards)
                .WithRequired(e => e.cards)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<cards>()
                .HasMany(e => e.load_batch_cards)
                .WithRequired(e => e.cards)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<connection_parameters>()
                .Property(e => e.connection_name)
                .IsUnicode(false);

            modelBuilder.Entity<connection_parameters>()
                .Property(e => e.address)
                .IsUnicode(false);

            modelBuilder.Entity<connection_parameters>()
                .Property(e => e.path)
                .IsUnicode(false);

            modelBuilder.Entity<connection_parameters>()
                .Property(e => e.protocol)
                .IsUnicode(false);

            modelBuilder.Entity<connection_parameters>()
                .HasMany(e => e.issuer_interface)
                .WithRequired(e => e.connection_parameters)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<country>()
                .Property(e => e.country_name)
                .IsUnicode(false);

            modelBuilder.Entity<country>()
                .Property(e => e.country_code)
                .IsUnicode(false);

            modelBuilder.Entity<country>()
                .Property(e => e.country_capital_city)
                .IsUnicode(false);

            modelBuilder.Entity<currency>()
                .Property(e => e.currency_code)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<currency>()
                .HasMany(e => e.issuer_product)
                .WithMany(e => e.currency)
                .Map(m => m.ToTable("product_currency").MapLeftKey("currency_id").MapRightKey("product_id"));

            modelBuilder.Entity<customer_account>()
                .Property(e => e.cms_id)
                .IsUnicode(false);

            modelBuilder.Entity<customer_account>()
                .Property(e => e.contract_number)
                .IsUnicode(false);

            modelBuilder.Entity<customer_account>()
                .Property(e => e.internal_account_nr)
                .IsUnicode(false);

            modelBuilder.Entity<customer_account_type>()
                .Property(e => e.account_type_name)
                .IsUnicode(false);

            modelBuilder.Entity<customer_account_type>()
                .HasMany(e => e.customer_account)
                .WithRequired(e => e.customer_account_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<customer_account_type>()
                .HasMany(e => e.customer_account_type_language)
                .WithRequired(e => e.customer_account_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<customer_account_type_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<customer_residency>()
                .Property(e => e.residency_name)
                .IsUnicode(false);

            modelBuilder.Entity<customer_residency>()
                .HasMany(e => e.customer_account)
                .WithRequired(e => e.customer_residency)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<customer_residency>()
                .HasMany(e => e.customer_residency_language)
                .WithRequired(e => e.customer_residency)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<customer_residency_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<customer_title>()
                .Property(e => e.customer_title_name)
                .IsUnicode(false);

            modelBuilder.Entity<customer_title>()
                .HasMany(e => e.customer_title_language)
                .WithRequired(e => e.customer_title)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<customer_title_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<customer_type>()
                .Property(e => e.customer_type_name)
                .IsUnicode(false);

            modelBuilder.Entity<customer_type>()
                .HasMany(e => e.customer_account)
                .WithRequired(e => e.customer_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<customer_type>()
                .HasMany(e => e.customer_type_language)
                .WithRequired(e => e.customer_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<customer_type_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<dist_batch>()
                .Property(e => e.dist_batch_reference)
                .IsUnicode(false);

            modelBuilder.Entity<dist_batch>()
                .HasMany(e => e.dist_batch_cards)
                .WithRequired(e => e.dist_batch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<dist_batch>()
                .HasMany(e => e.dist_batch_status)
                .WithRequired(e => e.dist_batch)
                .HasForeignKey(e => e.dist_batch_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<dist_batch>()
                .HasMany(e => e.dist_batch_status1)
                .WithRequired(e => e.dist_batch1)
                .HasForeignKey(e => e.dist_batch_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<dist_batch_status>()
                .Property(e => e.status_notes)
                .IsUnicode(false);

            modelBuilder.Entity<dist_batch_statuses>()
                .Property(e => e.dist_batch_status_name)
                .IsUnicode(false);

            modelBuilder.Entity<dist_batch_statuses>()
                .HasMany(e => e.dist_batch_status)
                .WithRequired(e => e.dist_batch_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<dist_batch_statuses>()
                .HasMany(e => e.dist_batch_statuses_language)
                .WithRequired(e => e.dist_batch_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<dist_batch_statuses_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<dist_card_statuses>()
                .Property(e => e.dist_card_status_name)
                .IsUnicode(false);

            modelBuilder.Entity<dist_card_statuses>()
                .HasMany(e => e.dist_batch_cards)
                .WithRequired(e => e.dist_card_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<dist_card_statuses>()
                .HasMany(e => e.dist_card_statuses_language)
                .WithRequired(e => e.dist_card_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<dist_card_statuses_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<file_history>()
                .Property(e => e.name_of_file)
                .IsUnicode(false);

            modelBuilder.Entity<file_history>()
                .Property(e => e.file_directory)
                .IsUnicode(false);

            modelBuilder.Entity<file_history>()
                .Property(e => e.file_load_comments)
                .IsUnicode(false);

            modelBuilder.Entity<file_history>()
                .HasMany(e => e.load_batch)
                .WithRequired(e => e.file_history)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<file_load>()
                .HasMany(e => e.file_history)
                .WithRequired(e => e.file_load)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<file_statuses>()
                .Property(e => e.file_status)
                .IsUnicode(false);

            modelBuilder.Entity<file_statuses>()
                .HasMany(e => e.file_history)
                .WithRequired(e => e.file_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<file_statuses>()
                .HasMany(e => e.file_statuses_language)
                .WithRequired(e => e.file_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<file_statuses_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<file_types>()
                .Property(e => e.file_type)
                .IsUnicode(false);

            modelBuilder.Entity<file_types>()
                .HasMany(e => e.file_history)
                .WithRequired(e => e.file_types)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<flex_parameters>()
                .Property(e => e.source_code)
                .IsUnicode(false);

            modelBuilder.Entity<flex_parameters>()
                .Property(e => e.request_token)
                .IsUnicode(false);

            modelBuilder.Entity<flex_parameters>()
                .Property(e => e.request_type)
                .IsUnicode(false);

            modelBuilder.Entity<flex_parameters>()
                .Property(e => e.source_channel_id)
                .IsUnicode(false);

            modelBuilder.Entity<flex_response_values>()
                .Property(e => e.flex_response_value)
                .IsUnicode(false);

            modelBuilder.Entity<flex_response_values_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<flex_responses>()
                .Property(e => e.flex_response_name)
                .IsUnicode(false);

            modelBuilder.Entity<flex_responses>()
                .HasMany(e => e.flex_response_values)
                .WithRequired(e => e.flex_responses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<interface_type>()
                .Property(e => e.interface_type_name)
                .IsUnicode(false);

            modelBuilder.Entity<interface_type>()
                .HasMany(e => e.interface_type_language)
                .WithRequired(e => e.interface_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<interface_type>()
                .HasMany(e => e.issuer_interface)
                .WithRequired(e => e.interface_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<interface_type_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<issuer>()
                .Property(e => e.issuer_name)
                .IsUnicode(false);

            modelBuilder.Entity<issuer>()
                .Property(e => e.issuer_code)
                .IsUnicode(false);

            modelBuilder.Entity<issuer>()
                .Property(e => e.license_file)
                .IsUnicode(false);

            modelBuilder.Entity<issuer>()
                .Property(e => e.license_key)
                .IsUnicode(false);

            modelBuilder.Entity<issuer>()
                .Property(e => e.cards_file_location)
                .IsUnicode(false);

            modelBuilder.Entity<issuer>()
                .Property(e => e.card_file_type)
                .IsUnicode(false);

            modelBuilder.Entity<issuer>()
                .Property(e => e.pin_file_location)
                .IsUnicode(false);

            modelBuilder.Entity<issuer>()
                .Property(e => e.pin_encrypted_ZPK)
                .IsUnicode(false);

            modelBuilder.Entity<issuer>()
                .Property(e => e.pin_mailer_file_type)
                .IsUnicode(false);

            modelBuilder.Entity<issuer>()
                .Property(e => e.pin_printer_name)
                .IsUnicode(false);

            modelBuilder.Entity<issuer>()
                .Property(e => e.pin_encrypted_PWK)
                .IsUnicode(false);

            modelBuilder.Entity<issuer>()
                .HasMany(e => e.branch)
                .WithRequired(e => e.issuer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<issuer>()
                .HasMany(e => e.flex_affiliate_codes)
                .WithRequired(e => e.issuer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<issuer>()
                .HasMany(e => e.issuer_interface)
                .WithRequired(e => e.issuer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<issuer>()
                .HasMany(e => e.issuer_product)
                .WithRequired(e => e.issuer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<issuer>()
                .HasMany(e => e.mod_interface_account_params)
                .WithRequired(e => e.issuer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<issuer>()
                .HasMany(e => e.user_group)
                .WithRequired(e => e.issuer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<issuer_product>()
                .Property(e => e.product_code)
                .IsUnicode(false);

            modelBuilder.Entity<issuer_product>()
                .Property(e => e.product_name)
                .IsUnicode(false);

            modelBuilder.Entity<issuer_product>()
                .Property(e => e.product_bin_code)
                .IsUnicode(false);

            modelBuilder.Entity<issuer_product>()
                .Property(e => e.name_on_card_top)
                .HasPrecision(8, 2);

            modelBuilder.Entity<issuer_product>()
                .Property(e => e.name_on_card_left)
                .HasPrecision(8, 2);

            modelBuilder.Entity<issuer_product>()
                .HasMany(e => e.cards)
                .WithRequired(e => e.issuer_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Issuer_product_font>()
                .Property(e => e.font_name)
                .IsUnicode(false);

            modelBuilder.Entity<Issuer_product_font>()
                .Property(e => e.resource_path)
                .IsUnicode(false);

            modelBuilder.Entity<Issuer_product_font>()
                .HasOptional(e => e.Issuer_product_font1)
                .WithRequired(e => e.Issuer_product_font2);

            modelBuilder.Entity<issuer_statuses>()
                .Property(e => e.issuer_status_name)
                .IsUnicode(false);

            modelBuilder.Entity<issuer_statuses>()
                .HasMany(e => e.issuer)
                .WithRequired(e => e.issuer_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<issuer_statuses>()
                .HasMany(e => e.issuer_statuses_language)
                .WithRequired(e => e.issuer_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<issuer_statuses_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<languages>()
                .Property(e => e.language_name_fr)
                .IsUnicode(false);

            modelBuilder.Entity<languages>()
                .Property(e => e.language_name_pt)
                .IsUnicode(false);

            modelBuilder.Entity<languages>()
                .Property(e => e.language_name_sp)
                .IsUnicode(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.audit_action_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.branch_card_codes_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.branch_card_statuses_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.branch_statuses_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.card_issue_reason_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.customer_account_type_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.customer_residency_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.customer_title_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.customer_type_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.dist_batch_statuses_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.dist_card_statuses_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.file_statuses_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.flex_response_values_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.interface_type_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.issuer)
                .WithOptional(e => e.languages)
                .HasForeignKey(e => e.language_id);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.issuer_statuses_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.load_batch_statuses_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.load_card_statuses_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.user_roles_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.user_status_language)
                .WithRequired(e => e.languages)
                .HasForeignKey(e => e.language_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<languages>()
                .HasMany(e => e.user)
                .WithOptional(e => e.languages)
                .HasForeignKey(e => e.language_id);

            modelBuilder.Entity<ldap_setting>()
                .Property(e => e.ldap_setting_name)
                .IsUnicode(false);

            modelBuilder.Entity<ldap_setting>()
                .Property(e => e.hostname_or_ip)
                .IsUnicode(false);

            modelBuilder.Entity<ldap_setting>()
                .Property(e => e.path)
                .IsUnicode(false);

            modelBuilder.Entity<ldap_setting>()
                .Property(e => e.domain_name)
                .IsUnicode(false);

            modelBuilder.Entity<load_batch>()
                .Property(e => e.load_batch_reference)
                .IsUnicode(false);

            modelBuilder.Entity<load_batch>()
                .HasMany(e => e.load_batch_cards)
                .WithRequired(e => e.load_batch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<load_batch>()
                .HasOptional(e => e.load_batch1)
                .WithRequired(e => e.load_batch2);

            modelBuilder.Entity<load_batch>()
                .HasMany(e => e.load_batch_status)
                .WithRequired(e => e.load_batch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<load_batch>()
                .HasMany(e => e.load_card_failed)
                .WithRequired(e => e.load_batch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<load_batch_status>()
                .Property(e => e.status_notes)
                .IsUnicode(false);

            modelBuilder.Entity<load_batch_statuses>()
                .Property(e => e.load_batch_status_name)
                .IsUnicode(false);

            modelBuilder.Entity<load_batch_statuses>()
                .HasMany(e => e.load_batch)
                .WithRequired(e => e.load_batch_statuses)
                .HasForeignKey(e => e.load_batch_status_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<load_batch_statuses>()
                .HasMany(e => e.load_batch_status)
                .WithRequired(e => e.load_batch_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<load_batch_statuses>()
                .HasMany(e => e.load_batch_statuses_language)
                .WithRequired(e => e.load_batch_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<load_batch_statuses_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<load_card_failed>()
                .Property(e => e.card_number)
                .IsUnicode(false);

            modelBuilder.Entity<load_card_failed>()
                .Property(e => e.card_sequence)
                .IsFixedLength();

            modelBuilder.Entity<load_card_failed>()
                .Property(e => e.load_batch_reference)
                .IsUnicode(false);

            modelBuilder.Entity<load_card_failed>()
                .Property(e => e.card_status)
                .IsUnicode(false);

            modelBuilder.Entity<load_card_statuses>()
                .Property(e => e.load_card_status)
                .IsUnicode(false);

            modelBuilder.Entity<load_card_statuses>()
                .HasMany(e => e.load_batch_cards)
                .WithRequired(e => e.load_card_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<load_card_statuses>()
                .HasMany(e => e.load_card_statuses_language)
                .WithRequired(e => e.load_card_statuses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<load_card_statuses_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<message_parameters>()
                .HasMany(e => e.message_parameters_has_interface)
                .WithRequired(e => e.message_parameters)
                .HasForeignKey(e => e.message_parameters_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<mod_interface_account_params>()
                .Property(e => e.BANK_C)
                .IsUnicode(false);

            modelBuilder.Entity<mod_interface_account_params>()
                .Property(e => e.GROUPC)
                .IsUnicode(false);

            modelBuilder.Entity<mod_interface_account_params>()
                .Property(e => e.STAT_CHANGE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<mod_interface_account_params>()
                .Property(e => e.LIM_INTR)
                .HasPrecision(6, 2);

            modelBuilder.Entity<mod_interface_account_params>()
                .Property(e => e.NON_REDUCE_BAL)
                .HasPrecision(12, 0);

            modelBuilder.Entity<mod_interface_account_params>()
                .Property(e => e.CRD)
                .HasPrecision(12, 0);

            modelBuilder.Entity<mod_interface_account_params>()
                .Property(e => e.CYCLE)
                .IsUnicode(false);

            modelBuilder.Entity<mod_interface_account_params>()
                .Property(e => e.REP_LANG)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<mod_interface_cond_accnt>()
                .Property(e => e.CCY)
                .IsUnicode(false);

            modelBuilder.Entity<mod_interface_cond_accnt>()
                .Property(e => e.COND_SET)
                .IsUnicode(false);

            modelBuilder.Entity<mod_interface_general>()
                .Property(e => e.requestID)
                .IsUnicode(false);

            modelBuilder.Entity<mod_response_mapping>()
                .Property(e => e.response_contains)
                .IsUnicode(false);

            modelBuilder.Entity<pin_batch>()
                .Property(e => e.batch_reference)
                .IsUnicode(false);

            modelBuilder.Entity<pin_batch>()
                .Property(e => e.manager_comment)
                .IsUnicode(false);

            modelBuilder.Entity<pin_batch>()
                .Property(e => e.batch_status)
                .IsUnicode(false);

            modelBuilder.Entity<pin_batch>()
                .Property(e => e.operator_comment)
                .IsUnicode(false);

            modelBuilder.Entity<pin_batch>()
                .Property(e => e.branch_code)
                .IsUnicode(false);

            modelBuilder.Entity<response_messages>()
                .Property(e => e.english_response)
                .IsUnicode(false);

            modelBuilder.Entity<response_messages>()
                .Property(e => e.french_response)
                .IsUnicode(false);

            modelBuilder.Entity<response_messages>()
                .Property(e => e.portuguese_response)
                .IsUnicode(false);

            modelBuilder.Entity<response_messages>()
                .Property(e => e.spanish_response)
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .Property(e => e.user_email)
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .Property(e => e.workstation)
                .IsFixedLength();

            modelBuilder.Entity<user>()
                .HasMany(e => e.audit_control)
                .WithRequired(e => e.user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user>()
                .HasMany(e => e.branch_card_status)
                .WithOptional(e => e.user)
                .HasForeignKey(e => e.operator_user_id);

            modelBuilder.Entity<user>()
                .HasMany(e => e.branch_card_status1)
                .WithRequired(e => e.user1)
                .HasForeignKey(e => e.user_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user>()
                .HasMany(e => e.customer_account)
                .WithRequired(e => e.user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user>()
                .HasMany(e => e.dist_batch_status)
                .WithRequired(e => e.user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user>()
                .HasMany(e => e.load_batch_status)
                .WithRequired(e => e.user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user>()
                .HasMany(e => e.user_password_history)
                .WithRequired(e => e.user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user>()
                .HasMany(e => e.user_group)
                .WithMany(e => e.user)
                .Map(m => m.ToTable("users_to_users_groups").MapLeftKey("user_id").MapRightKey("user_group_id"));

            modelBuilder.Entity<user_gender>()
                .Property(e => e.user_gender_text)
                .IsUnicode(false);

            modelBuilder.Entity<user_gender>()
                .HasMany(e => e.user)
                .WithRequired(e => e.user_gender)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user_group>()
                .Property(e => e.user_group_name)
                .IsUnicode(false);

            modelBuilder.Entity<user_roles>()
                .Property(e => e.user_role)
                .IsUnicode(false);

            modelBuilder.Entity<user_roles>()
                .HasMany(e => e.user_group)
                .WithRequired(e => e.user_roles)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user_roles>()
                .HasMany(e => e.user_roles_language)
                .WithRequired(e => e.user_roles)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user_roles_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<user_status>()
                .Property(e => e.user_status_text)
                .IsUnicode(false);

            modelBuilder.Entity<user_status>()
                .HasMany(e => e.user)
                .WithRequired(e => e.user_status)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user_status>()
                .HasMany(e => e.user_status_language)
                .WithRequired(e => e.user_status)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user_status_language>()
                .Property(e => e.language_text)
                .IsUnicode(false);

            modelBuilder.Entity<BLK_ACCOUNTS>()
                .Property(e => e.ACCOUNT)
                .IsFixedLength();

            modelBuilder.Entity<cms_connection_config>()
                .Property(e => e.connection_ip)
                .IsUnicode(false);

            modelBuilder.Entity<cms_connection_config>()
                .Property(e => e.connection_port)
                .IsUnicode(false);

            modelBuilder.Entity<cms_connection_config>()
                .Property(e => e.sign_on)
                .IsUnicode(false);

            modelBuilder.Entity<flex_affiliate_codes>()
                .Property(e => e.affiliate_code)
                .IsUnicode(false);

            modelBuilder.Entity<pin_batch_status>()
                .Property(e => e.batch_reference)
                .IsUnicode(false);

            modelBuilder.Entity<pin_batch_status>()
                .Property(e => e.batch_status)
                .IsUnicode(false);

            modelBuilder.Entity<pin_batch_status>()
                .Property(e => e.application_user)
                .IsUnicode(false);

            modelBuilder.Entity<pin_mailer>()
                .Property(e => e.pin_mailer_reference)
                .IsUnicode(false);

            modelBuilder.Entity<pin_mailer>()
                .Property(e => e.batch_reference)
                .IsUnicode(false);

            modelBuilder.Entity<pin_mailer>()
                .Property(e => e.pin_mailer_status)
                .IsUnicode(false);

            modelBuilder.Entity<pin_mailer>()
                .Property(e => e.card_number)
                .IsUnicode(false);

            modelBuilder.Entity<pin_mailer>()
                .Property(e => e.pvv_offset)
                .IsUnicode(false);

            modelBuilder.Entity<pin_mailer>()
                .Property(e => e.encrypted_pin)
                .IsUnicode(false);

            modelBuilder.Entity<pin_mailer>()
                .Property(e => e.customer_name)
                .IsUnicode(false);

            modelBuilder.Entity<pin_mailer>()
                .Property(e => e.customer_address)
                .IsUnicode(false);

            modelBuilder.Entity<system_parameters>()
                .Property(e => e.config)
                .IsUnicode(false);

            modelBuilder.Entity<system_parameters>()
                .Property(e => e.value)
                .IsUnicode(false);

            modelBuilder.Entity<TEMP_dist_cards>()
                .Property(e => e.seqeunce)
                .IsUnicode(false);

            modelBuilder.Entity<TEMP_dist_cards>()
                .Property(e => e.dist_batch_reference)
                .IsUnicode(false);

            modelBuilder.Entity<TEMP_dist_cards>()
                .Property(e => e.card_status)
                .IsUnicode(false);

            modelBuilder.Entity<TEMP_dist_cards>()
                .Property(e => e.issued_by)
                .IsUnicode(false);

            modelBuilder.Entity<TEMP_dist_cards>()
                .Property(e => e.customer_first_name)
                .IsUnicode(false);

            modelBuilder.Entity<TEMP_dist_cards>()
                .Property(e => e.customer_last_name)
                .IsUnicode(false);

            modelBuilder.Entity<TEMP_dist_cards>()
                .Property(e => e.account_type)
                .IsUnicode(false);

            modelBuilder.Entity<TEMP_dist_cards>()
                .Property(e => e.name_on_card)
                .IsUnicode(false);

            modelBuilder.Entity<TEMP_dist_cards>()
                .Property(e => e.branch_code)
                .IsUnicode(false);

            modelBuilder.Entity<TEMP_dist_cards>()
                .Property(e => e.reason_for_issue)
                .IsUnicode(false);

            modelBuilder.Entity<TEMP_dist_cards>()
                .Property(e => e.customer_title)
                .IsUnicode(false);

            modelBuilder.Entity<TEMP_dist_cards>()
                .Property(e => e.assigned_operator)
                .IsUnicode(false);

            modelBuilder.Entity<branch_card_status_current>()
                .Property(e => e.comments)
                .IsUnicode(false);

            modelBuilder.Entity<branch_card_status_current>()
                .Property(e => e.branch_card_code_name)
                .IsUnicode(false);

            modelBuilder.Entity<dist_batch_status_current>()
                .Property(e => e.status_notes)
                .IsUnicode(false);

            modelBuilder.Entity<load_batch_status_current>()
                .Property(e => e.status_notes)
                .IsUnicode(false);
        }
    }
}
