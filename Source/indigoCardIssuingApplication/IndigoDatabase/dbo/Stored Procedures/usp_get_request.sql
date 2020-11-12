CREATE PROCEDURE [dbo].[usp_get_request] 
	@request_id BIGINT,
	@check_masking BIT,
	@language_id INT,
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [GET_CARD_DETAILS_TRAN]
		BEGIN TRY 

			DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)			
			Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			--@check_masking = This proc is used by some backend process that require a clear mask and therefore 
			-- overrides the @mask_screen checking.

				SELECT distinct 
						hybrid_requests.[request_id]
					   , CASE @check_masking
							WHEN 1 THEN 
								CASE 
									WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
									ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
								END
							ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
						 END AS 'card_number'
					 					
					   , hybrid_requests.request_reference
					   , hybrid_requests.card_priority_id
					   , [card_priority_language].language_text AS 'card_priority_name'
					   , hybrid_requests.card_issue_method_id
					   , [card_issue_method_language].language_text AS 'card_issue_method_name'
					   , fee.fee_charged
					   , fee.vat
					   , fee.vat_charged
					   , fee.total_charged
					   , fee.fee_waiver_YN
					   , fee.fee_editable_YN
					   , fee.fee_overridden_YN
					   , fee.fee_reference_number
					   , fee.fee_reversal_ref_number
					   , CAST(0 as bit) AS pin_selected

					   , [hybrid_requests].product_id
					   , [issuer].issuer_name, [issuer].issuer_code, [issuer].issuer_id
					   , [issuer].instant_card_issue_YN 
					   , [issuer].maker_checker_YN, [issuer].enable_instant_pin_YN as enable_instant_pin_YN_issuer
					   , [issuer].back_office_pin_auth_YN, [issuer].authorise_pin_issue_YN
					   , [issuer].card_ref_preference
					   , [issuer].language_id
					   , [country].country_id
					   , [country].country_code
					   , [country].country_name
					   , [country].country_capital_city
					   , [branch].branch_name, [branch].branch_code, [branch].branch_id

					   , [issuer_product].product_name
					   , [issuer_product].product_code
					   , [issuer_product].product_bin_code
					   , [issuer_product].sub_product_code
					   , [issuer_product].Name_on_card_font_size
					   , [issuer_product].name_on_card_left
					   , [issuer_product].name_on_card_top
					   , [Issuer_product_font].font_name
					   , [issuer_product].enable_instant_pin_YN
					   , [issuer_product].pin_mailer_printing_YN
					   , [issuer_product].pin_mailer_reprint_YN
					   , [issuer_product].charge_fee_to_issuing_branch_YN
					   , [issuer_product].print_issue_card_YN	
					   , [issuer_product].allow_manual_uploaded_YN
					   , [issuer_product].allow_reupload_YN	

					   , [hybrid_request_status_current].hybrid_request_statuses_id
					   ,CAST(SWITCHOFFSET( [hybrid_request_status_current].status_date,@UserTimezone) as DateTime) as 'status_date' 
					   , [hybrid_request_statuses_language].language_text AS 'branch_card_statuses_name'
					  
					
					   , [customer_account].domicile_branch_id
					   , [domicile_branch].branch_code as domicile_branch_code
					   , [domicile_branch].branch_name as domicile_branch_name
					   , [delivery_branch].branch_code as delivery_branch_code
					   , [delivery_branch].branch_name as delivery_branch_name
					   , [customer_account].customer_account_id
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number'
					   , [customer_account].card_issue_reason_id
					
					   , [customer_account].account_type_id
					   , [customer_account_type_language].language_text AS 'customer_account_type_name'
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name'
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name'
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name'
					   , [customer_account].customer_title_id
					   , [customer_title_language].language_text AS 'customer_title_name'
					   , --[customer_account].date_issued
							CASE
								WHEN [hybrid_request_status_current].hybrid_request_statuses_id = 0 THEN CAST(SWITCHOFFSET([hybrid_request_status_current].status_date,@UserTimezone) as DateTime) 
								ELSE null
							END as date_issued
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card'
					   , [customer_account].[user_id]
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].first_name)) as 'user_first_name'
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].last_name)) as 'user_last_name'
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].username)) as 'username'
					   , [customer_account].cms_id
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].CustomerId)) as 'CustomerId'
					   , [customer_account].currency_id

					   , [currency].currency_code
					   , [currency].iso_4217_numeric_code
					   , [product_currency].is_base as is_base_currency
					   , [product_currency].usr_field_name_1
					   , [product_currency].usr_field_val_1
					   , [product_currency].usr_field_name_2
					   , [product_currency].usr_field_val_2

					   , [customer_account].resident_id
					   , [customer_residency_language].language_text AS 'customer_residency_name'
					   , [customer_account].customer_type_id
					   , [customer_type_language].language_text AS 'customer_type_name'
					   , [customer_account].contract_number
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].Id_number)) as 'id_number'
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number'

					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([product_fee_accounting].[fee_revenue_account_no])) as [fee_revenue_account_no]
					   , [product_fee_accounting].[fee_revenue_account_type_id]
					   , [product_fee_accounting].[fee_revenue_branch_code]
					   , [product_fee_accounting].[fee_revenue_narration_en]
				       , [product_fee_accounting].[fee_revenue_narration_fr]
					   , [product_fee_accounting].[fee_revenue_narration_pt]
					   , [product_fee_accounting].[fee_revenue_narration_es]
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([product_fee_accounting].[vat_account_no])) as [vat_account_no]
					   , [product_fee_accounting].[vat_account_type_id]
					   , [product_fee_accounting].[vat_account_branch_code]
					   , [product_fee_accounting].[vat_narration_en]
					   , [product_fee_accounting].[vat_narration_fr]
					   , [product_fee_accounting].[vat_narration_pt]
					   , [product_fee_accounting].[vat_narration_es]

				FROM [hybrid_requests]
					INNER JOIN [branch]
						ON [branch].branch_id = [hybrid_requests].branch_id
						LEFT OUTER JOIN [fee_charged] as fee
						ON fee.fee_id=[hybrid_requests].fee_id 
					INNER JOIN [branch] as [delivery_branch]
						ON [delivery_branch].branch_id = [hybrid_requests].delivery_branch_id
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [branch].issuer_id
					INNER JOIN [country]
						ON [country].country_id = [issuer].country_id						
					INNER JOIN [card_issue_method_language]
						ON [hybrid_requests].card_issue_method_id = [card_issue_method_language].card_issue_method_id
							AND [card_issue_method_language].language_id = @language_id
					INNER JOIN [card_priority_language]
						ON [hybrid_requests].card_priority_id = [card_priority_language].card_priority_id
							AND [card_priority_language].language_id = @language_id
					INNER JOIN [issuer_product]
						ON [issuer_product].product_id = [hybrid_requests].product_id
					INNER JOIN [Issuer_product_font]
						ON [issuer_product].font_id = [Issuer_product_font].font_id
					LEFT OUTER JOIN [product_fee_scheme]
						ON [issuer_product].fee_scheme_id = [product_fee_scheme].fee_scheme_id
					LEFT OUTER JOIN [product_fee_accounting]
						ON [product_fee_scheme].fee_accounting_id = [product_fee_accounting].fee_accounting_id

					LEFT OUTER JOIN [hybrid_request_status_current]
						ON [hybrid_request_status_current].request_id = [hybrid_requests].request_id
					LEFT OUTER JOIN [user] operator
						ON [hybrid_request_status_current].operator_user_id = operator.[user_id]
					LEFT OUTER JOIN [hybrid_request_statuses_language]
						ON [hybrid_request_statuses_language].hybrid_request_statuses_id = [hybrid_request_status_current].hybrid_request_statuses_id
							AND [hybrid_request_statuses_language].language_id = @language_id

						LEFT OUTER JOIN [customer_account_requests]
						ON [customer_account_requests].request_id = [hybrid_requests].request_id						
					LEFT OUTER JOIN [customer_account]
						ON [customer_account_requests].customer_account_id = [customer_account].customer_account_id

						LEFT OUTER JOIN [customer_account_cards]
						ON [customer_account_cards].customer_account_id = [customer_account].customer_account_id
						LEFT OUTER JOIN [cards]
						ON [cards].card_id =[customer_account_cards].card_id
						LEFT OUTER JOIN [branch] as [domicile_branch]
						ON [domicile_branch].branch_id = [customer_account].domicile_branch_id
						LEFT OUTER JOIN [customer_type_language]
						ON [customer_type_language].customer_type_id = [customer_account].customer_type_id
							AND [customer_type_language].language_id = @language_id
						LEFT OUTER JOIN [customer_account_type_language]
						ON [customer_account_type_language].account_type_id = [customer_account].account_type_id
							AND [customer_account_type_language].language_id = @language_id
					LEFT OUTER JOIN [customer_residency_language]
						ON [customer_residency_language].resident_id = [customer_account].resident_id
							AND [customer_residency_language].language_id = @language_id
					LEFT OUTER JOIN [customer_title_language]
						ON [customer_title_language].customer_title_id = [customer_account].customer_title_id
							AND [customer_title_language].language_id = @language_id
					LEFT OUTER JOIN [card_issue_reason_language]
						ON [card_issue_reason_language].card_issue_reason_id = [customer_account].card_issue_reason_id
							AND [card_issue_reason_language].language_id = @language_id
					LEFT OUTER JOIN [currency]
						ON [currency].currency_id = [customer_account].currency_id
					LEFT OUTER JOIN [product_currency]
						ON [product_currency].currency_id = [customer_account].currency_id
							AND [product_currency].product_id = [issuer_product].product_id
					LEFT OUTER JOIN [user]
						ON [customer_account].[user_id] = [user].[user_id]
				WHERE [hybrid_requests].request_id = @request_id
				

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key


			COMMIT TRANSACTION [GET_CARD_DETAILS_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_CARD_DETAILS_TRAN]
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

