

/****** Object:  StoredProcedure [dbo].[sp_get_user_by_user_id]    Script Date: 2/22/2016 8:42:28 AM ******/
DROP PROCEDURE [dbo].[sp_get_user_by_user_id]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_user_by_user_id]    Script Date: 2/22/2016 8:42:28 AM ******/
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
						,[user].[connection_parameter_id]
						,[user].[language_id]
						,[user].[online]    
						,[user].[workstation],(case when last_password_changed_date is null then getdate() else last_password_changed_date end ) as 'last_password_changed_date'
						,[user].number_of_incorrect_logins,[user].last_login_attempt,[user].external_interface_id
		FROM [user]
		WHERE [user].[user_id] = @user_id		

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO




/****** Object:  StoredProcedure [dbo].[sp_get_user_by_username]    Script Date: 2/22/2016 8:43:12 AM ******/
DROP PROCEDURE [dbo].[sp_get_user_by_username]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_user_by_username]    Script Date: 2/22/2016 8:43:12 AM ******/
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
						,[user].[connection_parameter_id]
						,[user].[language_id]
						,[user].[user_status_id] 
						,[user].[online]    
						,[user].[workstation],(case when last_password_changed_date is null then getdate() else last_password_changed_date end ) as 'last_password_changed_date'
						,[user].number_of_incorrect_logins,[user].last_login_attempt,[user].external_interface_id

		FROM [user]
		WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) = @username		

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END


GO



/****** Object:  StoredProcedure [dbo].[sp_get_user_for_login]    Script Date: 2/22/2016 8:43:37 AM ******/
DROP PROCEDURE [dbo].[sp_get_user_for_login]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_user_for_login]    Script Date: 2/22/2016 8:43:37 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 20 March 2014
-- Description:	Gets a user for login purposes.
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_user_for_login] 
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
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) as 'clr_username'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[password])) as 'clr_password'
						,[user].[online]    
						,[user].[workstation]
						,[user].last_login_attempt
						,[user].last_password_changed_date
						,[user].number_of_incorrect_logins
						,[user].language_id
						,[user].[connection_parameter_id]	
						,[user].user_status_id
						,[connection_parameters].address as domain_hostname_or_ip
						,[connection_parameters].domain_name
						,[connection_parameters].[path] as domain_path
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([connection_parameters].username)) as domain_username	
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([connection_parameters].[password])) as domain_password,
						[connection_parameters].connection_parameter_type_id,
						[connection_parameters].is_external_auth,[user].external_interface_id,auth_type,protocol,[path]								
		FROM [user] 
			LEFT OUTER JOIN [connection_parameters]
				ON [user].[connection_parameter_id] = [connection_parameters].[connection_parameter_id]
		WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) = @username

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END










GO




