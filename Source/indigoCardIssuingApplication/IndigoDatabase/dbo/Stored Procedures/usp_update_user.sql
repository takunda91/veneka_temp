-- =============================================
-- Author:		Richard Brenchley
-- Create date: 24 March 2014
-- Description:	Updates a user
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_user] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@user_status_id int = null,
    @username varchar(50),
    @first_name varchar(50),
    @last_name varchar(50),
	@user_email varchar(100),
    @online bit = null,
    @employee_id varchar(50),
    @last_login_date datetimeoffset = null,
    @last_login_attempt datetimeoffset = null,
    @number_of_incorrect_logins int = null,
    @last_password_changed_date datetimeoffset = null,
    @workstation nchar(50)  = null,
	@user_group_list AS user_group_id_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@time_zone_id varchar(500),
	@time_zone_utcoffset varchar(50),
	@auth_configuration_id int,
	@language_id int = null,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [UPDATE_USER_TRAN]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			--Check for duplicate username
			DECLARE @dup_check int
			Select @dup_check= tot 
			from 	(SELECT  COUNT(*) as 'tot'
			FROM [user]
			WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) = @username
					AND [user_id] != @user_id
			UNION

			SELECT  COUNT(*) as 'tot'
			FROM [user_pending]
			WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([user_pending].[username])) = @username
			AND [user_id] != @user_id
			) as t

			

			IF @dup_check > 0
				BEGIN
					SELECT @ResultCode = 69							
				END
			ELSE
			BEGIN
				UPDATE [user]
				SET	[user_status_id] = COALESCE(@user_status_id, [user].[user_status_id])
					,[username] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@username))
					,[first_name] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@first_name))
					,[last_name] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@last_name))
					,[user_email] = @user_email
					,[online] = COALESCE(@online, [user].[online])
					,[employee_id] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@employee_id))
					,[last_login_date] = COALESCE(@last_login_date, [user].[last_login_date])
					,[last_login_attempt] = COALESCE(@last_login_attempt, [user].[last_login_attempt])
					,[number_of_incorrect_logins] = COALESCE(@number_of_incorrect_logins, [user].[number_of_incorrect_logins])
					,[last_password_changed_date] = COALESCE(@last_password_changed_date, [user].[last_password_changed_date])
					,[workstation] = COALESCE(@workstation, [user].[workstation])
					,[language_id] = @language_id
					--,[external_interface_id]=@external_interface_id
					,time_zone_id =@time_zone_id
					,time_zone_utcoffset=@time_zone_utcoffset
					,authentication_configuration_id=@auth_configuration_id
				WHERE [user].[user_id] = @user_id

				

				--Remove links to user groups
				DELETE FROM [users_to_users_groups]
				WHERE [user_id] = @user_id

				--Link user to user groups
				INSERT INTO [users_to_users_groups]
							([user_id], [user_group_id])
				SELECT @user_id, ugl.user_group_id
				FROM @user_group_list ugl

				--log the audit record
				DECLARE @audit_description varchar(max), 
				        @groupname varchar(max),
						@user_status_name varchar(50)

				SELECT @user_status_name = user_status_text
				FROM [user_status] INNER JOIN [user]
						ON [user_status].user_status_id = [user].user_status_id
				WHERE [user].[user_id] = @user_id				

				SELECT @groupname = STUFF(
									(SELECT ', ' + user_group_name + 
											';' + CAST([user_group].[user_group_id] as varchar(max)) +
											';' + CAST([user_group].issuer_id as varchar(max))
									 FROM [user_group]
											INNER JOIN [users_to_users_groups]
												ON [user_group].user_group_id = [users_to_users_groups].user_group_id
										WHERE [users_to_users_groups].[user_id] = @user_id
										FOR XML PATH(''))
								   , 1
								   , 1
								   , '')
				
				--set @audit_description = 'Created: ' +@username + ', issu:'	+CAST(@login_issuer_id as varchar(100))+', groups:'+@groupname
				set @audit_description = 'Update: id: ' + CAST(@user_id as varchar(max)) + 
										 --', user: ' + @username + 
										 ', status: ' + COALESCE(@user_status_name, 'UNKNOWN') +
										 --', login ldap: ' + CAST(COALESCE(@connection_parameter_id, 0) as varchar(max)) + 
										 ', groups: ' + COALESCE(@groupname, 'none-selected')

				EXEC usp_insert_audit @audit_user_id, 
									 7,---UserAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL

				SELECT @ResultCode = 0		
						
			END

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
			COMMIT TRANSACTION [UPDATE_USER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_USER_TRAN]
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
GO