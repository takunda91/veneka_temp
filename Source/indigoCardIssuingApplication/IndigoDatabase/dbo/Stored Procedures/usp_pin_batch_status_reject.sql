
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Change batch status - Reject
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_batch_status_reject] 
	@pin_batch_id bigint,
	@new_pin_batch_status_id int,
	@status_notes varchar(150) = null,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [PIN_BATCH_STATUS_CHANGE_REJECT]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@new_pin_card_statuses_id int

			--Check that someone hasn't already updated the pin batch
			IF(dbo.PinBatchInCorrectStatusReject(@new_pin_batch_status_id, @pin_batch_id) = 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN
					SELECT @new_pin_card_statuses_id = reject_pin_card_statuses_id
						FROM [pin_batch_statuses_flow]
							INNER JOIN [pin_batch]
								ON [pin_batch_statuses_flow].card_issue_method_id = [pin_batch].card_issue_method_id
									AND [pin_batch_statuses_flow].pin_batch_type_id = [pin_batch].pin_batch_type_id
							INNER JOIN [pin_batch_status_current]
								ON [pin_batch_status_current].pin_batch_statuses_id = [pin_batch_statuses_flow].pin_batch_statuses_id
									AND [pin_batch_status_current].pin_batch_id = [pin_batch].pin_batch_id
					WHERE [pin_batch].pin_batch_id = @pin_batch_id
							AND [pin_batch_statuses_flow].reject_pin_batch_statuses_id = @new_pin_batch_status_id


					--Update the batch status.
					UPDATE pin_batch_status
						SET [pin_batch_statuses_id] = @new_pin_batch_status_id, 
						[user_id] = @audit_user_id, 
						[status_date] = SYSDATETIMEOFFSET(), 
						[status_notes] = @status_notes
					OUTPUT Deleted.* INTO pin_batch_status_audit
					WHERE [pin_batch_id] = @pin_batch_id

					--INSERT [pin_batch_status]
					--		([pin_batch_id], [pin_batch_statuses_id], [user_id], [status_date], [status_notes])
					--VALUES (@pin_batch_id, @new_pin_batch_status_id, @audit_user_id, SYSDATETIMEOFFSET(), @status_notes)

					--Check if we need to update the card status
					IF (@new_pin_card_statuses_id IS NOT NULL)
					BEGIN 
						--Update the cards linked to the pin batch with the new status.
						UPDATE pin_batch_cards
						SET pin_batch_cards_statuses_id = @new_pin_card_statuses_id
						WHERE pin_batch_id = @pin_batch_id
					END
				
					--AUDIT
					DECLARE @batch_status_name varchar(100),
							@batch_ref varchar(100)

					SELECT @batch_status_name =  pin_batch_statuses_name
					FROM pin_batch_statuses
					WHERE pin_batch_statuses_id = @new_pin_batch_status_id

					SELECT @batch_ref = pin_batch_reference
					FROM pin_batch
					WHERE pin_batch_id = @pin_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@pin_batch_id AS varchar(max)) +
										', ' + COALESCE(@batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					SET @ResultCode = 0					
				END

				--Fetch the batch with latest details
				EXEC usp_get_pin_batch @pin_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [PIN_BATCH_STATUS_CHANGE_REJECT]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [PIN_BATCH_STATUS_CHANGE_REJECT]
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