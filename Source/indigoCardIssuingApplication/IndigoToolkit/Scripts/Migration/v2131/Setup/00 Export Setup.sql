USE [{SOURCE_DATABASE_NAME}]
GO
--Items needed on source DB
CREATE FUNCTION migration_products(	)
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
      ,CAST(0 as BIT) as [charge_fee_at_cardrequest]
	  FROM [dbo].[issuer_product]
)
GO

CREATE FUNCTION migration_connection_parameters(	)
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
      ,ISNULL([file_delete_YN], CAST(0 as BIT)) as [file_delete_YN]
      ,[file_encryption_type_id]
      ,ISNULL([duplicate_file_check_YN], CAST(0 as BIT)) as [duplicate_file_check_YN]
      ,[private_key]
      ,[public_key]
      ,[domain_name]
      ,[is_external_auth]
	  ,[remote_port]
	  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [remote_username])) as [remote_username]
	  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [remote_password])) as [remote_password]
	  ,NULL AS [nonce]
  FROM [dbo].[connection_parameters]
)
GO

CREATE FUNCTION migration_users(	)
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
	       ,ToDateTimeOffset([last_login_date], DATENAME(tz, SYSDATETIMEOFFSET())) as [last_login_date]
	       ,ToDateTimeOffset([last_login_attempt], DATENAME(tz, SYSDATETIMEOFFSET())) as [last_login_attempt]
	       ,[number_of_incorrect_logins]
	       ,ToDateTimeOffset([last_password_changed_date], DATENAME(tz, SYSDATETIMEOFFSET())) as [last_password_changed_date]
	       ,[workstation]
	       ,[language_id]
	       ,[username_index]
		   ,[connection_parameter_id]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [instant_authorisation_pin])) as [instant_authorisation_pin]
		   ,ToDateTimeOffset([last_authorisation_pin_changed_date], DATENAME(tz, SYSDATETIMEOFFSET())) as [last_authorisation_pin_changed_date]
		   ,[external_interface_id]
		   ,'+00:00' as [time_zone_utcoffset]
	   	   ,'GMT Standard Time' as [time_zone_id]
		   ,NULL as [useradmin_user_id]
		   ,NULL as [record_datetime]
		   ,NULL as [approved_user_id]
		   ,NULL as [approved_datetime]
	FROM [dbo].[user]
)
GO

CREATE FUNCTION migration_user_password_history(	)
RETURNS TABLE 
AS
RETURN 
(
	SELECT [user_id]
		,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [password_history])) as [password_history]
		,ToDateTimeOffset([date_changed], DATENAME(tz, SYSDATETIMEOFFSET())) as [date_changed]
	FROM [dbo].[user_password_history]
)
GO

CREATE FUNCTION migration_cards( )
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
			  --,CONVERT(varbinary(24), 'HelloPeople') as [card_index]
			  ,[card_issue_method_id]
			  ,[card_priority_id]
			  ,[card_request_reference]
			  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [card_production_date])) as [card_production_date]
			  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [card_expiry_date])) as [card_expiry_date]
			  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [card_activation_date])) as [card_activation_date]
			  ,[pvv]
			  ,[fee_charged]
			  ,[vat]
			  ,[fee_waiver_YN]
			  ,[fee_editable_YN]
			  ,[fee_overridden_YN]
			  ,[fee_reference_number]
			  ,[fee_reversal_ref_number]
			  ,[origin_branch_id]
			  ,[export_batch_id]
			  ,[ordering_branch_id]
			  ,[delivery_branch_id]
			  ,[card_fee_charge_status_id]
		FROM [dbo].[cards]
)
GO

CREATE FUNCTION migration_customer_accounts(	)
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
      ,ToDateTimeOffset([date_issued], DATENAME(tz, SYSDATETIMEOFFSET())) as [date_issued]
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

CREATE FUNCTION migration_customer_fields( )
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



CREATE FUNCTION migration_integration_cardnumbers( )
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

