-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_threed_batch_status] 
	-- Add the parameters for the stored procedure here
	
	@threed_batch_id bigint,
	@threed_batch_statuses_id int,
	@status_note varchar(100),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION [UPDATE_THREED_BATCH_CHARGE_TRAN]
		BEGIN TRY 
		UPDATE threed_secure_batch_status 
					SET threed_batch_statuses_id = @threed_batch_statuses_id, 
						[user_id] = @audit_user_id, 
						[status_date] = SYSDATETIMEOFFSET(), 
						status_note = @status_note	
					OUTPUT Deleted.* INTO threed_secure_batch_status_audit
					WHERE threed_batch_id = @threed_batch_id

					Declare @audit_description as varchar(max)

					SET @audit_description = 'ThreedBatch status updated to Recreated for : ' + CAST(@threed_batch_id AS VARCHAR(max))	
										
										 	
			EXEC usp_insert_audit @audit_user_id, 
									4,
									NULL, 
									@audit_workstation, 
									@audit_description, 
									null, NULL, NULL, NULL

					COMMIT TRANSACTION [UPDATE_THREED_BATCH_CHARGE_TRAN]
			SET @ResultCode = 0
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_THREED_BATCH_CHARGE_TRAN]
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
