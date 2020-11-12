CREATE PROCEDURE [dbo].[usp_create_user_group]
	@user_group_name varchar(50),
	@user_role_id int,
	@issuer_id int,
	@can_read bit,
	@can_create bit,
	@can_update bit,
	@mask_screen_pan bit = 1,
	@mask_report_pan bit = 1,
	@all_branch_access bit,
	@branch_list AS dbo.branch_id_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@new_user_group_id int OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Check for duplicate's
	IF (SELECT COUNT(*) FROM [user_group] WHERE ([user_group_name] = @user_group_name AND [issuer_id] = @issuer_id)) > 0
		BEGIN
			SET @new_user_group_id = 0
			SET @ResultCode = 215						
		END
	ELSE
		BEGIN

		BEGIN TRANSACTION [CREATE_USER_GROUP_TRAN]
		BEGIN TRY 

			DECLARE @RC int


			INSERT INTO [user_group]
			   ([user_role_id], [issuer_id], 
				[can_create], [can_read], [can_update], [can_delete],
				[mask_screen_pan], [mask_report_pan],
				[all_branch_access], [user_group_name])
			VALUES
			   (@user_role_id, @issuer_id, 
			    @can_create, @can_read, @can_update, 0, 
				@mask_screen_pan, @mask_report_pan,
				@all_branch_access, @user_group_name)

			SET @new_user_group_id = SCOPE_IDENTITY();

			--Link branches to user group
			EXECUTE @RC = [usp_insert_user_group_branches] @new_user_group_id, @branch_list, @audit_user_id, @audit_workstation

			--Insert audit
			DECLARE @branches varchar(max),
					@user_role_name varchar(50),
					@issuer_code varchar(10)

			IF (@all_branch_access = 0)
				BEGIN
					SELECT  @branches = STUFF(
										(SELECT ', ' +b.[branch_code] + ';' + cast(b.[branch_id] as varchar(max)) 
										 FROM user_groups_branches ug
											INNER JOIN [branch] b 
												ON ug.[branch_id] = b.[branch_id]
											WHERE ug.user_group_id = @new_user_group_id
											FOR XML PATH(''))
									   , 1
									   , 1
									   , '')
				END
			ELSE
				BEGIN
					SELECT  @branches = STUFF(
										(SELECT ', ' + [branch_code] + ';' + cast([branch_id] as varchar(max))
										 FROM [branch]
										 WHERE issuer_id = @issuer_id
										 FOR XML PATH(''))
									   , 1
									   , 1
									   , '')
				END

			SELECT @user_role_name = user_role
			FROM [user_roles]
			WHERE @user_role_id = @user_role_id

			SELECT @issuer_code = issuer_code
			FROM [issuer]
			WHERE issuer_id = @issuer_id

			DECLARE @audit_description varchar(max)
			SET @audit_description = 'Create: ' + COALESCE(@user_group_name, 'UNKNOWN') +
									 ', iss:' + COALESCE(@issuer_code, 'UNKNOWN') + ';' + COALESCE(CAST(@issuer_id as varchar(max)), 'UNKNOWN') + 
									 ', read: ' + COALESCE(CAST(@can_read as varchar(1)), 'UNKNOWN') + 
									 ', create: ' + COALESCE(CAST(@can_create as varchar(1)), 'UNKNOWN') +
									 ', update: ' + COALESCE(CAST(@can_update as varchar(1)), 'UNKNOWN') +
									 ', branches: ' + COALESCE(@branches, 'UNKNOWN')

			EXEC usp_insert_audit @audit_user_id, 
								 8,----UserGroupAdmin
								 NULL, 
								 @audit_workstation, 
								 @audit_description, 
								 @issuer_id, NULL, NULL, NULL

			SET @ResultCode = 0
			COMMIT TRANSACTION [CREATE_USER_GROUP_TRAN]

		END TRY
			BEGIN CATCH
			ROLLBACK TRANSACTION [CREATE_USER_GROUP_TRAN]
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