CREATE FUNCTION migration_masterkeys( )
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT [masterkey_id]
		  ,[masterkey_name]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [masterkey])) as [masterkey]
		  ,[issuer_id]
		  ,ToDateTimeOffset([date_created], DATENAME(tz, SYSDATETIMEOFFSET())) as [date_created]
		  ,ToDateTimeOffset([date_changed], DATENAME(tz, SYSDATETIMEOFFSET())) as [date_changed]		  
	FROM [dbo].[masterkeys]
)
GO

CREATE FUNCTION migration_terminals( )
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
		,ToDateTimeOffset([date_created], DATENAME(tz, SYSDATETIMEOFFSET())) as [date_created]
		,ToDateTimeOffset([date_changed], DATENAME(tz, SYSDATETIMEOFFSET())) as [date_changed]
		,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [password])) as [password]
		,[IsMacUsed]
	FROM [dbo].[terminals]
)
GO

CREATE FUNCTION migration_pin_reissue( )
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT [issuer_id]
		  ,[branch_id]
		  ,[product_id]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [pan])) as [pan]
		  ,ToDateTimeOffset([reissue_date], DATENAME(tz, SYSDATETIMEOFFSET())) as [reissue_date]	
		  ,[operator_user_id]
		  ,[authorise_user_id]
		  ,[failed]
		  ,[notes]
		  ,[pin_reissue_id]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [primary_index_number])) as [primary_index_number]
		  ,ToDateTimeOffset([request_expiry], DATENAME(tz, SYSDATETIMEOFFSET())) as [request_expiry]
	FROM [dbo].[pin_reissue]
)
GO

CREATE FUNCTION migration_zone_keys( )
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

CREATE FUNCTION migration_product_fee_accounting( )
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

CREATE FUNCTION migration_integration_fields( )
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

----------------------------------------------------------------------------------------------------------------------------------------------------------------------
--Items needed on target DB
USE [{DATABASE_NAME}]
GO

CREATE PROCEDURE [dbo].[migration_products]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate

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
      ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([PVKI], '')) as [PVKI]
      ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([PVK], '')) as [PVK]
      ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([CVKA], '')) as [CVKA]
      ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([CVKB], '')) as [CVKB]
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
      ,CASE WHEN [decimalisation_table] IS NOT NULL THEN ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [decimalisation_table]) ELSE NULL END as [decimalisation_table]
	  ,CASE WHEN [pin_validation_data] IS NOT NULL THEN ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [pin_validation_data]) ELSE NULL END as [pin_validation_data]      
      ,[pin_block_formatid]
      ,[production_dist_batch_status_flow]
      ,[distribution_dist_batch_status_flow]
      ,[charge_fee_to_issuing_branch_YN]
      ,[print_issue_card_YN]
      ,[allow_manual_uploaded_YN]
      ,[allow_reupload_YN]
      ,[remote_cms_update_YN]
      ,[charge_fee_at_cardrequest]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_products]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO



CREATE PROCEDURE [dbo].[migration_connection_parameters]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate


	SELECT [connection_parameter_id]
      ,[connection_name]
      ,[address]
      ,[port]
      ,[path]
      ,[protocol]
      ,[auth_type]
	  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([username], '')) as [username]
	  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([password], '')) as [password]
      ,[connection_parameter_type_id]
      ,[header_length]
      ,CASE WHEN [identification] IS NOT NULL THEN ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [identification]) ELSE NULL END as [identification]
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
	  ,[remote_port]
	  ,CASE WHEN [remote_username] IS NOT NULL THEN ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [remote_username]) ELSE NULL END as [remote_username]
	  ,CASE WHEN [remote_password] IS NOT NULL THEN ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [remote_password]) ELSE NULL END as [remote_password]
	  ,NULL AS [nonce]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_connection_parameters]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

