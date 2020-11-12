CREATE PROCEDURE [dbo].[usp_issue_card_to_customer]
	@card_id bigint,	
    @customer_account_number varchar(27),
	@domicile_branch_id int,
	@account_type_id int,
	@card_issue_reason_id int,
	@customer_first_name varchar(50),
	@customer_middle_name varchar(50),
	@customer_last_name varchar(50),
	@name_on_card varchar(30),
	@customer_title_id int,
	@cms_id varchar(50),
	@customer_id varchar(150),
	@contract_number varchar(50),
	@contact_number varchar(50),
	@id_number varchar(50),
	@currency_id int,
	@resident_id int,
	@customer_type_id int,
	@cbs_account_type varchar(50),
	@fee_detail_id int = NULL,
	@fee_waiver_YN bit = NULL,
	@fee_editable_YN bit = NULL,
	@fee_charged decimal(10,4) = NULL,
	@fee_overridden_YN bit = NULL,
	@product_fields as dbo.key_binary_value_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [ISSUE_CARD_TO_CUST_TRAN]
		BEGIN TRY 

			IF @customer_middle_name IS NULL
				SET @customer_middle_name = ''
			
			DECLARE @new_customer_account_id bigint,
					@branch_id int,
					@current_card_status_id int,
					@status_date datetimeoffset

			--get the current status for the card
			SELECT @current_card_status_id = branch_card_statuses_id
			FROM branch_card_status_current
			WHERE card_id = @card_id
										  
			--Check that someone hasn't already updated the card
			IF(@current_card_status_id = 1 OR @current_card_status_id = 3)				
				BEGIN
					--get the vat rate which is used for this customer
					DECLARE @vat decimal(7, 4)

					SELECT @vat = vat
					FROM [product_fee_charge]
					WHERE fee_detail_id = @fee_detail_id
						AND currency_id = @currency_id
						AND card_issue_reason_id = @card_issue_reason_id


					OPEN SYMMETRIC KEY Indigo_Symmetric_Key
					DECRYPTION BY CERTIFICATE Indigo_Certificate

					SET @status_date = SYSDATETIMEOFFSET()

					SELECT @branch_id = branch_id
					FROM [cards]
					WHERE card_id = @card_id

					--Update the cards status.
					UPDATE branch_card_status 
					SET branch_id = @branch_id, 
						branch_card_statuses_id = 2, 
						status_date = @status_date, 
						[user_id] = @audit_user_id, 
						operator_user_id = @audit_user_id,
						branch_card_code_id = NULL,
						comments = NULL,
						pin_auth_user_id = NULL
					OUTPUT Deleted.* INTO branch_card_status_audit
					WHERE card_id = @card_id

					--INSERT branch_card_status
					--		(card_id, branch_id, branch_card_statuses_id, status_date, [user_id], operator_user_id)
					--VALUES (@card_id, @branch_id, 2, @status_date, @audit_user_id, @audit_user_id) 

					--Save customer details
					INSERT customer_account
							([user_id], card_issue_reason_id, account_type_id, customer_account_number,
								customer_first_name, customer_middle_name, customer_last_name, name_on_card, contact_number,Id_number, customer_title_id, 
								date_issued, customer_type_id, currency_id, resident_id, cms_id, contract_number, domicile_branch_id, CustomerId,cbs_account_type)
					VALUES (@audit_user_id, @card_issue_reason_id, @account_type_id, 
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_account_number)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_first_name)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_middle_name)), 
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_last_name)), 
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@name_on_card)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@contact_number)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@id_number)) ,
							@customer_title_id, @status_date, @customer_type_id, @currency_id, @resident_id, 
							@cms_id, @contract_number, @domicile_branch_id,
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_id)),@cbs_account_type )	
							
					SET @new_customer_account_id = SCOPE_IDENTITY()				
					
					INSERT customer_account_cards(customer_account_id,card_id) VALUES(@new_customer_account_id,@card_id)
				
					DECLARE @fee_id as int, @new_fee_id as int
					SELECT @fee_id = fee_id from cards where cards.card_id=@card_id
					IF @fee_id is NULL
					BEGIN

					INSERT INTO fee_charged(fee_waiver_YN,fee_editable_YN,fee_charged,vat,fee_overridden_YN,operator_user_id)
									VALUES(@fee_waiver_YN,@fee_editable_YN,@fee_charged,@vat,@fee_overridden_YN,@audit_user_id)
					SET @new_fee_id = SCOPE_IDENTITY()	
					UPDATE [cards]
					SET fee_id=@new_fee_id
					WHERE card_id = @card_id
					END
					ELSE
					BEGIN
					UPDATE [fee_charged]
					SET fee_waiver_YN = @fee_waiver_YN,
						fee_editable_YN = @fee_editable_YN, 
						fee_charged = @fee_charged,
						vat = @vat,
						fee_overridden_YN = @fee_overridden_YN
					WHERE fee_id = @fee_id
					END

					--Update fee's for card
					

					--Update Product Fields
					INSERT INTO customer_fields (customer_account_id, product_field_id, value)
						SELECT @new_customer_account_id, pf.[key], ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CAST(pf.[value] as varbinary(max)))
						FROM @product_fields pf INNER JOIN product_fields
							ON pf.[key] = product_fields.product_field_id
						WHERE product_fields.print_field_type_id = 0 
						--and product_fields.deleted=0

					--Update Product Image Fields
					INSERT INTO customer_image_fields (customer_account_id, product_field_id, value)
						SELECT @new_customer_account_id, pf.[key], pf.[value]
						FROM @product_fields pf INNER JOIN product_fields
							ON pf.[key] = product_fields.product_field_id
						WHERE product_fields.print_field_type_id = 1  
						--and product_fields.deleted=0
					
					--Log audit stuff
					DECLARE @branchcardstatus  varchar(max),
							@Scenario  varchar(max),
							@audit_msg varchar(max),
							@cardnumber varchar(16)

					SELECT @cardnumber = CONVERT(VARCHAR,DECRYPTBYKEY(cards.card_number))
					FROM cards 
					WHERE cards.card_id = @card_id

					SELECT  @branchcardstatus =  branch_card_statuses.branch_card_statuses_name
					FROM    branch_card_statuses 
					WHERE	branch_card_statuses.branch_card_statuses_id = 2

					SELECT  @Scenario =  card_issue_reason.[card_issuer_reason_name]
					FROM	card_issue_reason 
					WHERE	card_issue_reason.[card_issue_reason_id] = @card_issue_reason_id

					SET @audit_msg =  COALESCE(@branchcardstatus, 'UNKNWON') +  
									  ', ' + dbo.MaskString(@cardnumber, 6, 4) + 
									  ', cust id:' + COALESCE(CAST(@cms_id as varchar(max)), 'n/a') +
									  ', a/c:' + dbo.MaskString(@customer_account_number, 3, 4) + 
									  ', ' + COALESCE(@Scenario, 'UNKNWON')

					--SET @audit_msg = 'Issued card ' + dbo.MaskString(@cardnumber, 6, 4) +
					--				 ' , acc:'+ dbo.MaskString(@customer_account_number, 3, 4) + 
					--				 ', typeid :'+CAST(@customer_account_type_id as nvarchar(50))

					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
										 3,---IssueCard
										 NULL, 
										 @audit_workstation, 
										 @audit_msg, 
										 NULL, NULL, NULL, NULL

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
					SET @ResultCode = 0					
				END
			ELSE
				BEGIN
					SET @ResultCode = 100
				END
			
				
				COMMIT TRANSACTION [ISSUE_CARD_TO_CUST_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_TO_CUST_TRAN]
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