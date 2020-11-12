-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_customer_details] 
	@card_id bigint,
	@customer_account_id bigint,
	@branch_id int,
	@delivery_branch_id int,
	@product_id int,
	@card_priority_id int,
    @customer_account_number varchar(27),
	@domicile_branch_id int,
	@account_type_id int,
	@card_issue_reason_id int,
	@customer_first_name varchar(50),
	@customer_middle_name varchar(50),
	@customer_last_name varchar(50),
	@name_on_card varchar(30),
	@customer_title_id int,	
	@currency_id int,
	@resident_id int,
	@customer_type_id int,
	@cms_id varchar(50),
	@contract_number varchar(50),
	@customer_idnumber varchar(50),
	@contact_number varchar(50),
	@audit_user_id bigint,
	@audit_workstation varchar(100),	
	@ResultCode int OUTPUT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_CUST_TRAN]
		BEGIN TRY 

			IF @customer_middle_name IS NULL
				SET @customer_middle_name = ''
			
			DECLARE @status_date DATETIMEOFFSET,
					@branch_card_statuses_id int

			SET @branch_card_statuses_id = 2

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			--The initial card status.
			INSERT branch_card_status
					(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id,branch_id)
			VALUES (@card_id, @branch_card_statuses_id, SYSDATETIMEOFFSET(), @audit_user_id, @audit_user_id,@branch_id)
			
			UPDATE [cards]
			SET branch_id = @branch_id,
				ordering_branch_id = @branch_id,
				origin_branch_id = @branch_id,
				delivery_branch_id = @delivery_branch_id,
				product_id = @product_id,
				card_priority_id = @card_priority_id
			WHERE card_id = @card_id

			--Save customer details
			UPDATE customer_account 
			SET card_issue_reason_id = @card_issue_reason_id, 
				account_type_id = @account_type_id, 
				customer_account_number= ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_account_number)),
				customer_first_name = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_first_name)), 
				customer_middle_name = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_middle_name)), 
				customer_last_name = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_last_name)), 
				name_on_card = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@name_on_card)), 
				customer_title_id = @customer_title_id, 
				customer_type_id = @customer_type_id, 
				currency_id = @currency_id, 
				resident_id = @resident_id, 
				cms_id = @cms_id, 
				contract_number = @contract_number, 
				Id_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_idnumber)),
				contact_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@contact_number)),
				domicile_branch_id = @domicile_branch_id
			WHERE customer_account_id = @customer_account_id

			--Log audit stuff
			DECLARE @branchcardstatus  varchar(max),
					@Scenario  varchar(max),
					@audit_msg varchar(max),
					@cardnumber varchar(16)

			SELECT  @branchcardstatus =  branch_card_statuses.branch_card_statuses_name
			FROM    branch_card_statuses 
			WHERE	branch_card_statuses.branch_card_statuses_id = @branch_card_statuses_id

			SELECT  @Scenario =  card_issue_reason.[card_issuer_reason_name]
			FROM	card_issue_reason 
			WHERE	card_issue_reason.[card_issue_reason_id] = @card_issue_reason_id

			SET @audit_msg =  'card request-' + 
								COALESCE(@branchcardstatus, 'UNKNWON') +  
								', cust id:' + COALESCE(CAST(@cms_id as varchar(max)), 'n/a') +
								', a/c:' + dbo.MaskString(@customer_account_number, 3, 4) + 
								', ' + COALESCE(@Scenario, 'UNKNWON')

			--log the audit record		
			EXEC usp_insert_audit @audit_user_id, 
									3,---IssueCard
									NULL, 
									@audit_workstation, 
									@audit_msg, 
									NULL, NULL, NULL, NULL

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key			
				
			COMMIT TRANSACTION [UPDATE_CUST_TRAN]
			SET @ResultCode = 0

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_CUST_TRAN]
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

