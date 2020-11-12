CREATE PROCEDURE [dbo].[usp_user_audit_reject] 
	@pending_user_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(max),
	@ResultCode int OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [REJECT_USER_TRAN]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

				DELETE FROM dbo.[user_to_user_group_pending]  
				--OUTPUT DELETED.* INTO user_to_user_group_audit  
				WHERE [pending_user_id] = @pending_user_id
				
				
			
				

				--log the audit record
				DECLARE @audit_description varchar(max), 
				        @groupname varchar(max),
						@username varchar(max),
						@auth_configuration_id varchar(max),
						@user_status_name varchar(50)

				SELECT @user_status_name = user_status_text,@username=CONVERT(VARCHAR(max),DECRYPTBYKEY([user_pending].[username])),@auth_configuration_id=[user_pending].authentication_configuration_id
				FROM [user_status] INNER JOIN [user_pending]
						ON [user_status].user_status_id = [user_pending].user_status_id
				WHERE [user_pending].[pending_user_id] = @pending_user_id				

					DELETE FROM dbo.[user_pending]  
				WHERE [pending_user_id] = @pending_user_id

				
				--set @audit_description = 'Created: ' +@username + ', issu:'	+CAST(@login_issuer_id as varchar(100))+', groups:'+@groupname
				set @audit_description = 'rejected: id: ' + CAST(@pending_user_id as varchar(max)) + 
										 ', user: ' + @username + 
										 ', status: ' + COALESCE(@user_status_name, 'UNKNOWN') +
										 ', login ldap: ' + CAST(COALESCE(@auth_configuration_id, 0) as varchar(max)) 

				EXEC usp_insert_audit @audit_user_id, 
									 7,---UserAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL

				SELECT @ResultCode = 0		
						
			

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
			COMMIT TRANSACTION [REJECT_USER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [REJECT_USER_TRAN]
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
END
