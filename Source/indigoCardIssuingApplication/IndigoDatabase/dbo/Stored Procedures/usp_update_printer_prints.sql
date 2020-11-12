CREATE PROCEDURE [dbo].[usp_update_printer_prints]
	-- Add the parameters for the stored procedure here
	@serial_no NVARCHAR(100),
	@total_prints nvarchar(100),
	@audit_user_id bigint,
	@audit_workstation NVARCHAR(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	BEGIN TRANSACTION [UPDATE_PRINT_TRAN]
		BEGIN TRY 
			update printer
			 set total_prints= @total_prints, 
				audit_user_id=@audit_user_id,
				last_update_date=SYSDATETIMEOFFSET()
			WHERE serial_no=@serial_no		
			
			Declare @audit_description nvarchar(500)
			SELECT @audit_description = 'printer total_Prints updated: ' + @serial_no  + ', total_prints:' + @total_prints 
																	
				EXEC usp_insert_audit @audit_user_id, 
									 4,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 -1, NULL, NULL, NULL
		 COMMIT TRANSACTION [UPDATE_PRINT_TRAN]
							
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_PRINT_TRAN]
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