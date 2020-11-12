-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get card info
-- =============================================
CREATE  PROCEDURE [dbo].[usp_get_card] 
	@card_id BIGINT,
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
						[cards].card_id
					   , CASE @check_masking
							WHEN 1 THEN 
								CASE 
									WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(100),DECRYPTBYKEY([cards].card_number)),6,4) 
									ELSE CONVERT(VARCHAR(100),DECRYPTBYKEY([cards].card_number))
								END
							ELSE CONVERT(VARCHAR(100),DECRYPTBYKEY([cards].card_number))
						 END AS 'card_number'
					   , CASE @check_masking
							WHEN 1 THEN CONVERT(VARCHAR(100),DECRYPTBYKEY([cards].pvv)) ELSE ''
							END as pvv							
					   , [cards].card_request_reference
					   , [cards].card_priority_id
					   , [card_priority_language].language_text AS 'card_priority_name'
					   , [cards].card_issue_method_id
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

					   , [cards].product_id
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
					   , [load_card_statuses].load_card_status			   					   
					   , [load_batch].load_batch_reference

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

					   , [dist_card_statuses].dist_card_status_name
					   , [dist_batch].dist_batch_reference 

					   , [branch_card_status_current].branch_card_statuses_id
					   ,CAST(SWITCHOFFSET( [branch_card_status_current].status_date,@UserTimezone) as DateTime) as 'status_date' 
					   , [branch_card_statuses_language].language_text AS 'branch_card_statuses_name'
					   , [branch_card_status_current].branch_card_code_type_id
					   , [branch_card_status_current].branch_card_code_name			   
					   , [branch_card_status_current].spoil_only
					   , [branch_card_status_current].comments
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY(operator.username)) as operator
					   , [customer_account].domicile_branch_id
					   , [domicile_branch].branch_code as domicile_branch_code
					   , [domicile_branch].branch_name as domicile_branch_name
					   , [delivery_branch].branch_code as delivery_branch_code
					   , [delivery_branch].branch_name as delivery_branch_name
					   , [customer_account].customer_account_id
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number'
					   , [customer_account].card_issue_reason_id
					   , [card_issue_reason_language].language_text AS 'card_issuer_reason_name'
					   , [customer_account].account_type_id
					   , [customer_account_type_language].language_text AS 'customer_account_type_name'
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name'
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name'
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name'
					   , [customer_account].customer_title_id
					   , [customer_title_language].language_text AS 'customer_title_name'
					   , --[customer_account].date_issued
							CASE
								WHEN [branch_card_status_current].branch_card_statuses_id = 6 THEN CAST(SWITCHOFFSET([branch_card_status_current].status_date,@UserTimezone) as DateTime) 
								ELSE null
							END as date_issued
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card'
					   , [customer_account].[user_id]
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY([user].first_name)) as 'user_first_name'
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY([user].last_name)) as 'user_last_name'
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY([user].username)) as 'username'
					   , [customer_account].cms_id
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY([customer_account].CustomerId)) as 'CustomerId'
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
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY([customer_account].Id_number)) as 'id_number'
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number'
					   , [pin_mailer_reprint_statuses_language].pin_mailer_reprint_status_id
					   , [pin_mailer_reprint_statuses_language].language_text as 'pin_mailer_reprint_status_name'

					   , CONVERT(VARCHAR(100),DECRYPTBYKEY([product_fee_accounting].[fee_revenue_account_no])) as [fee_revenue_account_no]
					   , [product_fee_accounting].[fee_revenue_account_type_id]
					   , [product_fee_accounting].[fee_revenue_branch_code]
					   , [product_fee_accounting].[fee_revenue_narration_en]
				       , [product_fee_accounting].[fee_revenue_narration_fr]
					   , [product_fee_accounting].[fee_revenue_narration_pt]
					   , [product_fee_accounting].[fee_revenue_narration_es]
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY([product_fee_accounting].[vat_account_no])) as [vat_account_no]
					   , [product_fee_accounting].[vat_account_type_id]
					   , [product_fee_accounting].[vat_account_branch_code]
					   , [product_fee_accounting].[vat_narration_en]
					   , [product_fee_accounting].[vat_narration_fr]
					   , [product_fee_accounting].[vat_narration_pt]
					   , [product_fee_accounting].[vat_narration_es]
					   , [product_account_types_mapping].cms_account_type
					   ,[product_account_types_mapping].cbs_account_type
				FROM [cards]
					INNER JOIN [branch]
						ON [branch].branch_id = [cards].branch_id
					INNER JOIN [branch] as [delivery_branch]
						ON [delivery_branch].branch_id = [cards].delivery_branch_id
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [branch].issuer_id
					INNER JOIN [country]
						ON [country].country_id = [issuer].country_id						
					INNER JOIN [card_issue_method_language]
						ON [cards].card_issue_method_id = [card_issue_method_language].card_issue_method_id
							AND [card_issue_method_language].language_id = @language_id
					INNER JOIN [card_priority_language]
						ON [cards].card_priority_id = [card_priority_language].card_priority_id
							AND [card_priority_language].language_id = @language_id					
					LEFT OUTER JOIN [load_batch_cards]
						ON [load_batch_cards].card_id = [cards].[card_id]
					LEFT OUTER JOIN [load_batch]
						ON [load_batch].load_batch_id = [load_batch_cards].load_batch_id
					LEFT OUTER JOIN [load_card_statuses]
						ON [load_card_statuses].load_card_status_id = [load_batch_cards].load_card_status_id
					INNER JOIN [issuer_product]
						ON [issuer_product].product_id = [cards].product_id
					INNER JOIN [Issuer_product_font]
						ON [issuer_product].font_id = [Issuer_product_font].font_id
					LEFT OUTER JOIN [product_fee_scheme]
						ON [issuer_product].fee_scheme_id = [product_fee_scheme].fee_scheme_id
					LEFT OUTER JOIN [product_fee_accounting]
						ON [product_fee_scheme].fee_accounting_id = [product_fee_accounting].fee_accounting_id
					LEFT OUTER JOIN 	[fee_charged] as fee
						ON fee.fee_id=cards.fee_id

					LEFT OUTER JOIN [dist_batch_cards]
						ON [cards].card_id = [dist_batch_cards].card_id
					LEFT OUTER  JOIN [dist_batch]
						ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id
					LEFT OUTER  JOIN [dist_card_statuses]
						ON [dist_card_statuses].dist_card_status_id = [dist_batch_cards].dist_card_status_id

					LEFT OUTER JOIN [branch_card_status_current]
						ON [branch_card_status_current].card_id = [cards].card_id
					LEFT OUTER JOIN [user] operator
						ON [branch_card_status_current].operator_user_id = operator.[user_id]
					LEFT OUTER JOIN [branch_card_statuses_language]
						ON [branch_card_statuses_language].branch_card_statuses_id = [branch_card_status_current].branch_card_statuses_id
							AND [branch_card_statuses_language].language_id = @language_id
						LEFT OUTER JOIN  [customer_account_cards]
						ON [cards].card_id = [customer_account_cards].card_id
					LEFT OUTER JOIN [customer_account] ON [customer_account].customer_account_id =[customer_account_cards].customer_account_id
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
					
					LEFT OUTER JOIN [product_account_types_mapping]
						ON [product_account_types_mapping].product_id=cards.product_id 
						AND [product_account_types_mapping].cbs_account_type=customer_account.cbs_account_type

					LEFT OUTER JOIN [pin_mailer_reprint_status_current]
						ON [pin_mailer_reprint_status_current].card_id = [cards].card_id
					LEFT OUTER JOIN [pin_mailer_reprint_statuses_language]
						ON [pin_mailer_reprint_statuses_language].pin_mailer_reprint_status_id = [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id
							AND [pin_mailer_reprint_statuses_language].language_id = @language_id
				WHERE [cards].card_id = @card_id
				

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









