

/****** Object:  StoredProcedure [dbo].[sp_get_user]    Script Date: 2/16/2016 3:11:11 PM ******/
DROP PROCEDURE [dbo].[sp_get_user]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_user]    Script Date: 2/16/2016 3:11:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_get_user]
	(
		@username varchar(256),
		@user_status varchar(30)
	)
AS
	/* SET NOCOUNT ON */

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

    SELECT user_id, u.user_status_id, user_gender_id, username, first_name,
		   last_name, CONVERT(VARCHAR(max),DECRYPTBYKEY(password)), online, employee_id, last_login_date,
		   last_login_attempt, number_of_incorrect_logins, last_password_changed_date,
		   workstation 
	FROM [user] u,user_status us
	WHERE (DECRYPTBYKEY(username)=@username And us.user_status_text = @user_status)
	
CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;









GO

/****** Object:  StoredProcedure [dbo].[sp_get_user_by_user_id]    Script Date: 2/16/2016 3:11:44 PM ******/
DROP PROCEDURE [dbo].[sp_get_user_by_user_id]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_user_by_user_id]    Script Date: 2/16/2016 3:11:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 24 March 2014
-- Description:	Get a user based on the users system ID.
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_user_by_user_id] 
	-- Add the parameters for the stored procedure here
	@user_id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		--There should only be one result
		SELECT TOP 1 [user].[user_id]						
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) as 'username'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) as 'first_name'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
						,[user].[user_email]
						,[user].[user_status_id] 
						,[user].[ldap_setting_id]
						,[user].[language_id]
						,[user].[online]    
						,[user].[workstation],(case when last_password_changed_date is null then getdate() else last_password_changed_date end ) as 'last_password_changed_date'
						,[user].number_of_incorrect_logins,[user].last_login_attempt
		FROM [user]
		WHERE [user].[user_id] = @user_id		

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END










GO




/****** Object:  StoredProcedure [dbo].[sp_get_user_by_username]    Script Date: 2/16/2016 3:12:07 PM ******/
DROP PROCEDURE [dbo].[sp_get_user_by_username]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_user_by_username]    Script Date: 2/16/2016 3:12:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 14 March 2014
-- Description:	Return a user based on the username
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_user_by_username] 
	-- Add the parameters for the stored procedure here
	@username varchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		--There should only be one result
		SELECT TOP 1 [user].[user_id]						
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) as 'username'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) as 'first_name'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
						,[user].[user_email]
						,[user].[ldap_setting_id]
						,[user].[language_id]
						,[user].[user_status_id] 
						,[user].[online]    
						,[user].[workstation],(case when last_password_changed_date is null then getdate() else last_password_changed_date end ) as 'last_password_changed_date'
						,[user].number_of_incorrect_logins,[user].last_login_attempt

		FROM [user]
		WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) = @username		

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END










GO


/****** Object:  StoredProcedure [dbo].[sp_finalise_login_failed]    Script Date: 2/16/2016 3:13:00 PM ******/
DROP PROCEDURE [dbo].[sp_finalise_login_failed]
GO

/****** Object:  StoredProcedure [dbo].[sp_finalise_login_failed]    Script Date: 2/16/2016 3:13:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 11 March 2014
-- Description:	This stored procedure will fanalise the failed login by updating last login, workstation etc.
-- =============================================

CREATE PROCEDURE [dbo].[sp_finalise_login_failed] 
    @user_id bigint,    
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [FINALISE_LOGIN_FAILED_TRAN]
		BEGIN TRY
			
			DECLARE @incorrect_login_attempts int,
					 @maxInvalidPasswordAttempts int,
					 @PasswordAttemptLockoutDuration int,
					 @uer_status_id int
			DECLARE @last_login_attempt datetime

			--Fetch number of incorrect login attempts
			SELECT @incorrect_login_attempts = number_of_incorrect_logins,@uer_status_id=user_status_id,@last_login_attempt=last_login_attempt
			FROM [user]
			WHERE [user].[user_id] = @user_id


			SELECT @maxInvalidPasswordAttempts = maxInvalidPasswordAttempts,@PasswordAttemptLockoutDuration=PasswordAttemptLockoutDuration
			FROM [user_admin]

			--and @uer_status_id = 0 and
			if( DATEADD(hour,@PasswordAttemptLockoutDuration,@last_login_attempt) >= GETDATE())
			BEGIN
				if(@incorrect_login_attempts >= @maxInvalidPasswordAttempts)
					BEGIN
						UPDATE [user] 
							SET [user_status_id]=3 
							WHERE [user].[user_id] = @user_id
					END
				
				--Update last login and increment the incorrect login attempts
					UPDATE [user]
					SET last_login_attempt = GETDATE(),
						number_of_incorrect_logins = (@incorrect_login_attempts + 1)
					WHERE [user].[user_id] = @user_id

			END

			ELSE
			BEGIN
			UPDATE [user] 
							SET [user_status_id]=0 ,number_of_incorrect_logins=1,last_login_attempt = GETDATE()
							WHERE [user].[user_id] = @user_id
			END
			--log the audit record
			DECLARE @audit_description varchar(500)
			SELECT @audit_description = 'Login failed'			
			EXEC sp_insert_audit @user_id, 
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










GO



