CREATE PROCEDURE [dbo].[usp_get_pin_batch_card_details] 
	-- Add the parameters for the stored procedure here
	@pin_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)
	Declare @UserTimezone  nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT 
			 
				CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)) as 'card_number', 
				[cards].branch_id, [cards].card_id, [cards].card_issue_method_id,
				[cards].card_priority_id, [cards].card_request_reference, [cards].card_sequence,
				[cards].product_id,
				CONVERT(DATETIME,SWITCHOFFSET(CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_activation_date)) ,@UserTimezone)) as 'card_activation_date', 
				CONVERT(DATETIME,SWITCHOFFSET(CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_expiry_date)) ,@UserTimezone)) as 'card_expiry_date', 
				CONVERT(DATETIME,SWITCHOFFSET(CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_production_date)) ,@UserTimezone)) as 'card_production_date',						
				CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].pvv)) as 'pvv',
				[pin_batch_cards].pin_batch_id,
				[pin_batch_cards].pin_batch_cards_statuses_id,
				[pin_batch_card_statuses].pin_batch_card_statuses_name,
				[customer_account].account_type_id, [customer_account].cms_id, 
				[customer_account].contract_number, [customer_account].currency_id, [customer_account].customer_account_id,
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number', 
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].Id_number)) as 'Id_number',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].CustomerId)) as 'CustomerId',
				[customer_account].customer_title_id, [customer_account].customer_type_id,
								CONVERT(DATETIME,SWITCHOFFSET(CONVERT(VARCHAR(max),[customer_account].date_issued) ,@UserTimezone)) as date_issued ,   [customer_account].resident_id,
				[customer_account].[user_id],
				[issuer].issuer_id, [issuer].issuer_code, [issuer].issuer_name,
				[branch].branch_code, [branch].branch_name,
				[issuer_product].[product_code],
				[issuer_product].sub_product_code,
				[issuer_product].[product_name],
				[issuer_product].[product_bin_code],
				[issuer_product].[src1_id],
				[issuer_product].[src2_id],
				[issuer_product].[src3_id],
				--[issuer_product].sub_product_id_length,
				CONVERT(INT, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVKI]))) as 'PVKI',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVK])) as 'PVK',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKA])) as 'CVKA',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKB])) as 'CVKB',
				[issuer_product].[expiry_months]
				--[sub_product].sub_product_id,
				--[sub_product].sub_product_name,
				--[sub_product].sub_product_code
		FROM [cards]
			INNER JOIN [pin_batch_cards] 
				ON [pin_batch_cards].card_id = [cards].card_id	
			INNER JOIN [pin_batch_card_statuses]
				ON 	[pin_batch_cards].pin_batch_cards_statuses_id = [pin_batch_card_statuses].pin_batch_card_statuses_id
			INNER JOIN [branch]
				ON [cards].branch_id = [branch].branch_id
			INNER JOIN [issuer]
				ON [branch].issuer_id = [issuer].issuer_id
			INNER JOIN [issuer_product]
				ON [cards].product_id = [issuer_product].product_id
			LEFT OUTER JOIN  [customer_account_cards]
						ON [customer_account_cards].card_id = [cards].card_id
					INNER JOIN [customer_account] ON [customer_account].customer_account_id =[customer_account_cards].customer_account_id
			--LEFT OUTER JOIN [sub_product]
			--	ON [cards].sub_product_id = [sub_product].sub_product_id
			--		AND [cards].product_id = [sub_product].product_id
		WHERE [pin_batch_cards].pin_batch_id = @pin_batch_id					
														   
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	
END

