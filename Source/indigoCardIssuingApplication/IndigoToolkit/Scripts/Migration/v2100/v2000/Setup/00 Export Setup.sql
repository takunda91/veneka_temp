USE [{SOURCE_DATABASE_NAME}]
GO
--Items needed on source DB

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
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [dbo].[user].[username])) as [username]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [first_name])) as [first_name]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [last_name])) as [last_name]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [dbo].[user].[password])) as [password]
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
		   ,[dbo].[ldap_setting].[ldap_setting_name]
	FROM [dbo].[user] LEFT OUTER JOIN [dbo].[ldap_setting]
		ON [dbo].[user].[ldap_setting_id] = [dbo].[ldap_setting].[ldap_setting_id]
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
			  ,CONVERT(varbinary(24), 'HelloPeople') as [card_index]
			  ,1 as [card_issue_method_id]
			  ,1 as [card_priority_id]
			  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, card_number)) as [card_request_reference]
			  ,NULL as [card_production_date]
			  ,NULL as [card_expiry_date]
			  ,NULL as [card_activation_date]
			  ,NULL as [pvv]
			  ,NULL as [fee_charged]
			  ,NULL as [fee_waiver_YN]
			  ,NULL as [fee_editable_YN]
			  ,NULL as [fee_overridden_YN]
			  ,CONVERT(VARCHAR(100),NULL) as [fee_reference_number]
			  ,CONVERT(VARCHAR(100),NULL) as [fee_reversal_ref_number]
			  ,[branch_id] as [origin_branch_id]
			  ,NULL as [export_batch_id]
			  ,[branch_id] as [ordering_branch_id]
			  ,[branch_id] as [delivery_branch_id]			  
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
      ,CONVERT(VARCHAR(MAX), '') as [Id_number]
      ,CONVERT(VARCHAR(MAX), '') as [contact_number]
      ,[internal_account_nr] as [CustomerId]
      ,[dbo].[cards].[branch_id] as [domicile_branch_id]
  FROM [dbo].[customer_account] INNER JOIN [dbo].[cards]
	ON [dbo].[customer_account].[card_id] = [dbo].[cards].[card_id]
)
GO

CREATE FUNCTION migration_file_locations(	)
RETURNS TABLE 
AS
RETURN 
(
	WITH [locations] AS (
	SELECT DISTINCT [cards_file_location]
		FROM [dbo].[issuer] 
		WHERE [cards_file_location] IS NOT NULL AND [cards_file_location] != ''	
		)
	SELECT --NULL as [connection_parameter_id]
		   'cards_file_location_' + CONVERT(varchar, ROW_NUMBER() OVER (ORDER BY (SELECT 1))) AS [connection_name]
		  ,'' as [address]
		  ,0 as [port]
		  ,[cards_file_location] as [path]
		  ,2 as [protocol]
		  ,0 as [auth_type]
		  ,'' as [username]
		  ,'' as [password]
		  ,1 as [connection_parameter_type_id]
		  ,NULL as [header_length]
		  ,'' as [identification]
		  ,NULL as [timeout_milli]
		  ,NULL as [buffer_size]
		  ,CONVERT(CHAR(1), NULL) as [doc_type]
		  ,CONVERT(VARCHAR(100), NULL) as [name_of_file]
		  ,NULL as [file_delete_YN]
		  ,0 as [file_encryption_type_id]
		  ,NULL as [duplicate_file_check_YN]
		  ,CONVERT(VARCHAR(100), NULL) as [private_key]
		  ,CONVERT(VARCHAR(100), NULL) as [public_key]
		  ,CONVERT(VARCHAR(100), NULL) as [domain_name]
		  ,0 as [is_external_auth]
	FROM [locations]
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
      ,[private_key]
      ,[public_key]
      ,[domain_name]
      , [is_external_auth]
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
		  ,[private_key]
		  ,[public_key]
		  ,[domain_name]
		  ,[is_external_auth]
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
		   ,CONVERT(nchar(50), [ldap_setting_name]) as [workstation]
	       ,CONVERT(int, [language_id]) as [language_id]
	       ,CONVERT(varbinary(20),HashBytes( N'SHA1', CONVERT(varbinary(8000), [username]) + @key )) as [username_index] --one
		   
	       ,CONVERT(varbinary(256), [instant_authorisation_pin]) as [instant_authorisation_pin]
		   ,CONVERT(datetime, [last_authorisation_pin_changed_date]) as [last_authorisation_pin_changed_date]
		  
		   ,	NuLL as authentication_configuration_id
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_users]() 
	--as [mig_users] LEFT OUTER JOIN [dbo].[connection_parameters] 
	--		ON [mig_users].[ldap_setting_name] = [dbo].[connection_parameters].[connection_name]
	

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO

--CREATE PROCEDURE [dbo].[migration_users]	
--AS
--BEGIN
--	-- SET NOCOUNT ON added to prevent extra result sets from
--	-- interfering with SELECT statements.
--	SET NOCOUNT ON;

--	--SELECT * FROM [indigo_database_main_dev].[dbo].[user]

--	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
--	DECRYPTION BY CERTIFICATE Indigo_Certificate

--	 SELECT 	  
			 	  
--			CONVERT(bigint, 0) AS [user_id],
--	        CONVERT(int, 0) as [user_status_id]
--	       ,CONVERT(int, 0) as [user_gender_id]
--	       ,CONVERT(varbinary(256), ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), 'username')) as [username]
--	       ,CONVERT(varbinary(256), ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), 'first_name')) as [first_name]
--	       ,CONVERT(varbinary(256), ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), 'last_name')) as [last_name]
--	       ,CONVERT(varbinary(256), ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), 'password')) as [password]
--	       ,CONVERT(VARCHAR(100), 'email') as [user_email]
--	       ,CONVERT(bit, 0) as [online]
--	       ,CONVERT(varbinary(256), ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), 'employee_id')) as [employee_id]
--	       ,GETDATE() AS [last_login_date]
--	       ,GETDATE() AS [last_login_attempt]
--	       ,CONVERT(int, 0) as [number_of_incorrect_logins]
--	       ,GETDATE() as [last_password_changed_date]
--	       ,CONVERT(nchar(50), 'workstation') as [workstation]
--	       ,CONVERT(int, 0) as [language_id]
--	       ,CONVERT(varbinary(20),HashBytes( N'SHA1', '123')) as [username_index]
--		   ,CONVERT(int, 0) as [connection_parameter_id]
--	       ,CONVERT(varbinary(256), ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), 'bad')) as [instant_authorisation_pin]
--		   ,GETDATE() as [last_authorisation_pin_changed_date]
--		   ,CONVERT(char(36), '123456789012345678901234567890123456') as [external_interface_id]
	

--	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
--END
--GO

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

CREATE PROCEDURE [dbo].[migration_file_locations]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
	DECRYPTION BY CERTIFICATE Indigo_Certificate


	SELECT --[connection_parameter_id]
      [connection_name]
      ,[address]
      ,[port]
      ,[path]
      ,[protocol]
      ,[auth_type]
	  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([username], '')) as [username]
	  ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([password], '')) as [password]
      ,[connection_parameter_type_id]
      ,[header_length]
      ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([identification], '')) as [identification]
      ,[timeout_milli]
      ,[buffer_size]
      ,[doc_type]
      ,[name_of_file]
      ,[file_delete_YN]
      ,[file_encryption_type_id]
      ,[duplicate_file_check_YN]
      ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([private_key], '')) as [private_key]
      ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), ISNULL([public_key], '')) as [public_key]
      ,[domain_name]
      ,[is_external_auth]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[migration_file_locations]()

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END
GO
