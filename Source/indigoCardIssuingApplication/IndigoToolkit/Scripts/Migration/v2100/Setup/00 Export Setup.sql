USE [{SOURCE_DATABASE_NAME}]
GO

--Items needed on source DB
CREATE TABLE temp_migrate_products
(
	new_product_id INT IDENTITY (1, 1) NOT NULL,
	product_id INT NOT NULL,
	sub_product_id INT NOT NULL,
	bin_code VARCHAR(25) NOT NULL
)
GO

INSERT INTO temp_migrate_products (product_id, sub_product_id, bin_code)
SELECT [issuer_product].product_id , [sub_product].sub_product_id,
			product_bin_code + RIGHT('000'+CAST(ISNULL([sub_product].sub_product_id,'') AS VARCHAR(3)),[issuer_product].sub_product_id_length)
FROM [sub_product] INNER JOIN [issuer_product]
		ON [sub_product].product_id = [issuer_product].product_id
	INNER JOIN [issuer] 
		ON [issuer_product].issuer_id = [issuer].issuer_id
UNION
SELECT
	[issuer_product].product_id , -1, product_bin_code 
FROM [issuer_product] 
	INNER JOIN [issuer] ON [issuer_product].issuer_id = [issuer].issuer_id
WHERE product_id NOT IN (SELECT product_id FROM [sub_product])

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
      ,(CASE WHEN [port] = 0 THEN 1 ELSE 0 END) as [connection_parameter_type_id]
      ,NULL as [header_length]
      ,NULL as [identification]
      ,NULL as [timeout_milli]
      ,NULL as [buffer_size]
      ,CONVERT(CHAR(1), NULL) as [doc_type]
      ,CONVERT(VARCHAR(100), NULL) as [name_of_file]
      ,NULL as [file_delete_YN]
      ,NULL as [file_encryption_type_id]
      ,NULL as [duplicate_file_check_YN]
      ,NULL as [private_key]
      ,NULL as [public_key]
      ,CONVERT(VARCHAR(100), NULL) as [domain_name]
      ,CONVERT(BIT, 0) as [is_external_auth]
	  ,NULL as remote_username
      ,NULL as remote_password
      ,NULL as nonce
  FROM [dbo].[connection_parameters]
)
GO

CREATE FUNCTION migration_ldap(	)
RETURNS TABLE 
AS
RETURN 
(
	SELECT  [ldap_setting_name]
			,[hostname_or_ip]
			,0 as [port]
			,[path]
			,0 as [protocol]
			,0 as [auth_type]
			,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL,[username])) as [username]
			,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL,[password])) as [password]
			,4 as [connection_parameter_type_id] 			
			,NULL as [header_length]
			,NULL as [identification]
			,NULL as [timeout_milli]
			,NULL as [buffer_size]
			,CONVERT(CHAR(1), NULL) as [doc_type]
			,CONVERT(VARCHAR(100), NULL) as [name_of_file]
			,0 as [file_delete_YN]
			,0 as [file_encryption_type_id]
			,0 as [duplicate_file_check_YN]
			,NULL as [private_key]
			,NULL as [public_key]
			,[domain_name]
			,0 as [is_external_auth]
			
	FROM [dbo].[ldap_setting]
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
	       ,[last_login_date]
	       ,[last_login_attempt]
	       ,[number_of_incorrect_logins]
	       ,[last_password_changed_date]
	       ,[workstation]
	       ,[language_id]
	       ,[username_index]
		  ,CONVERT(int, NULL) as [connection_parameter_id]
	       ,'' as [instant_authorisation_pin]
		   ,CONVERT(datetime, NULL) as [last_authorisation_pin_changed_date]
		   ,CONVERT(char(36), NULL) as [external_inferface_id]
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
		,[date_changed]
	FROM [dbo].[user_password_history]
)
GO

