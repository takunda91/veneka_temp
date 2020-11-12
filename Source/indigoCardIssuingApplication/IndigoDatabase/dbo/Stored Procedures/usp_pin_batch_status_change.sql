
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Change pin batch status
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_batch_status_change] 
	@pin_batch_id bigint,
	@status_notes varchar(150) = null,
	@pin_batch_status_id int,
	@new_pin_batch_status_id int,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [PIN_BATCH_STATUS_CHANGE]
		BEGIN TRY 
			
			DECLARE @current_dist_batch_status_id int,
					@audit_msg varchar(max),
					@original_batch_type_id int,
					@new_batch_type_id int,
					@new_pin_card_statuses_id int		
					
			--If the statuses are the same then we arent changing the batches status, just return pin batch
			--IF(@pin_batch_status_id = @new_pin_batch_status_id)	
			--	BEGIN
			--		SET @ResultCode = 0	
			--	END					  
			--Check that someone hasn't already updated the dist batch
			IF(dbo.PinBatchInCorrectStatus(@pin_batch_status_id, @new_pin_batch_status_id, @pin_batch_id) = 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN		
					--Check if we need to create dist batch
					SELECT @original_batch_type_id = [pin_batch_statuses_flow].pin_batch_type_id,
							  @new_batch_type_id = flow_pin_batch_type_id,
							  @new_pin_card_statuses_id = flow_pin_card_statuses_id
						FROM [pin_batch_statuses_flow]
							INNER JOIN [pin_batch]
								ON [pin_batch_statuses_flow].card_issue_method_id = [pin_batch].card_issue_method_id
									AND [pin_batch_statuses_flow].pin_batch_type_id = [pin_batch].pin_batch_type_id
							INNER JOIN [pin_batch_status_current]
								ON [pin_batch_status_current].pin_batch_statuses_id = [pin_batch_statuses_flow].pin_batch_statuses_id
									AND [pin_batch_status_current].pin_batch_id = [pin_batch].pin_batch_id
					WHERE [pin_batch].pin_batch_id = @pin_batch_id

					--Update the pin batch status.
					UPDATE pin_batch_status
						SET [pin_batch_statuses_id] = @new_pin_batch_status_id, 
						[user_id] = @audit_user_id, 
						[status_date] = SYSDATETIMEOFFSET(), 
						[status_notes] = @status_notes
					OUTPUT Deleted.* INTO pin_batch_status_audit
					WHERE [pin_batch_id] = @pin_batch_id


					--INSERT pin_batch_status
					--		([pin_batch_id], [pin_batch_statuses_id], [user_id], [status_date], [status_notes])
					--VALUES (@pin_batch_id, @new_pin_batch_status_id, @audit_user_id, SYSDATETIMEOFFSET(), @status_notes)

					IF(@new_pin_card_statuses_id IS NOT NULL)
					BEGIN
						--Update the cards linked to the dist batch with the new status.
						UPDATE pin_batch_cards
						SET pin_batch_cards_statuses_id = @new_pin_card_statuses_id
						WHERE pin_batch_id = @pin_batch_id

						IF (@new_pin_card_statuses_id = 4)
						BEGIN
							--Update the pin reprint cards to completed.
							UPDATE t
								SET t.[user_id] = @audit_user_id
									  ,t.[pin_mailer_reprint_status_id] = 3
									  ,t.[status_date] = SYSDATETIMEOFFSET()
									  ,t.[comments] = ''
							OUTPUT Deleted.* INTO  [pin_mailer_reprint_audit]
							FROM [pin_mailer_reprint] t
									INNER JOIN [pin_batch_cards] s ON t.card_id = s.card_id AND t.pin_mailer_reprint_status_id = 2
							WHERE s.pin_batch_id = @pin_batch_id
							
							--INSERT INTO [pin_mailer_reprint] (card_id, comments, pin_mailer_reprint_status_id, status_date, [user_id])
							--SELECT [pin_mailer_reprint].card_id, '', 3, SYSDATETIMEOFFSET(), @audit_user_id
							--FROM [pin_batch_cards]
							--		INNER JOIN [pin_mailer_reprint]
							--			ON [pin_batch_cards].card_id = [pin_mailer_reprint].card_id
							--			AND [pin_mailer_reprint].pin_mailer_reprint_status_id = 2
							--WHERE pin_batch_id = @pin_batch_id
						END
					END

					IF(@original_batch_type_id != 1 AND @new_batch_type_id = 1)
					BEGIN
						EXEC [usp_pin_prod_to_pin_batch] @pin_batch_id,
														@audit_user_id,
														@audit_workstation
					END
				
					--AUDIT
					DECLARE @pin_batch_status_name varchar(50),
							@pin_batch_ref varchar(100)

					SELECT @pin_batch_status_name =  pin_batch_statuses_name
					FROM pin_batch_statuses
					WHERE pin_batch_statuses_id = @pin_batch_status_id

					SELECT @pin_batch_ref = pin_batch_reference
					FROM pin_batch
					WHERE pin_batch_id = @pin_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@pin_batch_id AS varchar(max)) +
										', ' + COALESCE(@pin_batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@pin_batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					SET @ResultCode = 0					
				END

				--Fetch the pin batch with latest details
				EXEC usp_get_pin_batch @pin_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [PIN_BATCH_STATUS_CHANGE]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [PIN_BATCH_STATUS_CHANGE]
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