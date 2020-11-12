CREATE PROCEDURE [dbo].[usp_print_batch_status_change] 
	@print_batch_id bigint,
	@print_batch_statuses_id int = null,
	@new_print_batch_statuses_id int = null,
	@status_notes varchar(150) = null,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@autogenerate_dist_batch_id bit,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [PRINT_BATCH_STATUS_CHANGE]
		BEGIN TRY 
			
			DECLARE @current_print_batch_status_id int,
					@audit_msg varchar(max),
					@original_batch_type_id int		
					
			
			--Check that someone hasn't already updated the dist batch
			IF(dbo.[PrintBatchInCorrectStatus](@print_batch_statuses_id, @new_print_batch_statuses_id, @print_batch_id) <> 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN
					--Update the pin batch status.
					UPDATE print_batch_status
						SET [print_batch_statuses_id] = @new_print_batch_statuses_id, 
						[user_id] = @audit_user_id, 
						[status_date] = SYSDATETIMEOFFSET(), 
						[status_notes] = @status_notes
					OUTPUT Deleted.* INTO pin_batch_status_audit
					WHERE [print_batch_id] = @print_batch_id


					
				
					DECLARE @print_batch_status_name varchar(50),
							@print_batch_ref varchar(100)

					SELECT @print_batch_status_name =  print_batch_statuses
					FROM print_batch_statuses
					WHERE print_batch_statuses_id = @print_batch_statuses_id

					SELECT @print_batch_ref = print_batch_reference
					FROM print_batch
					WHERE print_batch_id = @print_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@print_batch_id AS varchar(max)) +
										', ' + COALESCE(@print_batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@print_batch_status_name, 'UNKNOWN')
								   
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
				EXEC usp_get_print_batch @print_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [PRINT_BATCH_STATUS_CHANGE]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [PRINT_BATCH_STATUS_CHANGE]
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

	

