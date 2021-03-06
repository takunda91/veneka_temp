USE [{DATABASE_NAME}]
GO
--Items needed on source DB
CREATE FUNCTION validation_products(	)
RETURNS TABLE 
AS
RETURN 
(
	SELECT [product_id]
      ,[product_code]
      ,[product_name]
      ,[product_bin_code]
      ,[issuer_id]
      ,[name_on_card_top]
      ,[name_on_card_left]
      ,[Name_on_card_font_size]
      ,[font_id]
      ,[DeletedYN]
      ,[src1_id]
      ,[src2_id]
      ,[src3_id]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [PVKI])) as [PVKI]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [PVK])) as [PVK]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [CVKA])) as [CVKA]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [CVKB])) as [CVKB]
      ,[expiry_months]
      ,[fee_scheme_id]
      ,[enable_instant_pin_YN]
      ,[min_pin_length]
      ,[max_pin_length]
      ,[enable_instant_pin_reissue_YN]
      ,[cms_exportable_YN]
      ,[product_load_type_id]
      ,[sub_product_code]
      ,[pin_calc_method_id]
      ,[auto_approve_batch_YN]
      ,[account_validation_YN]
      ,[pan_length]
      ,[pin_mailer_printing_YN]
      ,[pin_mailer_reprint_YN]
      ,[sub_product_id]
      ,[master_product_id]
      ,[card_issue_method_id]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [decimalisation_table])) as [decimalisation_table]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [pin_validation_data])) as [pin_validation_data]
      ,[pin_block_formatid]
      ,[production_dist_batch_status_flow]
      ,[distribution_dist_batch_status_flow]
      ,[charge_fee_to_issuing_branch_YN]
      ,[print_issue_card_YN]
      ,[allow_manual_uploaded_YN]
      ,[allow_reupload_YN]
      ,[remote_cms_update_YN]
      ,[charge_fee_at_cardrequest]
	  FROM [dbo].[issuer_product]
)
GO

CREATE FUNCTION validation_connection_parameters(	)
RETURNS TABLE 
AS
RETURN 
(
	SELECT [connection_parameter_id]
      ,[connection_name]
      ,[address]
      ,[port]
      ,[path]
      ,[protocol]
      ,[auth_type]
	  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [username])) as [username]
	  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [password])) as [password]
      ,[connection_parameter_type_id]
      ,[header_length]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [identification])) as [identification]
      ,[timeout_milli]
      ,[buffer_size]
      ,[doc_type]
      ,[name_of_file]
      ,[file_delete_YN]
      ,[file_encryption_type_id]
      ,[duplicate_file_check_YN]
      ,[private_key]
      ,[public_key]
      ,[domain_name]
      ,[is_external_auth]
	  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [remote_username])) as [remote_username]
	  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [remote_password])) as [remote_password]	  
  FROM [dbo].[connection_parameters]
)
GO

CREATE FUNCTION validation_users(	)
RETURNS TABLE 
AS
RETURN 
(

SELECT  [user_id]
	       ,[user_status_id]
	       ,[user_gender_id]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [username])) as [username]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [first_name])) as [first_name]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [last_name])) as [last_name]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [password])) as [password]
	       ,[user_email]
	       ,[online]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [employee_id])) as [employee_id]
	       ,[last_login_date]
	       ,[last_login_attempt]
	       ,[number_of_incorrect_logins]
	       ,[last_password_changed_date]
	       ,[workstation]
	       ,[language_id]
	       ,[username_index]
		   ,[connection_parameter_id]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [instant_authorisation_pin])) as [instant_authorisation_pin]
		   ,[last_authorisation_pin_changed_date]
		   ,[external_interface_id]
		   ,[time_zone_utcoffset]
	   	   ,[time_zone_id]
		   ,[useradmin_user_id]
		   ,[record_datetime]
		   ,[approved_user_id]
		   ,[approved_datetime]
	FROM [dbo].[user]
)
GO

CREATE FUNCTION validation_user_password_history(	)
RETURNS TABLE 
AS
RETURN 
(
	SELECT [user_id]
		,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [password_history])) as [password_history]
		,[date_changed]
	FROM [dbo].[user_password_history]
)
GO

