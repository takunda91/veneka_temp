-- =============================================
-- Author:		Richard Brenchley
-- Create date: 3 March 2014
-- Description:	This stored procedure will fanalise the login by updating last login, workstation etc.
-- =============================================
CREATE PROCEDURE [dbo].[usp_finalise_login] 
	@user_id bigint,
    @workstation nvarchar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate;

		BEGIN TRANSACTION [FINALISE_LOGIN_TRAN]
		BEGIN TRY

			--Update workstation and last login
			UPDATE [user]
			SET workstation = @workstation,
				[online] = 1,
				last_login_attempt = SYSDATETIMEOFFSET(),
				last_login_date = SYSDATETIMEOFFSET(),
				number_of_incorrect_logins = 0
			WHERE [user_id] = @user_id

			--log the audit record
			DECLARE @audit_description varchar(500)
			SELECT @audit_description = 'Login success'		
			EXEC usp_insert_audit @user_id, 
									6,---Logon
									NULL, 
									@workstation, 
									@audit_description, 
									NULL, NULL, NULL, NULL

		COMMIT TRANSACTION [FINALISE_LOGIN_TRAN]

		END TRY
		BEGIN CATCH
			ROLLBACK TRANSACTION [FINALISE_LOGIN_TRAN]
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

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

	RETURN

END