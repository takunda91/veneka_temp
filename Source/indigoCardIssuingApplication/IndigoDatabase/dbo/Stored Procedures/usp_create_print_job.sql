CREATE PROCEDURE [dbo].[usp_create_print_job]
	-- Add the parameters for the stored procedure here
	@serial_no NVARCHAR(100),
	@customer_id bigint,
	@print_statuses_id int,
	@audit_user_id bigint,
	@audit_workstation NVARCHAR(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	BEGIN TRANSACTION [CREATE_PRINT_JOBS]
		BEGIN TRY 

		Declare @printer_id Int,@print_job_id bigint

		select @printer_id=printer_id from printer WHERE serial_no=@serial_no

		INSERT INTO print_jobs
                         (printer_id, status_date, print_statuses_id, audit_user_id)
	VALUES        (@printer_id,SYSDATETIMEOFFSET(),@print_statuses_id,@audit_user_id)	

	set @print_job_id=SCOPE_IDENTITY()

	--insert customer_account_cards([customer_account_id],[card_id],[print_job_id]) VALUES(@customer_id,null,@print_job_id)

	set @ResultCode=0		
							
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_PRINT_JOBS]
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