CREATE FUNCTION migrate_products()
RETURNS TABLE
AS
RETURN
(
	SELECT 
		temp.new_product_id as product_id,
		[issuer_product].product_code, [issuer_product].product_name, 
		[product_bin_code], [issuer_product].[issuer_id],[name_on_card_top],[name_on_card_left],
		[Name_on_card_font_size],[font_id],[DeletedYN],[src1_id],[src2_id],[src3_id],
		CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, PVKI)) as [PVKI],
		CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, PVK)) as [PVK],
		CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, CVKA)) as [CVKA],
		CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, CVKB)) as [CVKB],
		[expiry_months],
		[sub_product].card_issue_method_id,
		ISNULL([sub_product].[fee_scheme_id], [issuer_product].[fee_scheme_id]) as [fee_scheme_id],
		[issuer_product].[enable_instant_pin_YN],[min_pin_length],[max_pin_length],[issuer_product].[enable_instant_pin_reissue_YN],
		RIGHT('000'+CAST(ISNULL([sub_product].sub_product_id,'') AS VARCHAR(3)),[issuer_product].sub_product_id_length) as sub_product_code,
		[sub_product].sub_product_id, [issuer_product].product_id as master_product_id,
		account_validation_YN, [pin_mailer_printing_YN], [pin_mailer_reprint_YN]
	FROM [sub_product] INNER JOIN [issuer_product]
			ON [sub_product].product_id = [issuer_product].product_id
		INNER JOIN [issuer] 
			ON [issuer_product].issuer_id = [issuer].issuer_id
		INNER JOIN temp_migrate_products as temp
			ON [sub_product].product_id = temp.product_id
				AND [sub_product].sub_product_id = temp.sub_product_id
	UNION
	SELECT
		temp.new_product_id as product_id,
		[issuer_product].product_code, [issuer_product].product_name, 
		[product_bin_code], [issuer_product].[issuer_id],[name_on_card_top],[name_on_card_left],
		[Name_on_card_font_size],[font_id],[DeletedYN],[src1_id],[src2_id],[src3_id],
		CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, PVKI)) as [PVKI],
		CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, PVK)) as [PVK],
		CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, CVKA)) as [CVKA],
		CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, CVKB)) as [CVKB],
		[expiry_months],
		[issuer_product].card_issue_method_id,
		[issuer_product].[fee_scheme_id],
		[issuer_product].[enable_instant_pin_YN],[min_pin_length],[max_pin_length],[issuer_product].[enable_instant_pin_reissue_YN],
		null as sub_product_code,
		-1 as sub_product_id, [issuer_product].product_id as master_product_id,
		account_validation_YN, [pin_mailer_printing_YN], [pin_mailer_reprint_YN]
	FROM [issuer_product] 
		INNER JOIN [issuer] ON [issuer_product].issuer_id = [issuer].issuer_id
		INNER JOIN temp_migrate_products as temp
			ON [issuer_product].product_id = temp.product_id
				AND temp.sub_product_id = -1
	WHERE [issuer_product].product_id NOT IN (SELECT product_id FROM [sub_product])
)
GO

CREATE FUNCTION migration_cards( )
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT [card_id]
			  ,temp.[new_product_id] as [product_id]
			  ,[branch_id]
			  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, card_number)) as [card_number]
			  ,[card_sequence]
			  ,CONVERT(varbinary(24), 'HelloPeople') as [card_index]
			  ,[card_issue_method_id]
			  ,[card_priority_id]
			  ,[card_request_reference]
			  ,[card_production_date]
			  ,[card_expiry_date]
			  ,[card_activation_date]
			  ,[pvv]
			  ,[fee_charged]
			  ,[fee_waiver_YN]
			  ,[fee_editable_YN]
			  ,[fee_overridden_YN]
			  ,[fee_reference_number]
			  ,[fee_reversal_ref_number]
			  ,[origin_branch_id]
			  ,NULL as [export_batch_id]
			  ,[branch_id] as [ordering_branch_id]
			  ,[branch_id] as [delivery_branch_id]				  
		FROM [dbo].[cards] 
			INNER JOIN temp_migrate_products as temp
				ON [cards].product_id = temp.product_id
					AND [cards].sub_product_id = ISNULL(temp.sub_product_id, -1)
)
GO

CREATE FUNCTION migration_customer_accounts(	)
RETURNS TABLE 
AS
RETURN 
(
	SELECT [customer_account_id]
      ,[user_id]
      ,[dbo].[customer_account].[card_id]
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
      ,[Id_number]
      ,[contact_number]
      ,[CustomerId]
      ,[domicile_branch_id]
  FROM [dbo].[customer_account] INNER JOIN [dbo].[cards]
	ON [dbo].[customer_account].[card_id] = [dbo].[cards].[card_id]
)
GO

CREATE FUNCTION migration_integration_cardnumbers( )
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [card_sequence_number])) as [card_sequence_number]
		,temp.[new_product_id] as [product_id], -1 as [sub_product_id] 
	FROM [dbo].[integration_cardnumbers]
		INNER JOIN temp_migrate_products as temp
				ON [integration_cardnumbers].product_id = temp.product_id
					AND [integration_cardnumbers].sub_product_id = ISNULL(temp.sub_product_id, -1)
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
		  ,[date_created]
		  ,[date_changed]		  
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
		  ,[date_created]
		  ,[date_changed]	  
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
		  ,temp.new_product_id as [product_id]		  
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
		INNER JOIN temp_migrate_products as temp
				ON [pin_reissue].product_id = temp.product_id
					AND CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [pan])) LIKE temp.bin_code + '%'
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

CREATE FUNCTION migrate_integration_fields()
RETURNS TABLE
AS
RETURN
(
	SELECT [integration_id]
			,[integration_object_id]
			,[integration_field_id]
			,[integration_field_name]
			,[accept_all_responses]
			,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [integration_field_default_value])) as [integration_field_default_value]			
	FROM [dbo].[integration_fields]
)
GO


--Items needed on target DB
USE [{DATABASE_NAME}]
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
      ,[identification]
      ,[timeout_milli]
      ,[buffer_size]
      ,[doc_type]
      ,[name_of_file]
      ,[file_delete_YN]
      ,[file_encryption_type_id]
      ,[duplicate_file_check_YN]      
      ,[domain_name]
      ,[is_external_auth]
	  ,[private_key]
      ,[public_key]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_connection_parameters]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

