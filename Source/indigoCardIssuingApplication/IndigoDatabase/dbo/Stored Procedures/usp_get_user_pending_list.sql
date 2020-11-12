CREATE PROCEDURE [dbo].[usp_get_user_pending_list]
	@issuer_id int =null,
	@branch_id int=null,
	@user_name varchar(100)=null,
	@audit_user_id bigint =null,
	@audit_workstation varchar(100)=null,
	@PageIndex INT = 1,
@RowsPerPage INT = 20
AS
BEGIN


		
	IF((@branch_id=null) OR (@branch_id=0))
	BEGIN
	SET	@branch_id =NULL
	END
	
	IF((@issuer_id=null) OR (@issuer_id=0))
	BEGIN
	SET	@issuer_id =NULL
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

	
select distinct u.[pending_user_id] as [user_id]						
							,CONVERT(VARCHAR(max),DECRYPTBYKEY(u.[username])) as 'username'
							,CONVERT(VARCHAR(max),DECRYPTBYKEY(u.[first_name])) as 'first_name'
							,CONVERT(VARCHAR(max),DECRYPTBYKEY(u.[last_name])) as 'last_name' 					
							,CONVERT(VARCHAR(max),DECRYPTBYKEY(u.[employee_id])) as 'empoyee_id'
							,us.[user_status_text] 
							,u.[online]    
							,u.[workstation],b.issuer_id,u.authentication_configuration_id
							,case when (acc.[connection_parameter_id] is null) then 'Indigo' else 'LDAP' end as 'AUTHENTICATION_TYPE'
	
	from [user_pending] u
inner join [dbo].user_roles_branch_pending urb on urb.[user_id] =u.[pending_user_id]
inner join branch b on b.branch_id= urb.branch_id
inner join user_status us on us.user_status_id=u.user_status_id
inner join user_roles ur on ur.user_role_id=urb.user_role_id
INNER JOIN [dbo].[auth_configuration] ac ON
					u.[authentication_configuration_id]= ac.[authentication_configuration_id]
					inner join [dbo].[auth_configuration_connection_parameters] acc ON
					ac.[authentication_configuration_id]= acc.[authentication_configuration_id]

where 
b.issuer_id=COALESCE(@issuer_id, b.issuer_id) and
 b.branch_id=ISNULL(@branch_id, b.branch_id)  and
 (ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(u.username)), '')=ISNULL(@user_name, ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(u.username)), '')) ) 

 and (acc.auth_type_id=0 or acc.auth_type_id is null)
 )
		AS Src )
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

