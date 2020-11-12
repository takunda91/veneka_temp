-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_request_pin_mailer_reprints] 
	@card_issue_method_id int,
	@issuer_id int,
	@branch_id int = null,
	@product_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@cards_in_batch int OUTPUT,
	@pin_batch_id int OUTPUT,
	@pin_batch_ref varchar(100) OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [TRAN_CREATE_PIN_REPRINT_BATCH]
		BEGIN TRY 

		SET @cards_in_batch = 0
		SET	@pin_batch_id = 0
		SET @pin_batch_ref = ''

		DECLARE @pin_mailer_reprint_status_id int
		SET @pin_mailer_reprint_status_id = 1

		--Only create a batch if there are cards for the batch
		IF( (SELECT COUNT(*) 
			 FROM [pin_mailer_reprint_status_current]
					INNER JOIN [branch]
						ON [pin_mailer_reprint_status_current].branch_id = [branch].branch_id					
			 WHERE [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id = @pin_mailer_reprint_status_id
					AND product_id = @product_id
					AND card_issue_method_id = @card_issue_method_id
					AND [pin_mailer_reprint_status_current].branch_id = COALESCE(@branch_id, [pin_mailer_reprint_status_current].branch_id)
					AND issuer_id = @issuer_id) = 0)
		BEGIN
			SET @ResultCode = 400
			COMMIT TRANSACTION [TRAN_CREATE_PIN_REPRINT_BATCH]
		END			
		ELSE
			BEGIN

				DECLARE @cards_total int = 0,
						@batch_branch_id int,
						@audit_msg nvarchar(500)

				--create the reprint batch
				INSERT INTO [pin_batch]
					([card_issue_method_id],[issuer_id],[branch_id], [no_cards],[date_created],[pin_batch_reference],[pin_batch_type_id])
				VALUES (@card_issue_method_id, @issuer_id, @branch_id, 0, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), 2)

				SET @pin_batch_id = SCOPE_IDENTITY();

				--Add cards to production batch
				INSERT INTO [pin_batch_cards]
					([pin_batch_id],[card_id],[pin_batch_cards_statuses_id])
				SELECT @pin_batch_id, card_id, 0
				FROM [pin_mailer_reprint_status_current]
					INNER JOIN [branch]
						ON [pin_mailer_reprint_status_current].branch_id = [branch].branch_id					
				WHERE [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id = @pin_mailer_reprint_status_id
					AND product_id = @product_id
					AND card_issue_method_id = @card_issue_method_id
					AND [pin_mailer_reprint_status_current].branch_id = COALESCE(@branch_id, [pin_mailer_reprint_status_current].branch_id)
					AND issuer_id = @issuer_id


				--add reprint batch status of created
				INSERT INTO [dbo].[pin_batch_status]
					([pin_batch_id],[pin_batch_statuses_id],[user_id],[status_date],[status_notes])
				VALUES(@pin_batch_id, 0, @audit_user_id, SYSDATETIMEOFFSET(), 'Pin Mailer Reprint Batch Created')

				--Generate dist batch reference
				SELECT @pin_batch_ref =  [issuer].issuer_code + '' + 
										  CONVERT(VARCHAR(MAX),[issuer_product].product_id) + '' +										  
										  CONVERT(VARCHAR(8), SYSDATETIMEOFFSET(), 112) + '' +
										  CAST(@pin_batch_id AS varchar(max))
				FROM [issuer]					
					INNER JOIN [issuer_product]
						ON [issuer_product].issuer_id = [issuer].issuer_id
				WHERE [issuer].issuer_id = @issuer_id

				SELECT @cards_in_batch = COUNT(*)
				FROM pin_batch_cards
				WHERE pin_batch_id = @pin_batch_id 

				--UPDATE reprint batch with reference and number of cards
				UPDATE [pin_batch]
				SET [pin_batch_reference] = @pin_batch_ref,
					[no_cards] = @cards_in_batch
				WHERE [pin_batch].pin_batch_id = @pin_batch_id


				update [pin_mailer_reprint]
				set pin_mailer_reprint_status_id=2,
				comments='Assigned to batch',
				status_date=SYSDATETIMEOFFSET(),
				[user_id]=@audit_user_id
				OUTPUT Deleted.* INTO pin_mailer_reprint_audit
				where card_id in (SELECT card_id FROM pin_batch_cards
				WHERE pin_batch_id = @pin_batch_id)

				----UPDATE pin reprint status for those cards that have been added to the new reprint batch.
				--INSERT INTO [pin_mailer_reprint]
				--	(pin_mailer_reprint_status_id, card_id, comments, status_date, [user_id])
				--SELECT 2, card_id, 'Assigned to batch', SYSDATETIMEOFFSET(), @audit_user_id
				--FROM pin_batch_cards
				--WHERE pin_batch_id = @pin_batch_id	

				--Add audit for pin batch creation	
				DECLARE @pin_batch_status_name varchar(50)
				SELECT @pin_batch_status_name = pin_batch_statuses.pin_batch_statuses_name
				FROM pin_batch_statuses
				WHERE pin_batch_statuses_id = 0
											
				SET @audit_msg = 'Create: ' + CAST(@pin_batch_id AS varchar(max)) +
									', ' + COALESCE(@pin_batch_ref, 'UNKNOWN') +
									', ' + COALESCE(@pin_batch_status_name, 'UNKNOWN')
								   
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										2,
										NULL, 
										@audit_workstation, 
										@audit_msg, 
										NULL, NULL, NULL, NULL

				SELECT @ResultCode = 0
				COMMIT TRANSACTION [TRAN_CREATE_PIN_REPRINT_BATCH]	

			END					
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [TRAN_CREATE_PIN_REPRINT_BATCH]
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




