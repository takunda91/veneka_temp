﻿-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_export_batch_status_reject]
	-- Add the parameters for the stored procedure here
	@export_batch_id bigint,
	@comments varchar(100),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [EXPORT_BATCH_REJECT]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@current_export_batch_id int,
					@new_export_batch_statuses_id int = 4

			SELECT @current_export_batch_id = export_batch_statuses_id FROM [export_batch_status_current] WHERE export_batch_id = @export_batch_id
						
						  
			--Check that someone hasn't already updated the dist batch
			IF(@current_export_batch_id = 0 OR @current_export_batch_id = 3)
				BEGIN	

					DECLARE @auditDatetime DATETIMEOFFSET = SYSDATETIMEOFFSET()

					UPDATE [export_batch_status]
						SET [export_batch_statuses_id] = @new_export_batch_statuses_id, 
							[status_date] = @auditDatetime, 
							[user_id] = @audit_user_id, 
							[comments] = @comments
					OUTPUT Deleted.* INTO [export_batch_status_audit]
					WHERE [export_batch_id] = @export_batch_id

					--INSERT INTO export_batch_status (export_batch_id, export_batch_statuses_id, status_date, [user_id], comments)
					--VALUES (@export_batch_id, @new_export_batch_statuses_id, @auditDatetime, @audit_user_id, @comments)
					

					--AUDIT 
					DECLARE @batch_status_name varchar(100),
							@batch_ref varchar(100)

					SELECT @batch_status_name =  export_batch_statuses_name
					FROM export_batch_statuses
					WHERE export_batch_statuses_id = @new_export_batch_statuses_id

					SELECT @batch_ref = batch_reference
					FROM export_batch
					WHERE export_batch_id = @export_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@export_batch_id AS varchar(max)) +
										', ' + COALESCE(@batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											11,
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
				EXEC usp_get_export_batch @export_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [EXPORT_BATCH_REJECT]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [EXPORT_BATCH_REJECT]
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