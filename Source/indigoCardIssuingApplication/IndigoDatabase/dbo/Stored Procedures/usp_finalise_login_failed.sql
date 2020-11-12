
-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 11 March 2014
-- Description:	This stored procedure will fanalise the failed login by updating last login, workstation etc.
-- =============================================

CREATE PROCEDURE [dbo].[usp_finalise_login_failed] 
    @user_id bigint,    
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [FINALISE_LOGIN_FAILED_TRAN]
		BEGIN TRY
			
			

			--Fetch number of incorrect login attempts
			--SELECT @incorrect_login_attempts = number_of_incorrect_logins, @uer_status_id = user_status_id, 
			--		@last_login_attempt = last_login_attempt
			--FROM [user]
			--WHERE [user].[user_id] = @user_id


			

			--Check how long the user is locked out for
			--IF( DATEADD(hour, @PasswordAttemptLockoutDuration, @last_login_attempt) >= GETDATE())
			--BEGIN
			--	IF(@incorrect_login_attempts >= @maxInvalidPasswordAttempts)
			--	BEGIN
			--		UPDATE [user] 
			--		SET [user_status_id] = 3 
			--		WHERE [user].[user_id] = @user_id
			--	END
				
			--	--Update last login and increment the incorrect login attempts
			--	UPDATE [user]
			--	SET last_login_attempt = GETDATE(),
			--		number_of_incorrect_logins = (@incorrect_login_attempts + 1)
			--	WHERE [user].[user_id] = @user_id
			--END
			--ELSE
			--BEGIN
			--	UPDATE [user] 
			--	SET [user_status_id] = 0
			--		,number_of_incorrect_logins = 1
			--		,last_login_attempt = GETDATE()
			--	WHERE [user].[user_id] = @user_id
			--END



			DECLARE @maxInvalidPasswordAttempts int
					 
			SELECT @maxInvalidPasswordAttempts = maxInvalidPasswordAttempts
			FROM [user_admin]

			--Increment number of login tried, lockout if limit reached
			UPDATE [user] 
			SET [number_of_incorrect_logins] = [user].[number_of_incorrect_logins] + 1
			    , [last_login_attempt] = SYSDATETIMEOFFSET()
				, [user_status_id] = CASE WHEN [user].[number_of_incorrect_logins] + 1 >= @maxInvalidPasswordAttempts 
										THEN 3 
										ELSE [user_status_id] END
			WHERE [user].[user_id] = @user_id


			--log the audit record
			DECLARE @audit_description varchar(500)
			SELECT @audit_description = 'Login failed'			
			EXEC usp_insert_audit @user_id, 
									6,---Logon
									NULL, 
									@audit_workstation, 
									@audit_description, 
									NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [FINALISE_LOGIN_FAILED_TRAN]

		END TRY
		BEGIN CATCH
		  ROLLBACK TRANSACTION [FINALISE_LOGIN_FAILED_TRAN]
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
	RETURN
END