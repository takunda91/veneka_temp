CREATE PROCEDURE [dbo].[usp_remote_get_pending]
	@issuer_id int,
	@remote_component_ip varchar(250),
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statementsaudit_
	SET NOCOUNT ON;	

	DECLARE @pending_cards TABLE ([remote_update_statuses_id] INT, [card_id] BIGINT, 
									[status_date] DATETIMEOFFSET,  [comments] NVARCHAR(MAX), [remote_component] VARCHAR(250), [user_id] BIGINT, [remote_updated_time] DATETIMEOFFSET)

	--"Checkout the cards to the remote component"
	
	UPDATE [dbo].[remote_update_status]
		SET comments = '', 
		remote_component = @remote_component_ip, 
		remote_update_statuses_id = 1, 
		remote_updated_time = NULL, 
		[user_id] = @audit_user_id, 
		status_date = NULL
	OUTPUT Deleted.* INTO @pending_cards	
	WHERE [remote_update_statuses_id] IN (0, 3)

	INSERT INTO [remote_update_status_audit] ([remote_update_statuses_id], [card_id], [status_date],  [comments], [remote_component], [user_id], [remote_updated_time])
	SELECT [remote_update_statuses_id], [card_id], [status_date],  [comments], [remote_component], [user_id], [remote_updated_time]
	FROM @pending_cards
	

	--INSERT INTO [dbo].[remote_update_status] (card_id, comments, remote_component, remote_update_statuses_id, status_date, [user_id], [remote_updated_time])
	--OUTPUT inserted.* INTO @pending_cards
	--SELECT card_id, '', @remote_component_ip, 1, SYSDATETIMEOFFSET(), @audit_user_id, null
	--FROM [dbo].[remote_update_current]
	--WHERE [dbo].[remote_update_current].[remote_update_statuses_id] IN (0, 3)

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	--Return the card data for these checked out cards
	SELECT 
		CONVERT(VARCHAR(MAX), DECRYPTBYKEY([cards].card_number)) AS 'card_number'
		, cards.card_request_reference AS card_reference_number
		, [cards].branch_id
		, [cards].card_id
		, [cards].card_request_reference
		, [cards].card_sequence
		, [cards].product_id
		, [cards].card_priority_id
		, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_activation_date)), 109) as 'card_activation_date'
		, CONVERT(DATETIME2,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_expiry_date))) as 'card_expiry_date'
		, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_production_date)), 109) as 'card_production_date'						
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].pvv)) as 'pvv'
		, [customer_account].account_type_id
		, [customer_account].cms_id
		, [customer_account].contract_number
		, [customer_account].currency_id
		, [customer_account].card_issue_reason_id
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].Id_number)) as 'Id_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].CustomerId)) as 'CustomerId'
		, [customer_account].customer_title_id
		, [customer_account].customer_type_id
		, [customer_account].date_issued
		, [customer_account].resident_id
		, [issuer].issuer_id
		, [issuer].issuer_code
		, [issuer].issuer_name
		, [branch].branch_code
		, [branch].branch_name
		, [issuer_product].product_id
		, [issuer_product].[product_code]
		, [issuer_product].[sub_product_code]
		, [issuer_product].[product_name]
		, [issuer_product].[product_bin_code]
		, [issuer_product].[src1_id]
		, [issuer_product].[src2_id]
		, [issuer_product].[src3_id]
		, [issuer_product].[pan_length]
		, CONVERT(INT, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVKI]))) as 'PVKI'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVK])) as 'PVK'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKA])) as 'CVKA'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKB])) as 'CVKB'
		, [issuer_product].[expiry_months]
		, [currency].currency_code
		, [currency].iso_4217_numeric_code
		, [country].country_name
		, [country].country_code
	FROM 
		[cards]							
		INNER JOIN [branch]
			ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer]
			ON [branch].issuer_id = [issuer].issuer_id
		INNER JOIN [issuer_product]
			ON [cards].product_id = [issuer_product].product_id
		INNER JOIN [customer_account_cards]
						ON [customer_account_cards].card_id = [cards].card_id
		INNER JOIN [customer_account] ON [customer_account].customer_account_id =[customer_account_cards].customer_account_id
		INNER JOIN [currency]
			ON [currency].currency_id = [customer_account].currency_id
		INNER JOIN [country]
			ON [country].country_id = [issuer].country_id
	WHERE [dbo].[cards].[card_id] IN (SELECT card_id FROM @pending_cards)

	--return the connection information for the cards per product
	SELECT DISTINCT [dbo].[cards].product_id,
			[address],[port],[path],[protocol],[auth_type],[header_length],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([identification])) as identification,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password], 
			connection_parameter_type_id, timeout_milli, buffer_size, doc_type, name_of_file, 
			file_delete_YN, file_encryption_type_id, duplicate_file_check_YN, remote_port,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([private_key])) as [private_key], 
			CONVERT(VARCHAR(max),DECRYPTBYKEY([public_key])) as [public_key],domain_name ,is_external_auth, remote_port,
			CONVERT(VARCHAR(max),DECRYPTBYKEY(remote_username)) as [remote_username], 
			CONVERT(VARCHAR(max),DECRYPTBYKEY(remote_password)) as [remote_password],
			[dbo].[product_interface].interface_guid,
			[dbo].[product_interface].interface_type_id
	FROM [dbo].[connection_parameters]
			INNER JOIN [dbo].[product_interface]
				ON [dbo].[connection_parameters].connection_parameter_id = [dbo].[product_interface].connection_parameter_id
					AND [dbo].[product_interface].interface_type_id IN (1, 9) --Get the CMS and the Remote CMS
					AND [dbo].[product_interface].interface_area = 1
			INNER JOIN [dbo].[cards]	
				ON [dbo].[product_interface].product_id = [dbo].[cards].product_id
	WHERE [cards].card_id IN (SELECT card_id FROM @pending_cards)
		
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

	SELECT DISTINCT PES.external_system_field_id,PES.product_id,PES.field_value, [external_system_fields].external_system_id,
			[external_system_fields].field_name,[external_systems].external_system_type_id
	FROM [product_external_system] PES INNER JOIN [external_system_fields]
			ON PES.external_system_field_id = [external_system_fields].external_system_field_id
		INNER JOIN [external_systems]
			ON [external_system_fields].external_system_id = [external_systems].external_system_id
		INNER JOIN [dbo].[cards]
			ON PES.product_id = [dbo].[cards].product_id 
	WHERE [external_systems].external_system_type_id in (2, 4)
			AND [cards].card_id IN (SELECT card_id FROM @pending_cards)
