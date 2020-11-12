CREATE PROCEDURE [dbo].[usp_get_request_history_status]
	@request_id INT
	, @languageId INT = NULL
	, @audit_user_id BIGINT
	, @audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @UserTimezone nvarchar(50) = [dbo].[GetUserTimeZone](@audit_user_id);

	BEGIN TRANSACTION [GET_REQUEST_HIST]
		BEGIN TRY 

		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate;

			SELECT
				[hybrid_request_statuses]
				, CAST(SWITCHOFFSET(status_date, @UserTimezone) as datetime) as 'status_date'
				, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(u.username)) AS username
			FROM 
				hybrid_requests c
				LEFT JOIN hybrid_request_status cs ON c.request_id = cs.request_id				
				LEFT JOIN hybrid_request_statuses bcs ON cs.hybrid_request_statuses_id = bcs.hybrid_request_statuses_id
				LEFT JOIN [user] u ON u.[user_id] = cs.[user_id]
			WHERE
				c.request_id = @request_id
			UNION ALL
			SELECT
				[hybrid_request_statuses]
				, CAST(SWITCHOFFSET(status_date, @UserTimezone) as datetime) as 'status_date'
				, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(u.username)) AS username
			FROM 
				hybrid_requests c				
				LEFT JOIN hybrid_request_status_audit csa ON csa.request_id = c.request_id
				LEFT JOIN hybrid_request_statuses bcs ON csa.hybrid_request_statuses_id = bcs.hybrid_request_statuses_id
				LEFT JOIN [user] u ON u.[user_id] = csa.[user_id]
			WHERE
				c.request_id = @request_id
			ORDER BY status_date

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			COMMIT TRANSACTION [GET_REQUEST_HIST]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_REQUEST_HIST]
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
