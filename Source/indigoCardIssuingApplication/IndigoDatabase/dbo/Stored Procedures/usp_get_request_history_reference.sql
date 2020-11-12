CREATE  PROCEDURE [dbo].[usp_get_request_history_reference]
	@request_id INT
	, @languageId INT = NULL
	, @audit_user_id BIGINT
	, @audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
		BEGIN TRANSACTION [GET_REQUEST_HIST_STATUS]
			BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			SELECT
				db.print_batch_id
				, print_batch_reference 
				, CAST(SWITCHOFFSET(db.date_created,@UserTimezone) AS datetime) as 'date_created'
			FROM
				print_batch db
				LEFT JOIN print_batch_requests dbc ON db.print_batch_id = dbc.print_batch_id
				LEFT JOIN hybrid_requests c ON c.request_id = dbc.request_id
			WHERE
				dbc.request_id = @request_id

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			COMMIT TRANSACTION [GET_REQUEST_HIST_STATUS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_REQUEST_HIST_STATUS]
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

