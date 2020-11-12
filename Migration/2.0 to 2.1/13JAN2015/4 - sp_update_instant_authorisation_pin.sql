USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_instant_authorisation_pin]    Script Date: 2015/01/14 04:26:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20150108
-- Description:	User authorisation pin number
-- =============================================
ALTER PROCEDURE [dbo].[sp_update_instant_authorisation_pin]
	@user_id bigint,
	@authorisation_pin_number varchar(100),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	 BEGIN TRANSACTION [UPDATE_USER_AUTH_PIN_TRAN]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate
			
			UPDATE dbo.[user] 
			SET 
				[instant_authorisation_pin] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar, @authorisation_pin_number)),
				[last_authorisation_pin_changed_date] = GETDATE()
			WHERE [user].[user_id] = @user_id

				--log the audit record
				DECLARE @audit_description varchar(max),
				        @username varchar(100)

				SELECT  @username = CONVERT(VARCHAR,DECRYPTBYKEY([username]))
				FROM [user]
				WHERE [user_id] = @user_id

				IF (@user_id = @audit_user_id)
					BEGIN
						SELECT @audit_description = 'Change Authoriation Pin: ' + @username
					END
				ELSE
					BEGIN
						SELECT @audit_description = 'Change Authoriation Pin: ' + @username
					END		

				
				EXEC sp_insert_audit @audit_user_id, 
									 7,---UserAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

			COMMIT TRANSACTION [UPDATE_USER_AUTH_PIN_TRAN]	
			SET @ResultCode = 0	
				END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_USER_AUTH_PIN_TRAN]
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
