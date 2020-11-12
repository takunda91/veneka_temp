-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_user_audit_approve] 
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

    BEGIN TRANSACTION [UPDATE_USER_TRAN]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			--Check for duplicate username
			Declare @user_id as bigint
			DECLARE @dup_check int, @new_user_id bigint

			
			SELECT @user_id=[user_id] FROM [user_pending] WHERE pending_user_id=@pending_user_id

			
			IF(@user_id IS NULL)
			BEGIN
			INSERT INTO [user]
						([user_status_id],[user_gender_id],[username],[first_name],[last_name],[password],[user_email],[online]
						 ,[employee_id],[last_login_date],[last_login_attempt],[number_of_incorrect_logins]
						  ,[last_password_changed_date],[workstation], [language_id], [username_index],authentication_configuration_id,time_zone_id,time_zone_utcoffset,approved_datetime,approved_user_id,[useradmin_user_id],record_datetime)
					select u.user_status_id,
							1 as user_gender_id,
							u.username,
							u.first_name,
							u.last_name,
							u.password,
							u.user_email,
							u.online,
							u.employee_id,
							u.last_login_date,
							u.last_login_attempt,
							u.number_of_incorrect_logins,
							u.last_password_changed_date,
							u.workstation,
							u.language_id,
							u.username_index,u.authentication_configuration_id,u.time_zone_id,u.time_zone_utcoffset,SYSDATETIMEOFFSET(),@audit_user_id,u.useradmin_user_id,u.record_datetime					
				
				from [user_pending] as u				
				where u.[pending_user_id] = @pending_user_id
				
					SET @user_id = SCOPE_IDENTITY();

				
			END 
			BEGIN
				UPDATE [user]
				SET	[user_status_id] = COALESCE(u.user_status_id, [user].[user_status_id])
					,[username] = u.[username]
					,[first_name] = u.[first_name]
					,[last_name] = u.last_name
					,[user_email] = u.user_email
					,[online] = COALESCE(u.online, [user].[online])
					,[employee_id] = u.employee_id
					,[last_login_date] = COALESCE(u.last_login_date, [user].[last_login_date])
					,[last_login_attempt] = COALESCE(u.last_login_attempt, [user].[last_login_attempt])
					,[number_of_incorrect_logins] = COALESCE(u.number_of_incorrect_logins, [user].[number_of_incorrect_logins])
					,[last_password_changed_date] = COALESCE(u.last_password_changed_date, [user].[last_password_changed_date])
					,[workstation] = COALESCE(u.workstation, [user].[workstation])
					,authentication_configuration_id = u.authentication_configuration_id
					,[language_id] = u.language_id
					,time_zone_id =u.time_zone_id
					,time_zone_utcoffset=u.time_zone_utcoffset
					,approved_datetime=SYSDATETIMEOFFSET()
					,approved_user_id= @audit_user_id
					,useradmin_user_id=u.useradmin_user_id
					,record_datetime=u.record_datetime
				OUTPUT DELETED.* INTO [user_audit]  				
				from [user]
				inner join [user_pending] as u on [user].[user_id]=u.[user_id]
				where u.pending_user_id = @pending_user_id

					

				END

				
				

				--Remove links to user groups
				DELETE FROM [users_to_users_groups]
				OUTPUT Deleted.* INTO user_to_user_group_audit
				WHERE [user_id] = @user_id

				INSERT INTO [users_to_users_groups]
				select @user_id, [user_group_id]
				from [user_to_user_group_pending]
				--OUTPUT Deleted.* INTO user_to_user_group_audit
				where pending_user_id=@pending_user_id


				DELETE FROM dbo.[user_to_user_group_pending]  
				--OUTPUT DELETED.* INTO user_to_user_group_audit  
				WHERE [pending_user_id] = @pending_user_id
				
					DELETE FROM dbo.[user_pending]  
				WHERE [pending_user_id] = @pending_user_id
			
				

				--log the audit record
					--log the audit record
				DECLARE @audit_description varchar(max), 
				        @groupname varchar(max),
						@username varchar(max),
						@auth_configuration_id varchar(max),
						@user_status_name varchar(50)

				SELECT @user_status_name = user_status_text,@username=CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username]))
				,@auth_configuration_id=[user].authentication_configuration_id
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
										 ', user: ' + @username + 
										 ', status: ' + COALESCE(@user_status_name, 'UNKNOWN') +
										 ', login ldap: ' + CAST(COALESCE(@auth_configuration_id, 0) as varchar(max)) + 
										 ', groups: ' + COALESCE(@groupname, 'none-selected')

				EXEC usp_insert_audit @audit_user_id, 
									 7,---UserAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL

				SELECT @ResultCode = 0		
						
			

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
END
