CREATE PROCEDURE [dbo].[usp_get_users_by_branch_admin] 
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
						,[user].[workstation]
						,case when (acc.[connection_parameter_id] is null) then 'Indigo' else 'LDAP' end as 'AUTHENTICATION_TYPE'
						,[user].authentication_configuration_id,branch.issuer_id
			FROM [branch] INNER JOIN [user_group_branch_ex_ent]
					ON [branch].branch_id = [user_group_branch_ex_ent].branch_id
				INNER JOIN [user]
					ON [user].[user_id] = [user_group_branch_ex_ent].[user_id]
				INNER JOIN user_status
					ON user_status.user_status_id = [user].user_status_id
				INNER JOIN  [dbo].[user_status_language] usl 
					ON usl.user_status_id=user_status.user_status_id 
					INNER JOIN [dbo].[auth_configuration] ac ON
					[user].[authentication_configuration_id]= ac.[authentication_configuration_id]
					inner join [dbo].[auth_configuration_connection_parameters] acc ON
					ac.[authentication_configuration_id]= acc.[authentication_configuration_id] 
			WHERE [user_group_branch_ex_ent].issuer_id = COALESCE(@issuer_id, [user_group_branch_ex_ent].issuer_id)
				AND (@username IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) LIKE @username) 
				AND (@first_name IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) LIKE @first_name) 
				AND (@last_name IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) LIKE @last_name) 
				AND [branch].branch_id = COALESCE(@branch_id, branch.branch_id)				
				AND [branch].branch_status_id = COALESCE(@branch_status_id, branch.branch_status_id)
				AND [user].user_status_id = COALESCE(@user_status_id, [user].user_status_id)
				AND [user_group_branch_ex_ent].user_role_id = COALESCE(@user_role_id, [user_group_branch_ex_ent].user_role_id)
				AND usl.language_id=@languageId and [branch].branch_status_id=0 
				and (acc.auth_type_id=0 or acc.auth_type_id is null)
		) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY username ASC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END