CREATE PROCEDURE [dbo].[migration_ldap]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	 SELECT 
		   --NULL as [connection_parameter_id]
		   [ldap_setting_name] as [connection_name]
		  ,[hostname_or_ip] as [address]
		  ,[port]
		  ,[path]
		  ,[protocol]
		  ,[auth_type]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([username], '')) as [username]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([password], '')) as [password]
		 ,4 as [connection_parameter_type_id] 			
			,NULL as [header_length]
			,NULL as [identification]
			,NULL as [timeout_milli]
			,NULL as [buffer_size]
			,CONVERT(CHAR(1), NULL) as [doc_type]
			,CONVERT(VARCHAR(100), NULL) as [name_of_file]
			,0 as [file_delete_YN]
			,0 as [file_encryption_type_id]
			,0 as [duplicate_file_check_YN]
			,NULL as [private_key]
			,NULL as [public_key]
			,[domain_name]
			,0 as [is_external_auth]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_ldap]()

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
	       ,CONVERT(datetime,[last_login_date]) as [last_login_date]
	       ,CONVERT(datetime,[last_login_attempt]) as [last_login_attempt]
	       ,CONVERT(int, [number_of_incorrect_logins]) as [number_of_incorrect_logins]
	       ,CONVERT(datetime,[last_password_changed_date]) as [last_password_changed_date]
		   ,CONVERT(nchar(50), [workstation]) as [workstation]
	       ,CONVERT(int, [language_id]) as [language_id]
	       ,CONVERT(varbinary(20),HashBytes( N'SHA1', CONVERT(varbinary(8000), [username]) + @key )) as [username_index] --one		
	       ,CONVERT(varbinary(256), ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [instant_authorisation_pin])) as [instant_authorisation_pin]
		   ,CONVERT(datetime, [last_authorisation_pin_changed_date]) as [last_authorisation_pin_changed_date],
			NuLL as authentication_configuration_id
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

CREATE PROCEDURE [dbo].[migrate_products]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate
		SELECT
			[product_id]
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
           ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),[PVKI]) as [PVKI]
           ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),[PVK]) as [PVK]
           ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),[CVKA]) as [CVKA]
           ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),[CVKB]) as [CVKB]
           ,[expiry_months]
           ,[fee_scheme_id]
           ,[enable_instant_pin_YN]
           ,[min_pin_length]
           ,[max_pin_length]
           ,0 as [enable_instant_pin_reissue_YN]
           ,0 as [cms_exportable_YN]
           ,0 as [product_load_type_id]
           ,[sub_product_code]
           ,0 as [pin_calc_method_id]
           ,0 as [auto_approve_batch_YN]
           ,[account_validation_YN]
           ,16 as [pan_length]
           ,[pin_mailer_printing_YN]
           ,[pin_mailer_reprint_YN]
           ,[sub_product_id]
           ,[master_product_id]
           ,[card_issue_method_id]
           ,null as [decimalisation_table]
           ,null as [pin_validation_data]
           ,0 as [pin_block_formatid]
           ,3 as [production_dist_batch_status_flow]
           ,6 as [distribution_dist_batch_status_flow]
           ,1 as [charge_fee_to_issuing_branch_YN]
           ,1 as [print_issue_card_YN]
           ,0 as [allow_manual_uploaded_YN]
           ,0 as [allow_reupload_YN]
           ,0 as [remote_cms_update_YN]
		FROM [{SOURCE_DATABASE_NAME}].[dbo].[migrate_products]()

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
          ,[card_production_date]
          ,[card_expiry_date]
          ,[card_activation_date]
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
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [Id_number]) as [Id_number]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [contact_number]) as [contact_number]
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [CustomerId]) as [CustomerId]
		  ,[domicile_branch_id]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_customer_accounts]()

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

	SELECT [issuer_name] + '_DummyRecord' as [fee_accounting_name]
           ,[issuer_id]
           ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), '') as [fee_revenue_account_no]
           ,0  as [fee_revenue_account_type_id]
           ,'' as [fee_revenue_branch_code]
           ,'' as [fee_revenue_narration_en]
           ,'' as [fee_revenue_narration_fr]
           ,'' as [fee_revenue_narration_pt]
           ,'' as [fee_revenue_narration_es]
           ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), '') as [vat_account_no]
           ,0  as [vat_account_type_id]
           ,'' as [vat_account_branch_code]
           ,'' as [vat_narration_en]
           ,'' as [vat_narration_fr]
           ,'' as [vat_narration_pt]
           ,'' as [vat_narration_es]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[issuer]

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

CREATE PROCEDURE [dbo].[migrate_integration_fields]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	 SELECT 
		   [integration_id]
		  ,[integration_object_id]
		  ,[integration_field_id]
		  ,[integration_field_name]
		  ,[accept_all_responses]		  
		  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), [integration_field_default_value]) as [integration_field_default_value]  
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migrate_integration_fields]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