/****** Object:  StoredProcedure [dbo].[sp_get_users_by_branch]    Script Date: 2/22/2016 8:45:49 AM ******/
DROP PROCEDURE [dbo].[sp_get_users_by_branch]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_users_by_branch]    Script Date: 2/22/2016 8:45:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 13 March 2014
-- Description:	Get all ussers linked to the specified branch.
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_users_by_branch] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@branch_id int = null,
	@branch_status_id int = null,
	@user_status_id int = null,
	@user_role_id int = null,
	@username varchar(100) = null,
	@first_name varchar(100) = null,
	@last_name varchar(100) = null,
	@languageId int =null,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @username = '' OR @username IS NULL
		SET @username = NULL
	ELSE
		SET @username = '%' + @username + '%'

	IF @first_name = '' OR @first_name IS NULL
		SET @first_name = NULL
	ELSE
		SET @first_name = '%' + @first_name + '%'

	IF @last_name = '' OR @last_name IS NULL
		SET @last_name = NULL
	ELSE
		SET @last_name = '%' + @last_name + '%'

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY username ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
		SELECT DISTINCT [user].[user_id]						
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) as 'username'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) as 'first_name'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
						,usl.language_text as [user_status_text] 
						,[user].[online]    
						,[user].[workstation],case when ([user].connection_parameter_id is null) then 'Indigo' else 'LDAP' end as 'AUTHENTICATION_TYPE'
			FROM [branch] INNER JOIN [user_roles_branch]
					ON [branch].branch_id = [user_roles_branch].branch_id
				INNER JOIN [user]
					ON [user].[user_id] = [user_roles_branch].[user_id]
				INNER JOIN user_status
					ON user_status.user_status_id = [user].user_status_id
				INNER JOIN  [dbo].[user_status_language] usl 
					ON usl.user_status_id=user_status.user_status_id 
			WHERE [user_roles_branch].issuer_id = COALESCE(@issuer_id, [user_roles_branch].issuer_id)
				AND (@username IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) LIKE @username) 
				AND (@first_name IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) LIKE @first_name) 
				AND (@last_name IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) LIKE @last_name) 
				AND [branch].branch_id = COALESCE(@branch_id, branch.branch_id)				
				AND [branch].branch_status_id = COALESCE(@branch_status_id, branch.branch_status_id)
				AND [user].user_status_id = COALESCE(@user_status_id, [user].user_status_id)
				AND [user_roles_branch].user_role_id = COALESCE(@user_role_id, [user_roles_branch].user_role_id)
				AND usl.language_id=@languageId
		) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY username ASC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END









GO




/****** Object:  StoredProcedure [dbo].[sp_get_users_by_branch_admin]    Script Date: 2/22/2016 8:46:32 AM ******/
DROP PROCEDURE [dbo].[sp_get_users_by_branch_admin]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_users_by_branch_admin]    Script Date: 2/22/2016 8:46:32 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 13 March 2014
-- Description:	Get all ussers linked to the specified branch for user admin
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_users_by_branch_admin] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@branch_id int = null,
	@branch_status_id int = null,
	@user_status_id int = null,
	@user_role_id int = null,
	@username varchar(100) = null,
	@first_name varchar(100) = null,
	@last_name varchar(100) = null,
	@languageId int =null,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @username = '' OR @username IS NULL
		SET @username = NULL
	ELSE
		SET @username = '%' + @username + '%'

	IF @first_name = '' OR @first_name IS NULL
		SET @first_name = NULL
	ELSE
		SET @first_name = '%' + @first_name + '%'

	IF @last_name = '' OR @last_name IS NULL
		SET @last_name = NULL
	ELSE
		SET @last_name = '%' + @last_name + '%'

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY username ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
		SELECT DISTINCT [user].[user_id]						
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) as 'username'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) as 'first_name'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
						,usl.language_text as  [user_status_text] 
						,[user].[online]    
						,[user].[workstation],case when ([user].[connection_parameter_id] is null) then 'Indigo' else 'LDAP' end as 'AUTHENTICATION_TYPE'
			FROM [branch] INNER JOIN [user_group_branch_ex_ent]
					ON [branch].branch_id = [user_group_branch_ex_ent].branch_id
				INNER JOIN [user]
					ON [user].[user_id] = [user_group_branch_ex_ent].[user_id]
				INNER JOIN user_status
					ON user_status.user_status_id = [user].user_status_id
				INNER JOIN  [dbo].[user_status_language] usl 
					ON usl.user_status_id=user_status.user_status_id 
			WHERE [user_group_branch_ex_ent].issuer_id = COALESCE(@issuer_id, [user_group_branch_ex_ent].issuer_id)
				AND (@username IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) LIKE @username) 
				AND (@first_name IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) LIKE @first_name) 
				AND (@last_name IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) LIKE @last_name) 
				AND [branch].branch_id = COALESCE(@branch_id, branch.branch_id)				
				AND [branch].branch_status_id = COALESCE(@branch_status_id, branch.branch_status_id)
				AND [user].user_status_id = COALESCE(@user_status_id, [user].user_status_id)
				AND [user_group_branch_ex_ent].user_role_id = COALESCE(@user_role_id, [user_group_branch_ex_ent].user_role_id)
				AND usl.language_id=@languageId and [branch].branch_status_id=0
		) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY username ASC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END









