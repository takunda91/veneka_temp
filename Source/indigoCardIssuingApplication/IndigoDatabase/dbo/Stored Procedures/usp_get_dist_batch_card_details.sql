CREATE PROCEDURE [dbo].[usp_get_dist_batch_card_details] 
	@dist_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_DIST_BATCH_CARD_DETAILS]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;
			Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
			SELECT
				  CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)) AS 'card_number'
				, cards.card_request_reference AS card_reference_number
				, [cards].branch_id
				, [cards].card_id
				, [cards].card_issue_method_id
				, [cards].card_priority_id
				, [cards].card_request_reference
				, [cards].card_sequence
				, [cards].product_id
				, CONVERT(DATETIME, SWITCHOFFSET(CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_activation_date)),@UserTimezone) ) as 'card_activation_date'
				, CONVERT(DATETIME2,SWITCHOFFSET(CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_expiry_date)),@UserTimezone) ) as 'card_expiry_date'
				, CONVERT(DATETIME, SWITCHOFFSET(CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_production_date)),@UserTimezone) ) as 'card_production_date'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].pvv)) as 'pvv'
				, [dist_batch].dist_batch_reference
				, [dist_batch_cards].dist_batch_id
				, [dist_batch_cards].dist_card_status_id
				, [dist_card_statuses].dist_card_status_name
				, [customer_account].account_type_id
				, [customer_account].cms_id
				, [customer_account].contract_number
				, [customer_account].currency_id
				, [customer_account].customer_account_id
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
				,CAST(SWITCHOFFSET([customer_account].date_issued,@UserTimezone) as DATETIME) as 'date_issued'
				, [customer_account].resident_id
				, [customer_account].[user_id]
				, [issuer].issuer_id
				, [issuer].issuer_code
				, [issuer].issuer_name
				, [branch].branch_code
				, [branch].branch_name
				, [issuer_product].[product_code]
				, [issuer_product].[product_name]
				, [issuer_product].[product_bin_code]
				, [issuer_product].[sub_product_code]
				, [issuer_product].[pan_length]
				, [issuer_product].[src1_id]
				, [issuer_product].[src2_id]
				, [issuer_product].[src3_id]
				--, [issuer_product].sub_product_id_length
				, CONVERT(INT, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVKI]))) as 'PVKI'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVK])) as 'PVK'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKA])) as 'CVKA'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKB])) as 'CVKB'
				, [issuer_product].[expiry_months]
				,delivery_branch.[branch_code] as delivery_branch_code
				,delivery_branch.[branch_name] as delivery_branch_name
				--, [sub_product].sub_product_id
				--, [sub_product].sub_product_name
				--, [sub_product].sub_product_code
			FROM 
				[cards]							
				INNER JOIN [dist_batch_cards] 
					ON [dist_batch_cards].card_id = [cards].card_id	
				INNER JOIN [dist_card_statuses]
					ON 	[dist_batch_cards].dist_card_status_id = [dist_card_statuses].dist_card_status_id
				INNER JOIN [dist_batch]
					ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id	
				INNER JOIN [branch]
					ON [cards].branch_id = [branch].branch_id
				INNER JOIN [issuer]
					ON [branch].issuer_id = [issuer].issuer_id
				INNER JOIN [issuer_product]
					ON [cards].product_id = [issuer_product].product_id
				LEFT OUTER JOIN  [customer_account_cards]
						ON [customer_account_cards].card_id = [cards].card_id
					LEFT OUTER JOIN [customer_account] ON [customer_account].customer_account_id =[customer_account_cards].customer_account_id
				LEFT OUTER JOIN [branch] as delivery_branch
					ON [cards].[delivery_branch_id]= delivery_branch.branch_id 

				--LEFT OUTER JOIN [sub_product]
				--	ON [cards].sub_product_id = [sub_product].sub_product_id
				--		AND [cards].product_id = [sub_product].product_id
			WHERE 
				[dist_batch_cards].dist_batch_id = @dist_batch_id
					
														   
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting cards for distribution batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_DIST_BATCH_CARD_DETAILS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_DIST_BATCH_CARD_DETAILS]
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );
	END CATCH
END


GO
