CREATE PROCEDURE [dbo].[usp_request_instantcard_for_customer] 
	@delivery_branch_id int,
	@branch_id int,
	@product_id int,		
	@card_priority_id int=null,
    @customer_account_number varchar(27),
	@domicile_branch_id int,
	@account_type_id int,
	@card_issue_reason_id int=null,
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
	@idnumber varchar(50),
	@contact_number varchar(50),
	@customer_id varchar(50),
	@fee_waiver_YN bit = NULL,
	@fee_editable_YN bit = NULL,
	@fee_charged decimal(10,4) = NULL,
	@fee_overridden_YN bit = NULL,
	@card_issue_method_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@card_id bigint OUTPUT,
	@ResultCode int OUTPUT,
	@print_job_id bigint output,
	@new_customer_account_id int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [REQUEST_CARD_FOR_CUST_TRAN]
		BEGIN TRY 

			IF @customer_middle_name IS NULL
				SET @customer_middle_name = ''
			
			DECLARE @status_date datetimeoffset,
					@branch_card_statuses_id int

			SET @branch_card_statuses_id = 2

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			DECLARE @objid int
			SET @objid = object_id('cards')
			SET @status_date = SYSDATETIMEOFFSET()

			DECLARE @branch_type_id int

			SELECT 	@branch_type_id = branch_type_id from branch where branch_id=@branch_id

			IF(@card_issue_method_id=1 )
			BEGIN
			--Inserting a card record with an empty card number, the card number will be generated later in the process. 
			-- when that happens this record should be populated with a card number.
			INSERT INTO [cards]	([product_id],[ordering_branch_id],[branch_id],[origin_branch_id],[delivery_branch_id],[card_number],[card_sequence],[card_index], 
									card_issue_method_id, card_priority_id) 
				VALUES(@product_id, @branch_id, @branch_id, @branch_id, @branch_id, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR, '')), 0,
					   [dbo].[MAC]('0', @objid), @card_issue_method_id, 0)

			SET @card_id = SCOPE_IDENTITY();
			DECLARE  @new_fee_id as int
			
					INSERT INTO fee_charged(fee_waiver_YN,fee_editable_YN,fee_charged,fee_overridden_YN,operator_user_id)
									VALUES(@fee_waiver_YN,@fee_editable_YN,@fee_charged,@fee_overridden_YN,@audit_user_id)
					SET @new_fee_id = SCOPE_IDENTITY()	
					UPDATE [cards]
					SET fee_id=@new_fee_id
					WHERE card_id = @card_id
			--Update card with reference number
			--Generate card reference
			DECLARE @card_ref varchar(100)
			SET @card_ref =  'CCR' + CONVERT(VARCHAR(8), SYSDATETIMEOFFSET(), 112) + CAST(@product_id AS varchar(max)) + CAST(@card_id AS varchar(max))

			UPDATE [cards]
				SET card_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR(max), @card_ref)),
					card_request_reference = @card_ref
			WHERE [card_id] = @card_id


			--The initial card status.
			INSERT branch_card_status (card_id, branch_id, branch_card_statuses_id, status_date, [user_id], operator_user_id)
			VALUES (@card_id, @branch_id, @branch_card_statuses_id, @status_date, @audit_user_id, @audit_user_id)

		
			--Check if we need to do maker/checker for the request.
			--If no maker checker then we "Auto" approve the card for issue.
			UPDATE branch_card_status 
					SET branch_id = @branch_id, 
						branch_card_statuses_id = @branch_card_statuses_id, 
						status_date = DATEADD(ss, 1, @status_date), 
						[user_id] = @audit_user_id, 
						operator_user_id = @audit_user_id,
						comments = 'Auto Approve Card For Issue',
						branch_card_code_id = NULL,						
						pin_auth_user_id = NULL
					OUTPUT Deleted.* INTO branch_card_status_audit
					WHERE card_id = @card_id
						 

			--Save customer details
			INSERT customer_account
					([user_id], card_issue_reason_id, account_type_id, customer_account_number,
						customer_first_name, customer_middle_name, customer_last_name, name_on_card, customer_title_id, 
						date_issued, customer_type_id, currency_id, resident_id, cms_id, contract_number, Id_number,contact_number, CustomerId,
						domicile_branch_id)
			VALUES (@audit_user_id, 0, @account_type_id, 
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_account_number)),
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_first_name)),
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_middle_name)), 
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_last_name)), 
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),UPPER(@name_on_card))), 
					@customer_title_id, @status_date, @customer_type_id, @currency_id, @resident_id, @cms_id, @contract_number,
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@idnumber)),
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@contact_number)),
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_id)),
					@domicile_branch_id)		

			SET @new_customer_account_id = SCOPE_IDENTITY()	

			

				INSERT INTO print_jobs
                         (printer_id, status_date, print_statuses_id, audit_user_id)
			VALUES        (-1,SYSDATETIMEOFFSET(),0,@audit_user_id)	

	set @print_job_id=SCOPE_IDENTITY()
	

	Insert customer_account_cards(customer_account_id,card_id,print_job_id) values(@new_customer_account_id,@card_id,@print_job_id)

			--Log audit stuff
			DECLARE @branchcardstatus  varchar(max),
					@Scenario  varchar(max),
					@audit_msg varchar(max),
					@cardnumber varchar(16),
					@obranch varchar(max),
					@dbranch varchar(max)

			SELECT @obranch = branch_code from branch where branch_id = @branch_id
			SELECT @dbranch = branch_code from branch where branch_id = @delivery_branch_id

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
								', o/branch:' + COALESCE(@obranch, 'UNKNWON') +  
								', d/branch:' + COALESCE(@dbranch, 'UNKNWON') +  
								', ' + COALESCE(@Scenario, 'UNKNWON')

			--log the audit record		
			EXEC usp_insert_audit @audit_user_id, 
									3,---IssueCard
									NULL, 
									@audit_workstation, 
									@audit_msg, 
									NULL, NULL, NULL, NULL
END
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