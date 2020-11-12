USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_card]    Script Date: 2014/08/18 07:40:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get card info
-- =============================================
ALTER PROCEDURE [dbo].[sp_get_card] 
	@card_id BIGINT,
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

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT [cards].card_id, 
					   CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) AS 'card_number', 
					   [cards].product_id,					   
					   [issuer].issuer_name, [issuer].issuer_code, [issuer].issuer_id,
					   [issuer].instant_card_issue_YN, [issuer].pin_mailer_printing_YN,
					   [branch].branch_name, [branch].branch_code, [branch].branch_id,
					   [load_card_statuses].load_card_status,					   					   
					   [load_batch].load_batch_reference,	
					   
					   [issuer_product].product_name,
					   [issuer_product].product_code,
					   [issuer_product].Name_on_card_font_size,
					   [issuer_product].name_on_card_left,
					   [issuer_product].name_on_card_top,
					   [Issuer_product_font].font_name,	

					   [dist_card_statuses].dist_card_status_name,
					   [dist_batch].dist_batch_reference,
					   
					   [branch_card_status_current].branch_card_statuses_id,
					   [branch_card_status_current].status_date,
					   [branch_card_statuses_language].language_text AS 'branch_card_statuses_name',
					   [branch_card_status_current].branch_card_code_type_id,
					   [branch_card_status_current].branch_card_code_name,					   
					   [branch_card_status_current].spoil_only,
					   CONVERT(VARCHAR,DECRYPTBYKEY(operator.username)) as operator,

					   [customer_account].customer_account_id, 
					   CONVERT(VARCHAR,DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number',
					   [customer_account].card_issue_reason_id,
					   [card_issue_reason_language].language_text AS 'card_issuer_reason_name',
					   [customer_account].account_type_id,
					   [customer_account_type_language].language_text AS 'customer_account_type_name',
					   CONVERT(VARCHAR,DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name',
					   CONVERT(VARCHAR,DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name',
					   CONVERT(VARCHAR,DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name',
					   [customer_account].customer_title_id,
					   [customer_title_language].language_text AS 'customer_title_name',
					   [customer_account].date_issued,
					   CONVERT(VARCHAR,DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card',
					   [customer_account].[user_id],
					   CONVERT(VARCHAR,DECRYPTBYKEY([user].first_name)) as 'user_first_name',
					   CONVERT(VARCHAR,DECRYPTBYKEY([user].last_name)) as 'user_last_name',
					   CONVERT(VARCHAR,DECRYPTBYKEY([user].username)) as 'username',
					   [customer_account].cms_id,
					   [customer_account].currency_id,
					   [customer_account].resident_id,
					   [customer_residency_language].language_text AS 'customer_residency_name',
					   [customer_account].customer_type_id,
					   [customer_type_language].language_text AS 'customer_type_name',
					   [customer_account].contract_number
				FROM [cards]
					INNER JOIN [branch]
						ON [branch].branch_id = [cards].branch_id
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [branch].issuer_id					
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

					LEFT OUTER JOIN [dist_batch_cards]
						ON [cards].card_id = [dist_batch_cards].card_id
					LEFT OUTER  JOIN [dist_batch]
						ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id
					LEFT OUTER  JOIN [dist_card_statuses]
						ON [dist_card_statuses].dist_card_status_id = [dist_batch_cards].dist_card_status_id

					LEFT OUTER JOIN [branch_card_status_current]
						ON [branch_card_status_current].card_id = [cards].card_id
					--LEFT OUTER JOIN [branch_card_statuses]
					--	ON [branch_card_statuses].branch_card_statuses_id = [branch_card_status_current].branch_card_statuses_id
					LEFT OUTER JOIN [user] operator
						ON [branch_card_status_current].operator_user_id = operator.[user_id]
					LEFT OUTER JOIN [branch_card_statuses_language]
						ON [branch_card_statuses_language].branch_card_statuses_id = [branch_card_status_current].branch_card_statuses_id
							AND [branch_card_statuses_language].language_id = @language_id

					LEFT OUTER JOIN [customer_account]
						ON [cards].card_id = [customer_account].card_id
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
					LEFT OUTER  JOIN [user]
						ON [customer_account].[user_id] = [user].[user_id]
				WHERE [cards].card_id = @card_id
				

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			--log the audit record		
			--EXEC sp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting details for card.', 
			--					 NULL, NULL, NULL, NULL

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