CREATE PROCEDURE [dbo].[migration_users]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @objid INT

	SET @objid = object_id('user')			
	DECLARE @key varbinary(100)
	SET @key = null
	SELECT @key = DecryptByKeyAutoCert(cert_id('cert_ProtectIndexingKeys'), null, mac_key) 
	FROM mac_index_keys 
	WHERE table_id = @objid

	IF(@key IS NULL)
		RAISERROR (N'MAC Index Key is null.', 15, 1);

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	 SELECT 		  
		    CONVERT(bigint, [user_id]) as [user_id]
	       ,CONVERT(int, [user_status_id]) as [user_status_id]
	       ,CONVERT(int, [user_gender_id]) as [user_gender_id]
	       ,CONVERT(varbinary(256), ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [username])) as [username]
	       ,CONVERT(varbinary(256), ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [first_name])) as [first_name]
	       ,CONVERT(varbinary(256), ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [last_name])) as [last_name]
	       ,CONVERT(varbinary(256), ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [password])) as [password]
	       ,CONVERT(VARCHAR(100), [user_email]) as [user_email]
	       ,CONVERT(bit, [online]) as [online]
		   ,CONVERT(varbinary(256), ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [employee_id])) as [employee_id]
	       ,[last_login_date]
	       ,[last_login_attempt]
	       ,CONVERT(int, [number_of_incorrect_logins]) as [number_of_incorrect_logins]
	       ,[last_password_changed_date]
		   ,CONVERT(nchar(50), [workstation]) as [workstation]
	       ,CONVERT(int, [language_id]) as [language_id]
	       ,CONVERT(varbinary(20),HashBytes( N'SHA1', CONVERT(varbinary(8000), [username]) + @key )) as [username_index] --one
	       ,CONVERT(varbinary(256), ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [instant_authorisation_pin])) as [instant_authorisation_pin]
		   ,[last_authorisation_pin_changed_date]
		   ,NULL as authentication_configuration_id
		   ,[time_zone_utcoffset]
		   ,[time_zone_id]
		   ,[useradmin_user_id]
		   ,[record_datetime]
		   ,[approved_user_id]
		   ,[approved_datetime]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_users]() 	

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

CREATE PROCEDURE [dbo].[migration_user_password_history]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	 SELECT [user_id]
		,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [password_history]) as [password_history]
		,[date_changed]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_user_password_history]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

CREATE PROCEDURE [dbo].[migration_cards]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @objid INT

	SET @objid = object_id('cards')			
				DECLARE @key varbinary(100)
				SET @key = null
				SELECT @key = DecryptByKeyAutoCert(cert_id('cert_ProtectIndexingKeys'), null, mac_key) 
				FROM mac_index_keys 
				WHERE table_id = @objid

	IF(@key IS NULL)
		RAISERROR (N'MAC Index Key is null.', 15, 1);

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	 SELECT [card_id]
		  ,[product_id]
		  ,[branch_id]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [card_number]) as [card_number]
		  ,[card_sequence]
		  ,CONVERT(varbinary(24),HashBytes( N'SHA1', CONVERT(varbinary(8000), CONVERT(nvarchar(4000),RIGHT([card_number], 4))) + @key )) as [card_index]
		  ,[card_issue_method_id]
		  ,[card_priority_id]
		  ,[card_request_reference]
          ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [card_production_date]) as [card_production_date]
          ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [card_expiry_date]) as [card_expiry_date]
          ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [card_activation_date]) as [card_activation_date]
          ,[pvv]
          ,[fee_charged]
		  ,[vat]
          ,[fee_waiver_YN]
          ,[fee_editable_YN]
          ,[fee_overridden_YN]
          ,[fee_reference_number]
          ,[fee_reversal_ref_number]
          ,[origin_branch_id]
          ,[export_batch_id]
		  ,[ordering_branch_id]
		  ,[delivery_branch_id]
		  ,[card_fee_charge_status_id]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_cards]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

