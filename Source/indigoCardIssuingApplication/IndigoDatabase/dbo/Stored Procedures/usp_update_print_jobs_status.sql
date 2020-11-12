Create PROCEDURE [dbo].[usp_update_print_jobs_status]
	-- Add the parameters for the stored procedure here
	@print_job_id bigint,
	@print_statuses_id int,
	@comments nvarchar(100),
	@audit_user_id bigint,
	@audit_workstation NVARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	BEGIN TRANSACTION [UPDATE_PRINT_JOBS]
		BEGIN TRY 

	

			UPDATE print_jobs 
					SET print_statuses_id = @print_statuses_id, 
						audit_user_id = @audit_user_id, 
						comments=@comments,
						[status_date] = SYSDATETIMEOFFSET() 
					OUTPUT Deleted.* INTO dbo.[print_jobs_audit]
					WHERE print_job_id = @print_job_id

	COMMIT TRANSACTION [UPDATE_PRINT_JOBS]
							
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_PRINT_JOBS]
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