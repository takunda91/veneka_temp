Create PROCEDURE [dbo].[usp_insert_user] 
	@user_status_id int,
    @username varchar(50),
    @first_name varchar(50),
    @last_name varchar(50),
    @password varchar(50)=null,
	@user_email varchar(100),
    @online bit = 0,
    @employee_id varchar(50),
    @last_login_date datetimeoffset = null,
    @last_login_attempt datetimeoffset = null,
    @number_of_incorrect_logins int = 0,
    @last_password_changed_date datetimeoffset = null,
    @workstation nchar(50) = '',
	@user_group_list AS user_group_id_array READONLY,	 
	@audit_user_id bigint,
	@audit_workstation varchar(100),	
	@authentication_configuration_id int=null,
	@time_zone_id varchar(500),
	@time_zone_utcoffset varchar(50),
	@language_id int = null,
	@user_id bigint=null,
	@new_user_id bigint OUTPUT,
	@ResultCode int OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [INSERT_USER_TRAN]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			DECLARE @objid int
			SET @objid = object_id('user')

			--Check for duplicate username
			-- =============================================
			DECLARE @dup_check int
			Select @dup_check= tot 
			from 	(SELECT  COUNT(*) as 'tot'
			FROM [user]
			WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) = @username
					
			UNION

			SELECT  COUNT(*) as 'tot'
			FROM [user_pending]
			WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([user_pending].[username])) = @username
			) as t
			if(@user_id =0)
			set @user_id=null
			if(@password is null)
			set @password='default'

			Declare @checkoutcards int , @group_count int

			--- finding usergroups which are added new when editing
select @group_count= count(*) from 
(  select  [user_group_id]  from  @user_group_list 
except
select [users_to_users_groups].user_group_id												 
													from [users_to_users_groups]
											where 	[users_to_users_groups].user_id=@user_id) as t	
	
	--- if any new usergroups added check old usergroup branches have any cards checked out for him.
	IF(@group_count	>0)
	BEGIN

	select @checkoutcards=count(cards.card_id) from cards
						INNER JOIN [branch_card_status_current]
						ON [cards].card_id = [branch_card_status_current].card_id
						 where  [cards].branch_id  in (select [user_groups_branches].branch_id
													FROM [user_groups_branches] 
													inner join [users_to_users_groups] on  [user_groups_branches].user_group_id=[users_to_users_groups].user_group_id
													where 	[users_to_users_groups].user_id=@user_id)
							and  [branch_card_status_current].branch_card_statuses_id =1
							and   [cards].card_issue_method_id =1
		END
		ELSE
		  set @checkoutcards=0
	

			--Check for duplicate's
	IF (@checkoutcards>0)
		BEGIN
			SET @new_user_id = 0;
			SET @ResultCode = 809						
		END
	ELSE IF @dup_check > 0 and @user_id is null
				BEGIN
					SET @new_user_id = 0;
					SELECT @ResultCode = 69							
				END
			ELSE
			BEGIN
					INSERT INTO [user_pending]
						([user_status_id],[user_gender_id],[username],[first_name],[last_name],[password],[user_email],[online]
						 ,[employee_id],[last_login_date],[last_login_attempt],[number_of_incorrect_logins]
						  ,[last_password_changed_date],[workstation], [language_id], [username_index],authentication_configuration_id,time_zone_id,time_zone_utcoffset,user_id,[useradmin_user_id],record_datetime)
					VALUES
						   (@user_status_id,
							1,
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@username)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@first_name)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@last_name)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@password)),
							@user_email,
							@online,
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@employee_id)),
							@last_login_date,
							@last_login_attempt,
							@number_of_incorrect_logins,
							@last_password_changed_date,
							@workstation,
							@language_id,
							[dbo].[MAC] (@username, @objid),@authentication_configuration_id,@time_zone_id,@time_zone_utcoffset,@user_id,@audit_user_id,SYSDATETIMEOFFSET())

					SET @new_user_id = SCOPE_IDENTITY();
			

				--Link user to user groups
				INSERT INTO [user_to_user_group_pending]
							([pending_user_id], [user_group_id])
				SELECT @new_user_id, ugl.user_group_id
				FROM @user_group_list ugl

				
				
				--log the audit record
				DECLARE @audit_description varchar(max), 
				        @groupname varchar(max),
						@user_status_name varchar(50)

				SELECT @user_status_name = user_status_text
				FROM [user_status]
				WHERE user_status_id = @user_status_id				

				SELECT @groupname = STUFF(
									(SELECT ', ' + user_group_name + 
											';' + CAST([user_group].[user_group_id] as varchar(max)) +
											';' + CAST([user_group].issuer_id as varchar(max))
									 FROM [user_group]
											INNER JOIN [users_to_users_groups]
												ON [user_group].user_group_id = [users_to_users_groups].user_group_id
										WHERE [users_to_users_groups].[user_id] = @new_user_id
										FOR XML PATH(''))
								   , 1
								   , 1
								   , '')

				--SELECT @groupname = SUBSTRING(
				--	(
				--	select ';'+ u.[user_group_name] + '-' +cast(u.[user_group_id] as varchar(500))+'- issu:'
				--	+cast(u.issuer_id as varchar(500)) from [dbo].[user_group] u
				--	inner join [users_to_users_groups] ug on u.[user_group_id]=ug.[user_group_id]
				--	where ug.[user_id]=@new_user_id
				--	FOR XML PATH('')),2,2000)
				--	if( @groupname is null)
				--	set @groupname=''
				
				--set @audit_description = 'Created: ' +@username + ', issu:'	+CAST(@login_issuer_id as varchar(100))+', groups:'+@groupname
				set @audit_description = 'Created: id: ' + CAST(@new_user_id as varchar(max)) + 
										 ', user: ' + @username + 
										 ', status: ' + COALESCE(@user_status_name, 'UNKNOWN') +
										 ', authentication config: ' + CAST(COALESCE(@authentication_configuration_id, 0) as varchar(max)) + 
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

			COMMIT TRANSACTION [INSERT_USER_TRAN]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_USER_TRAN]
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
