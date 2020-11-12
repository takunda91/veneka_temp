CREATE PROCEDURE [dbo].[usp_get_unassigned_users] 
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

	BEGIN TRANSACTION [GET_UNASSIGNED_USERS]
		BEGIN TRY 

		DECLARE @StartRow INT, @EndRow INT;			

		SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
		SET @EndRow = @StartRow + @RowsPerPage - 1;

		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate;

		WITH PAGE_ROWS
		AS
		(
		SELECT ROW_NUMBER() OVER(ORDER BY username ASC) AS ROW_NO
				, COUNT(*) OVER() AS TOTAL_ROWS
				, *
		FROM( 
			SELECT DISTINCT [user].[user_id]						
							,CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username'
							,CONVERT(VARCHAR,DECRYPTBYKEY([user].[first_name])) as 'first_name'
							,CONVERT(VARCHAR,DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
							,CONVERT(VARCHAR,DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
							,usl.language_text as  [user_status_text] 
							,[user].[online]    
							,[user].[workstation],acc.authentication_configuration_id,
							case when (acc.[connection_parameter_id] is null) then 'Indigo' else 'LDAP' end as 'AUTHENTICATION_TYPE'
				FROM [user]
					INNER JOIN user_status
						ON user_status.user_status_id = [user].user_status_id
					INNER JOIN  [dbo].[user_status_language] usl
						 ON usl.user_status_id=user_status.user_status_id 
						 INNER JOIN [dbo].[auth_configuration] ac ON
					[user].[authentication_configuration_id]= ac.[authentication_configuration_id]
					inner join [dbo].[auth_configuration_connection_parameters] acc ON
					ac.[authentication_configuration_id]= acc.[authentication_configuration_id]
				WHERE [user].[user_id] NOT IN (SELECT DISTINCT [user_id] FROM [users_to_users_groups]) 
				 AND usl.language_id=@languageId and (acc.auth_type_id=0 or acc.auth_type_id is null)
			) AS Src )
		SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
			,*
		FROM PAGE_ROWS
		WHERE ROW_NO BETWEEN @StartRow AND @EndRow
		ORDER BY username ASC

		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

		--log the audit record		
		--EXEC usp_insert_audit @audit_user_id, 
		--						1,
		--						NULL, 
		--						@audit_workstation, 
		--						'Getting unassigned users.', 
		--						NULL, NULL, NULL, NULL

		COMMIT TRANSACTION [GET_UNASSIGNED_USERS]
	END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION [GET_UNASSIGNED_USERS]
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
