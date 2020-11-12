-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Update user logout status
-- =============================================
CREATE PROCEDURE [dbo].[usp_finalise_logout] 
	@user_id BIGINT,
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

        BEGIN TRANSACTION [FINALISE_LOGOUT_TRAN]
			BEGIN TRY 			

				DECLARE @workstation varchar(100),
						@username varchar(100)

				OPEN SYMMETRIC KEY Indigo_Symmetric_Key
					DECRYPTION BY CERTIFICATE Indigo_Certificate;

					SELECT @workstation = workstation, @username = CONVERT(VARCHAR,DECRYPTBYKEY(username))
					FROM [user]
					WHERE [user_id] = @user_id

				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

				--Update workstation and last login
				UPDATE [user]
				SET workstation = '',
					[online] = 0
				WHERE [user_id] = @user_id

				--log the audit record
				IF (@user_id = @audit_user_id)
					BEGIN
						EXEC usp_insert_audit @audit_user_id, 
									 6,--Logon
									 NULL, 
									 @audit_workstation,
									 'Logged off', 									  
									 NULL, NULL, NULL, NULL	
					END
				ELSE
					BEGIN
						DECLARE @audit_description varchar(500)

						SET @audit_description = 'Resetting user login status: ' + @username + ',' + @workstation

						EXEC usp_insert_audit @audit_user_id, 
									 7,--UserAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL	
					END					

			COMMIT TRANSACTION [FINALISE_LOGOUT_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [FINALISE_LOGOUT_TRAN]
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