GO



/****** Object:  StoredProcedure [dbo].[sp_update_user]    Script Date: 2/22/2016 8:49:24 AM ******/
DROP PROCEDURE [dbo].[sp_update_user]
GO

/****** Object:  StoredProcedure [dbo].[sp_update_user]    Script Date: 2/22/2016 8:49:24 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 24 March 2014
-- Description:	Updates a user
-- =============================================
CREATE PROCEDURE [dbo].[sp_update_user] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@user_status_id int = null,
    @username varchar(50),
    @first_name varchar(50),
    @last_name varchar(50),
	@user_email varchar(100),
    @online bit = null,
    @employee_id varchar(50),
    @last_login_date datetime = null,
    @last_login_attempt datetime = null,
    @number_of_incorrect_logins int = null,
    @last_password_changed_date datetime = null,
    @workstation nchar(50)  = null,
	@user_group_list AS user_group_id_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@connection_parameter_id int = null,
	@external_interface_id char(36)=null,
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
			DECLARE @dup_check int, @new_user_id bigint
			SELECT @dup_check = COUNT(*) 
			FROM [user]
			WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) = @username
					AND [user_id] != @user_id

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
					,[connection_parameter_id] = @connection_parameter_id
					,[language_id] = @language_id
					,[external_interface_id]=@external_interface_id
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
										 ', user: ' + @username + 
										 ', status: ' + COALESCE(@user_status_name, 'UNKNOWN') +
										 ', login ldap: ' + CAST(COALESCE(@connection_parameter_id, 0) as varchar(max)) + 
										 ', groups: ' + COALESCE(@groupname, 'none-selected')

				EXEC sp_insert_audit @audit_user_id, 
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




/****** Object:  StoredProcedure [dbo].[sp_insert_user]    Script Date: 2/22/2016 8:49:10 AM ******/
DROP PROCEDURE [dbo].[sp_insert_user]
GO

/****** Object:  StoredProcedure [dbo].[sp_insert_user]    Script Date: 2/22/2016 8:49:10 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 20 March 2014
-- Description:	Insert a user
-- =============================================
CREATE PROCEDURE [dbo].[sp_insert_user] 
	@user_status_id int,
    @username varchar(50),
    @first_name varchar(50),
    @last_name varchar(50),
    @password varchar(50),
	@user_email varchar(100),
    @online bit = 0,
    @employee_id varchar(50),
    @last_login_date datetime = null,
    @last_login_attempt datetime = null,
    @number_of_incorrect_logins int = 0,
    @last_password_changed_date datetime = null,
    @workstation nchar(50) = '',
	@user_group_list AS user_group_id_array READONLY,	 
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@connection_parameter_id int = null,
	@external_interface_id char(36)=null,
	@language_id int = null,
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
			DECLARE @dup_check int
			SELECT @dup_check = COUNT(*) 
			FROM [user]
			WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) = @username

			IF @dup_check > 0
				BEGIN
					SET @new_user_id = 0;
					SELECT @ResultCode = 69							
				END
			ELSE
			BEGIN
					INSERT INTO [user]
						([user_status_id],[user_gender_id],[username],[first_name],[last_name],[password],[user_email],[online]
						 ,[employee_id],[last_login_date],[last_login_attempt],[number_of_incorrect_logins]
						  ,[last_password_changed_date],[workstation], [connection_parameter_id], [language_id], [username_index],external_interface_id)
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
							@connection_parameter_id,
							@language_id,
							[dbo].[MAC] (@username, @objid),@external_interface_id)

					SET @new_user_id = SCOPE_IDENTITY();
			

				--Link user to user groups
				INSERT INTO [users_to_users_groups]
							([user_id], [user_group_id])
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
										 ', login ldap: ' + CAST(COALESCE(@connection_parameter_id, 0) as varchar(max)) + 
										 ', groups: ' + COALESCE(@groupname, 'none-selected')
				EXEC sp_insert_audit @audit_user_id, 
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










GO


