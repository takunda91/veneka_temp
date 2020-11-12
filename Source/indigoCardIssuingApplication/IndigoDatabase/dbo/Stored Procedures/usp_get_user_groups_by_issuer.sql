-- =============================================
-- Author:		Richard Brenchley
-- Create date: 13 March 2014
-- Description:	Get user groups by issuer.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_user_groups_by_issuer] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@user_role_id int = null,	
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

	BEGIN TRANSACTION [GET_USER_GROUPS]
		BEGIN TRY 

		DECLARE @StartRow INT, @EndRow INT;			

		SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
		SET @EndRow = @StartRow + @RowsPerPage - 1;

		WITH PAGE_ROWS
		AS
		(
		SELECT ROW_NUMBER() OVER(ORDER BY user_group_name ASC) AS ROW_NO
				, COUNT(*) OVER() AS TOTAL_ROWS
				, *
		FROM( 

			SELECT DISTINCT [user_group].*, url.language_text as user_role
			FROM [user_group] 
				INNER JOIN [issuer]
					ON user_group.issuer_id = issuer.issuer_id
				INNER JOIN [user_roles]
					ON [user_group].user_role_id = [user_roles].user_role_id
						INNER JOIN  [dbo].user_roles_language url 
					ON url.user_role_id=[user_roles].user_role_id 
			WHERE [user_group].issuer_id = @issuer_id
					AND [user_group].user_role_id = COALESCE(@user_role_id, [user_group].user_role_id)			
						AND url.language_id=@languageId
		) AS Src )
		SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
			,*
		FROM PAGE_ROWS
		WHERE ROW_NO BETWEEN @StartRow AND @EndRow
		ORDER BY user_group_name ASC

		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

		--log the audit record		
		--EXEC usp_insert_audit @audit_user_id, 
		--						1,
		--						NULL, 
		--						@audit_workstation, 
		--						'Getting user groups.', 
		--						NULL, NULL, NULL, NULL

		COMMIT TRANSACTION [GET_USER_GROUPS]
	END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION [GET_USER_GROUPS]
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