CREATE PROCEDURE [dbo].[migration_customer_accounts]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	 SELECT 
		  [customer_account_id]
		  ,[user_id]
		  ,[card_id]
		  ,[card_issue_reason_id]
		  ,[account_type_id]
		  ,[currency_id]
		  ,[resident_id]
		  ,[customer_type_id]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [customer_account_number]) as [customer_account_number]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [customer_first_name]) as [customer_first_name]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [customer_middle_name]) as [customer_middle_name]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [customer_last_name]) as [customer_last_name]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [name_on_card]) as [name_on_card]
		  ,[date_issued]
		  ,[cms_id]
		  ,[contract_number]
		  ,[customer_title_id]
		  ,CASE WHEN [Id_number] IS NOT NULL THEN ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [Id_number]) ELSE NULL END as [Id_number]
		  ,CASE WHEN [contact_number] IS NOT NULL THEN ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [contact_number]) ELSE NULL END as [contact_number]
		  ,CASE WHEN [CustomerId] IS NOT NULL THEN ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [CustomerId]) ELSE NULL END as [CustomerId]
		  ,[domicile_branch_id]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_customer_accounts]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

CREATE PROCEDURE [dbo].[migration_customer_fields]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	SELECT [customer_account_id]
		  ,[product_field_id]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [value]) as [value]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_customer_fields]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

CREATE PROCEDURE [dbo].[migration_integration_cardnumbers]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	 SELECT 
		   [product_id]
		  ,[sub_product_id]	
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [card_sequence_number]) as [card_sequence_number]		  
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_integration_cardnumbers]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

CREATE PROCEDURE [dbo].[migration_masterkeys]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	 SELECT 
		   [masterkey_id]
		  ,[masterkey_name]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [masterkey]) as [masterkey]
		  ,[issuer_id]
		  ,[date_created]
		  ,[date_changed]	  
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_masterkeys]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

CREATE PROCEDURE [dbo].[migration_terminals]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	 SELECT 
			[terminal_id]
		  ,[terminal_name]
		  ,[terminal_model]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [device_id]) as [device_id]
		  ,[branch_id]
		  ,[terminal_masterkey_id]
		  ,[workstation]
		  ,[date_created]
		  ,[date_changed]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [password]) as [password]
		  ,[IsMacUsed]		  
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_terminals]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

CREATE PROCEDURE [dbo].[migration_pin_reissue]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	 SELECT 
		   [issuer_id]
		  ,[branch_id]
		  ,[product_id]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [pan]) as [pan]
		  ,[reissue_date]
		  ,[operator_user_id]
		  ,[authorise_user_id]
		  ,[failed]
		  ,[notes]
		  ,[pin_reissue_id]	
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [primary_index_number]) as [primary_index_number]  
		  ,[request_expiry]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_pin_reissue]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

CREATE PROCEDURE [dbo].[migration_zone_keys]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY key_injection_keys		
	DECRYPTION BY CERTIFICATE cert_ZoneMasterKeys;

	 SELECT 
		   [issuer_id]
		  ,ENCRYPTBYKEY(KEY_GUID('key_injection_keys'), [zone]) as [zone]  
		  ,ENCRYPTBYKEY(KEY_GUID('key_injection_keys'), [final]) as [final]  
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_zone_keys]()

	CLOSE SYMMETRIC KEY key_injection_keys;
END
GO

CREATE PROCEDURE [dbo].[migration_product_fee_accounting]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	SELECT  [fee_accounting_id]
		   ,[fee_accounting_name]
           ,[issuer_id]
           ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([fee_revenue_account_no], '')) as [fee_revenue_account_no]
           ,[fee_revenue_account_type_id]
           ,[fee_revenue_branch_code]
           ,[fee_revenue_narration_en]
           ,[fee_revenue_narration_fr]
           ,[fee_revenue_narration_pt]
           ,[fee_revenue_narration_es]
           ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([vat_account_no], '')) as [vat_account_no]
           ,[vat_account_type_id]
           ,[vat_account_branch_code]
           ,[vat_narration_en]
           ,[vat_narration_fr]
           ,[vat_narration_pt]
           ,[vat_narration_es]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_product_fee_accounting]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO


CREATE PROCEDURE [dbo].[migration_integration_fields]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	SELECT [integration_id]
		  ,[integration_object_id]
		  ,[integration_field_id]
		  ,[integration_field_name]
		  ,[accept_all_responses]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [integration_field_default_value]) as [integration_field_default_value]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_integration_fields]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

-- Validation Checks