CREATE PROCEDURE [dbo].[usp_search_user]
	@user_name varchar(50) =null,
	@first_name varchar(50) =null,
	@last_name varchar(50)=null,
	@issuer_id int =null,
	@branch_id varchar(10)=null,
	@user_role varchar(30)=null,
	@audit_user_id int =null,
	@audit_workstation varchar(100)=null,
	@PageIndex INT = 1,
@RowsPerPage INT = 20
AS
BEGIN

IF((@user_name=null) OR (@user_name=''))
	BEGIN
		SET	@user_name =NULL
	END

	IF((@first_name=null) OR (@first_name=''))
	BEGIN
		SET	@first_name =NULL
	END
	
	IF((@last_name=null) OR (@last_name=''))
	BEGIN
	SET	@last_name =NULL
	
	END
		
	IF((@branch_id=null) OR (@branch_id=0))
	BEGIN
	SET	@branch_id =NULL
	END
	
	IF((@issuer_id=null) OR (@issuer_id=0))
	BEGIN
	SET	@issuer_id =NULL
	END
	
	IF((@user_role=null) OR (@user_role=''))
	BEGIN
	SET	@user_role =NULL
	END
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

	
select distinct u.[user_id]						
							,CONVERT(VARCHAR(max),DECRYPTBYKEY(u.[username])) as 'username'
							,CONVERT(VARCHAR(max),DECRYPTBYKEY(u.[first_name])) as 'first_name'
							,CONVERT(VARCHAR(max),DECRYPTBYKEY(u.[last_name])) as 'last_name' 					
							,CONVERT(VARCHAR(max),DECRYPTBYKEY(u.[employee_id])) as 'empoyee_id'
							,us.[user_status_text] 
							,u.[online]    
							,u.[workstation],b.issuer_id,acc.authentication_configuration_id,
						case when (acc.[connection_parameter_id] is null) then 'Indigo' else 'LDAP' end as 'AUTHENTICATION_TYPE'

	
	from [user] u
inner join [dbo].[user_roles_branch] urb on urb.[user_id] =u.[user_id]
inner join branch b on b.branch_id= urb.branch_id
inner join user_status us on us.user_status_id=u.user_status_id
inner join user_roles ur on ur.user_role_id=urb.user_role_id
INNER JOIN [dbo].[auth_configuration] ac ON
					u.[authentication_configuration_id]= ac.[authentication_configuration_id]
					inner join [dbo].[auth_configuration_connection_parameters] acc ON
					ac.[authentication_configuration_id]= acc.[authentication_configuration_id]
where 
ISNULL(b.issuer_id, '')=ISNULL(@issuer_id, ISNULL(b.issuer_id, '')) and
 ISNULL(b.branch_id, '')=ISNULL(@branch_id, ISNULL(b.branch_id, ''))  and
 ISNULL(ur.user_role, '')=ISNULL(@user_role, ISNULL(ur.user_role, ''))  and
 (ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(u.username)), '')=ISNULL(@user_name, ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(u.username)), '')) or  
 CONVERT(VARCHAR(max),DECRYPTBYKEY(u.username)) like '%'+@user_name+'%') and
 (ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(u.first_name)), '')=ISNULL(@first_name, ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(u.first_name)), '')) or
  CONVERT(VARCHAR(max),DECRYPTBYKEY(u.first_name)) like '%'+@first_name+'%') and
  (ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(u.last_name)), '')=ISNULL(@last_name, ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(u.last_name)), ''))
 or   CONVERT(VARCHAR(max),DECRYPTBYKEY(u.last_name)) like '%'+@last_name+'%')  and (acc.auth_type_id=0 or acc.auth_type_id is null)

		)AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY username ASC

	--EXEC usp_insert_audit @audit_user_id, 
	--							1,
	--							NULL, 
	--							@audit_workstation, 
	--							'Getting unassigned users.', 
	--							NULL, NULL, NULL, NULL

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END
GO

