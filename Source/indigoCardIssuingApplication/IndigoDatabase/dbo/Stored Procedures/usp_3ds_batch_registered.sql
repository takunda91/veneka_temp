-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Changes a 3DS batch status to registered
-- =============================================
CREATE PROCEDURE [dbo].[usp_3ds_batch_registered]
	@threeds_batch_id BIGINT,
	@language_id INT,
	@audit_user_id BIGINT,
	@audit_workstation NVARCHAR(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	    BEGIN TRANSACTION [THREEDS_BATCH_REGISTERED]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@current_threeds_batch_id int,
					@new_threeds_batch_statuses_id int = 1

			SELECT @current_threeds_batch_id = threed_batch_statuses_id FROM [dbo].[threed_secure_batch_status] WHERE threed_batch_id = @threeds_batch_id
						
						  
			--Check that someone hasn't already updated the batch
			IF(@current_threeds_batch_id = 0)
				BEGIN	

					DECLARE @auditDatetime DATETIMEOFFSET = SYSDATETIMEOFFSET()
					
					UPDATE [threed_secure_batch_status]
					SET [threed_batch_statuses_id] = @new_threeds_batch_statuses_id, 
							[status_date] = @auditDatetime, 
							[user_id] = @audit_user_id, 
							[status_note] = 'BATCH REGISTERD'
					OUTPUT Deleted.* INTO [threed_secure_batch_status_audit]
					WHERE [threed_batch_id] = @threeds_batch_id					

					--AUDIT 
					DECLARE @batch_status_name varchar(100),
							@batch_ref varchar(100)

					SELECT @batch_status_name =  threed_batch_statuses_name
					FROM threed_secure_batch_statuses
					WHERE threed_batch_statuses_id = @new_threeds_batch_statuses_id

					SELECT @batch_ref = batch_reference
					FROM threed_secure_batch
					WHERE threed_batch_id = @threeds_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@threeds_batch_id AS varchar(max)) +
										', ' + COALESCE(@batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											12,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					 

					SET @ResultCode = 0					
				END
			ELSE
				BEGIN
					SET @ResultCode = 100
				END

				--Fetch the batch with latest details


				COMMIT TRANSACTION [THREEDS_BATCH_REGISTERED]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [THREEDS_BATCH_REGISTERED]
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
