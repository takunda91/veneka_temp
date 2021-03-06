
/****** Object:  StoredProcedure [dbo].[sp_GetAuditData]    Script Date: 2015/04/07 10:49:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

ALTER PROCEDURE [dbo].[sp_GetAuditData]
	@audit_action_id int = NULL,
	@user_role_id int = NULL,
	@username varchar(50) = NULL,
	@issuer_id int = NULL,
	@date_from datetime ,
	@date_to datetime,	
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@aduti_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY audit_date DESC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
				SELECT [audit_control].audit_id,[audit_control].action_description,CONVERT(VARCHAR,DECRYPTBYKEY([user].username)) as 'UserName', CONVERT(VARCHAR(10),audit_date,103) as audit_date,CONVERT(VARCHAR(30),audit_date,108) as 'audit_Time', [workstation_address],
					  [audit_action].audit_action_name, [audit_control].[data_before],[audit_control].[data_after],
					   [audit_control].issuer_id 
				FROM [audit_control]
						INNER JOIN [audit_action] 
							ON [audit_control].audit_action_id = [audit_action].audit_action_id
						INNER JOIN [user]
							ON [user].[user_id] = [audit_control].[user_id]												
				WHERE [audit_control].audit_action_id = COALESCE(@audit_action_id, [audit_control].audit_action_id)
					AND audit_date BETWEEN @date_from AND DATEADD(day, 1, @date_to)					
					AND ((@username IS NULL) OR (CONVERT(VARCHAR,DECRYPTBYKEY([user].username)) LIKE @username))
					--AND [audit_control].issuer_id = COALESCE(@issuer_id, [audit_control].issuer_id)	
					AND [user].[user_id] IN (SELECT [user_id] 
											 FROM user_group
											 inner join users_to_users_groups on users_to_users_groups.[user_group_id]=user_group.[user_group_id]
											 WHERE user_group.user_role_id = COALESCE(@user_role_id, user_group.[user_role_id])
													AND user_group.issuer_id = COALESCE(@issuer_id, user_group.issuer_id))
						--	ON [user].[user_id] = [user_roles_branch].[user_id]	
						--	AND [user_roles_branch].user_role_id = COALESCE(@user_role_id, [user_roles_branch].user_role_id )	 
		)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY audit_date DESC

	
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END







