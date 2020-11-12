CREATE PROCEDURE [dbo].[usp_insert_printer]
	-- Add the parameters for the stored procedure here
	@serial_no varchar(100),
	@manufacturer NVARCHAR(100),
	@model NVARCHAR(100),
	@firmware_version NVARCHAR(100),
	@branch_id int,
	@total_prints int,
	@next_clean int,
	@print_job_id bigint,
	@audit_user_id bigint,
	@audit_workstation NVARCHAR(100),
	@new_printer_id int OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	BEGIN TRANSACTION [INSERT_PRINT_TRAN]
		BEGIN TRY 
	
		if((SElect count(*) from printer where serial_no=@serial_no)<1)
		BEGIN
			INSERT INTO printer
									 (serial_no,model,manufacturer,firmware_version, branch_id, total_prints, next_clean, audit_user_id, last_update_date)
			VALUES        (@serial_no,@model,@manufacturer,@firmware_version,@branch_id,@total_prints,@next_clean,@audit_user_id,SYSDATETIMEOFFSET())


			set @new_printer_id= SCOPE_IDENTITY()
			
			END
			ELSE
			BEGIN
			select @new_printer_id= printer_id from printer where serial_no=@serial_no
			END

			update print_jobs
			set printer_id= @new_printer_id
			where print_job_id=@print_job_id
			set @ResultCode=0
			Declare @audit_description nvarchar(500)
			SELECT @audit_description = 'printer registered: ' + @serial_no  + ', branch Id:' + cast ( @branch_id  as varchar(50))
																	
				EXEC usp_insert_audit @audit_user_id, 
									 4,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 -1, NULL, NULL, NULL
					COMMIT TRANSACTION [INSERT_PRINT_TRAN]
				
		
							
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_PRINT_TRAN]
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