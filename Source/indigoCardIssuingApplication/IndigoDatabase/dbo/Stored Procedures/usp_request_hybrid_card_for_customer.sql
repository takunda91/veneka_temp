Create PROCEDURE [dbo].[usp_request_hybrid_card_for_customer] 
	@delivery_branch_id int,
	@branch_id int,
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
	@idnumber varchar(50),
	@contact_number varchar(50),
	@customer_id varchar(50),
	@fee_waiver_YN bit = NULL,
	@fee_editable_YN bit = NULL,
	@fee_charged decimal(10,4) = NULL,
	@fee_overridden_YN bit = NULL,	
	@card_issue_method_id int,
	@cbs_account_type varchar(100),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@request_id bigint OUTPUT,
	@ResultCode int OUTPUT,
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
					@hybrid_request_statuses_id int

			SET @hybrid_request_statuses_id = 0

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			
			 set @status_date=SYSDATETIMEOFFSET()
			DECLARE @branch_type_id int

			SELECT 	@branch_type_id = branch_type_id from branch where branch_id=@branch_id

			IF(@branch_type_id =2 and @card_issue_method_id=1 )
			BEGIN
			--Inserting a card record with an empty card number, the card number will be generated later in the process. 
			-- when that happens this record should be populated with a card number.
			INSERT INTO hybrid_requests	([product_id],[ordering_branch_id],[branch_id],[origin_branch_id],[delivery_branch_id], 
									card_issue_method_id, card_priority_id) 
				VALUES(@product_id, @branch_id, @branch_id, @branch_id, @delivery_branch_id, @card_issue_method_id, @card_priority_id)

			SET @request_id = SCOPE_IDENTITY();

			--Update card with reference number
			--Generate card reference
			DECLARE @card_ref varchar(100)
			SET @card_ref =  'CCR' + CONVERT(VARCHAR(8), SYSDATETIMEOFFSET(), 112) + CAST(@product_id AS varchar(max)) + CAST(@request_id AS varchar(max))

			UPDATE hybrid_requests
				SET request_reference = @card_ref
			WHERE request_id = @request_id
			DECLARE @fee_id as int, @new_fee_id as int

			INSERT INTO fee_charged(fee_waiver_YN,fee_editable_YN,fee_charged,fee_overridden_YN,operator_user_id)
									VALUES(@fee_waiver_YN,@fee_editable_YN,@fee_charged,@fee_overridden_YN,@audit_user_id)
					SET @new_fee_id = SCOPE_IDENTITY()	
					UPDATE hybrid_requests
					SET fee_id=@new_fee_id
					WHERE request_id = @request_id
			

			--The initial card status.
			INSERT hybrid_request_status (request_id, branch_id, hybrid_request_statuses_id, status_date, [user_id], operator_user_id)
			VALUES (@request_id, @branch_id, @hybrid_request_statuses_id, @status_date, @audit_user_id, @audit_user_id)

			
			
			--NOTIFICATION for creation of request
			--exec usp_notification_branch_add @card_id, @branch_card_statuses_id

			--Check if we need to do maker/checker for the request.
			--If no maker checker then we "Auto" approve the card for issue.
			IF ((SELECT [issuer].maker_checker_YN
				FROM [issuer] INNER JOIN [branch]
					ON [issuer].issuer_id = [branch].issuer_id
				WHERE [branch].branch_id = @branch_id) = 0)		
				BEGIN		
					SET @hybrid_request_statuses_id = 1       --Not MakerChecker	
					--Add additional second to the request so that the order is preserved,
					UPDATE hybrid_request_status 
					SET branch_id = @branch_id, 
						hybrid_request_statuses_id = @hybrid_request_statuses_id, 
						status_date = DATEADD(ss, 1, @status_date), 
						[user_id] = @audit_user_id, 
						operator_user_id = @audit_user_id,
						comments = 'Auto Approve Card For Issue'						
					OUTPUT Deleted.* INTO hybrid_request_status_audit
					WHERE request_id = @request_id	

				

					--NOTIFICATION for approval
					--exec usp_notification_branch_add @card_id, @branch_card_statuses_id
				END	
						 

			--Save customer details
			INSERT customer_account
					([user_id], card_issue_reason_id, account_type_id, customer_account_number,
						customer_first_name, customer_middle_name, customer_last_name, name_on_card, customer_title_id, 
						date_issued, customer_type_id, currency_id, resident_id, cms_id, contract_number, Id_number,contact_number, CustomerId,
						domicile_branch_id,cbs_account_type)
			VALUES (@audit_user_id, @card_issue_reason_id, @account_type_id, 
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_account_number)),
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_first_name)),
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_middle_name)), 
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_last_name)), 
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),UPPER(@name_on_card))), 
					@customer_title_id, @status_date, @customer_type_id, @currency_id, @resident_id, @cms_id, @contract_number,
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@idnumber)),
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@contact_number)),
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_id)),
					@domicile_branch_id,@cbs_account_type)		

			SET @new_customer_account_id = SCOPE_IDENTITY()			
			
			insert customer_account_requests([customer_account_id],[request_id]) VALUES(@new_customer_account_id,@request_id)
			--Log audit stuff
			DECLARE @hybridrequeststatus  varchar(max),
					@Scenario  varchar(max),
					@audit_msg varchar(max),
					@cardnumber varchar(16),
					@obranch varchar(max),
					@dbranch varchar(max)

			SELECT @obranch = branch_code from branch where branch_id = @branch_id
			SELECT @dbranch = branch_code from branch where branch_id = @delivery_branch_id

			SELECT  @hybridrequeststatus =  hybrid_request_statuses.hybrid_request_statuses
			FROM    hybrid_request_statuses 
			WHERE	hybrid_request_statuses.hybrid_request_statuses_id = @hybrid_request_statuses_id

			SELECT  @Scenario =  card_issue_reason.[card_issuer_reason_name]
			FROM	card_issue_reason 
			WHERE	card_issue_reason.[card_issue_reason_id] = @card_issue_reason_id

			SET @audit_msg =  'hybrid request-' + 
								COALESCE(@hybridrequeststatus, 'UNKNWON') +  
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