CREATE FUNCTION validation_cards( )
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT [card_id]
			  ,[product_id]
			  ,[branch_id]
			  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, card_number)) as [card_number]
			  ,[card_sequence]			  
			  --,[card_index]
			  ,[card_issue_method_id]
			  ,[card_priority_id]
			  ,[card_request_reference]
			  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [card_production_date])) as [card_production_date]
			  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [card_expiry_date])) as [card_expiry_date]
			  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [card_activation_date])) as [card_activation_date]
			  ,[pvv]
			  ,[fee_charged]			  
			  ,[fee_waiver_YN]
			  ,[fee_editable_YN]
			  ,[fee_overridden_YN]
			  ,[fee_reference_number]
			  ,[fee_reversal_ref_number]
			  ,[origin_branch_id]
			  ,[export_batch_id]
			  ,[ordering_branch_id]
			  ,[delivery_branch_id]			  
		FROM [dbo].[cards]
)
GO

CREATE FUNCTION validation_customer_accounts(	)
RETURNS TABLE 
AS
RETURN 
(
	SELECT [customer_account_id]
      ,[user_id]
      ,[card_id]
      ,[card_issue_reason_id]
      ,[account_type_id]
      ,[currency_id]
      ,[resident_id]
      ,[customer_type_id]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [customer_account_number])) as [customer_account_number]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [customer_first_name])) as [customer_first_name]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [customer_middle_name])) as [customer_middle_name]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [customer_last_name])) as [customer_last_name]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [name_on_card])) as [name_on_card]
      ,[date_issued]
      ,[cms_id]
      ,[contract_number]
      ,[customer_title_id]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [Id_number])) as [Id_number]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [contact_number])) as [contact_number]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [CustomerId])) as [CustomerId]
      ,[domicile_branch_id]
  FROM [dbo].[customer_account]
)
GO

CREATE FUNCTION validation_customer_fields( )
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT [customer_account_id]
		  ,[product_field_id]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [value])) as [value]
	FROM [dbo].[customer_fields]
)
GO



CREATE FUNCTION validation_integration_cardnumbers( )
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [card_sequence_number])) as [card_sequence_number]
			,[product_id]
			,[sub_product_id]		  
		FROM [dbo].[integration_cardnumbers]
)
GO

CREATE FUNCTION validation_masterkeys( )
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT [masterkey_id]
		  ,[masterkey_name]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [masterkey])) as [masterkey]
		  ,[issuer_id]
		  ,[date_created]
		  ,[date_changed]		  
	FROM [dbo].[masterkeys]
)
GO

CREATE FUNCTION validation_terminals( )
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT [terminal_id]
		,[terminal_name]
		,[terminal_model]
		,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [device_id])) as [device_id]
		,[branch_id]
		,[terminal_masterkey_id]
		,[workstation]
		,[date_created]
		,[date_changed]
	FROM [dbo].[terminals]
)
GO

CREATE FUNCTION validation_pin_reissue( )
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT [issuer_id]
		  ,[branch_id]
		  ,[product_id]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [pan])) as [pan]
		  ,[reissue_date]	
		  ,[operator_user_id]
		  ,[authorise_user_id]
		  ,[failed]
		  ,[notes]
		  ,[pin_reissue_id]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [primary_index_number])) as [primary_index_number]
		  ,[request_expiry]
	FROM [dbo].[pin_reissue]
)
GO

CREATE FUNCTION validation_zone_keys( )
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT [issuer_id]
			,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('cert_ZoneMasterKeys'), NULL, [zone])) as [zone]
			,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('cert_ZoneMasterKeys'), NULL, [final])) as [final]
	FROM [dbo].[zone_keys]
)
GO

CREATE FUNCTION validation_product_fee_accounting( )
RETURNS TABLE 
AS
RETURN 
(
  SELECT [fee_accounting_id]
      ,[fee_accounting_name]
      ,[issuer_id]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('cert_ZoneMasterKeys'), NULL, [fee_revenue_account_no])) as [fee_revenue_account_no]
      ,[fee_revenue_account_type_id]
      ,[fee_revenue_branch_code]
      ,[fee_revenue_narration_en]
      ,[fee_revenue_narration_fr]
      ,[fee_revenue_narration_pt]
      ,[fee_revenue_narration_es]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('cert_ZoneMasterKeys'), NULL, [vat_account_no])) as [vat_account_no]
      ,[vat_account_type_id]
      ,[vat_account_branch_code]
      ,[vat_narration_en]
      ,[vat_narration_fr]
      ,[vat_narration_pt]
      ,[vat_narration_es]
  FROM [dbo].[product_fee_accounting]
)
GO

CREATE FUNCTION validation_integration_fields( )
RETURNS TABLE 
AS
RETURN 
(
  SELECT [integration_id]
      ,[integration_object_id]
      ,[integration_field_id]
      ,[integration_field_name]
      ,[accept_all_responses]
      ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('cert_ZoneMasterKeys'), NULL, [integration_field_default_value])) as [integration_field_default_value]
  FROM [dbo].[integration_fields]
)
GO