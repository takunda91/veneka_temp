-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: 
-- Description:	This proc is used to add details to the DB to request a card for a customer
-- =============================================
CREATE PROCEDURE sp_request_card_for_customer 
	@branch_id int,
	@product_id int,
	@card_priority_id int,
    @customer_account_number varchar(27),
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
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@card_id bigint OUTPUT,
	@ResultCode int OUTPUT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [REQUEST_CARD_FOR_CUST_TRAN]
		BEGIN TRY 

			IF @customer_middle_name IS NULL
				SET @customer_middle_name = ''
			
			DECLARE @status_date datetime


			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			DECLARE @objid int
			SET @objid = object_id('cards')
			SET @status_date = GETDATE()

			--Inserting a card record with an empty card number, the card number will be generated later in the process. 
			-- when that happens this record should be populated with a card number.
			INSERT INTO [cards]	([product_id],[branch_id],[card_number],[card_sequence],[card_index], 
									card_issue_method_id, card_priority_id) 
				VALUES(@product_id, @branch_id, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR, '')), 0,
					   [dbo].[MAC]('0', @objid), 0, @card_priority_id)

			SET @card_id = SCOPE_IDENTITY();

			--Update card with reference number
			--Generate card reference
			DECLARE @card_ref varchar(100)
			SET @card_ref =  'CCR' + CAST(@card_id AS varchar(max))
								   + CONVERT(VARCHAR(8), GETDATE(), 112)

			UPDATE [cards]
				SET card_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR, @card_ref)),
					card_request_reference = @card_ref
			WHERE [card_id] = @card_id

			--Update the cards status.
			INSERT branch_card_status
					(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id)
			VALUES (@card_id, 2, @status_date, @audit_user_id, @audit_user_id) 

			--Save customer details
			INSERT customer_account
					([user_id], card_id, card_issue_reason_id, account_type_id, customer_account_number,
						customer_first_name, customer_middle_name, customer_last_name, name_on_card, customer_title_id, 
						date_issued, customer_type_id, currency_id, resident_id, cms_id, contract_number)
			VALUES (@audit_user_id, @card_id, @card_issue_reason_id, @account_type_id, 
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_account_number)),
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_first_name)),
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_middle_name)), 
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_last_name)), 
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@name_on_card)), 
					@customer_title_id, @status_date, @customer_type_id, @currency_id, @resident_id, @cms_id, @contract_number)					

					
			--Log audit stuff
			DECLARE @branchcardstatus  varchar(max),
					@Scenario  varchar(max),
					@audit_msg varchar(max),
					@cardnumber varchar(16)

			SELECT  @branchcardstatus =  branch_card_statuses.branch_card_statuses_name
			FROM    branch_card_statuses 
			WHERE	branch_card_statuses.branch_card_statuses_id = 2

			SELECT  @Scenario =  card_issue_reason.[card_issuer_reason_name]
			FROM	card_issue_reason 
			WHERE	card_issue_reason.[card_issue_reason_id] = @card_issue_reason_id

			SET @audit_msg =  'card request-' + 
								COALESCE(@branchcardstatus, 'UNKNWON') +  
								', cust id:' + COALESCE(CAST(@cms_id as varchar(max)), 'n/a') +
								', a/c:' + dbo.MaskString(@customer_account_number, 3, 4) + 
								', ' + COALESCE(@Scenario, 'UNKNWON')

			--log the audit record		
			EXEC sp_insert_audit @audit_user_id, 
									3,---IssueCard
									NULL, 
									@audit_workstation, 
									@audit_msg, 
									NULL, NULL, NULL, NULL

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key			
				
			COMMIT TRANSACTION [REQUEST_CARD_FOR_CUST_TRAN]
			SET @ResultCode = 0

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [REQUEST_CARD_FOR_CUST_TRAN]
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
