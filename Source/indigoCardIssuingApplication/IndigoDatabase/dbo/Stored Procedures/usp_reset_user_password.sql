-- =============================================
-- Author:		Richard Brenchley
-- Create date: 26 March 2014
-- Description:	Resets a users password
-- =============================================
CREATE PROCEDURE [dbo].[usp_reset_user_password] 
	-- Add the parameters for the stored procedure here
	@password varchar(100),
	@user_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [RESET_USER_PASSWORD_TRAN]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate
				--Clear out the password history table with passwords older than 3 months
				--DELETE FROM [user_password_history]
				--WHERE [date_changed] < DATEADD(MONTH, -3, GETDATE())

			DECLARE @PreviousPasswordsCount int
			DECLARE @query nvarchar(max)
			DECLARE @subquery nvarchar(max)

			
			SELECT @PreviousPasswordsCount = PreviousPasswordsCount
			FROM [user_admin]
			--- select all other records other than @PreviousPasswordsCount
			set @subquery=' select  user_id from  user_password_history where  user_id not in (select top '+ cast( @PreviousPasswordsCount as varchar(10))+' user_id from [user_password_history] where [user_id] = '+cast( @user_id	as varchar(10))+' order by [date_changed] asc)'
			
			set @query=' DELETE FROM [user_password_history] where user_id  in ('+@subquery+')'
			
			exec (@query)
				
				--Move the current password into the history table
				INSERT INTO [user_password_history] ([user_id], [password_history], [date_changed])
				SELECT [user].[user_id], [user].[password], SYSDATETIMEOFFSET()
				FROM [user]
				WHERE [user].[user_id] = @user_id				
				
				--Update current password with new password
				UPDATE [user]
				SET	[password] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@password)),
					[last_password_changed_date] = SYSDATETIMEOFFSET(),
					[number_of_incorrect_logins] = 0
				WHERE [user].[user_id] = @user_id
				

				--log the audit record
				DECLARE @audit_description varchar(max),
				        @username varchar(100)

				SELECT  @username = CONVERT(VARCHAR(max),DECRYPTBYKEY([username]))
				FROM [user]
				WHERE [user_id] = @user_id

				IF (@user_id = @audit_user_id)
					BEGIN
						SELECT @audit_description = 'Change Password: ' + @username
					END
				ELSE
					BEGIN
						SELECT @audit_description = 'Reset Password: ' + @username
					END		

				
				EXEC usp_insert_audit @audit_user_id, 
									 7,---UserAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

			COMMIT TRANSACTION [RESET_USER_PASSWORD_TRAN]		
			
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [RESET_USER_PASSWORD_TRAN]
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