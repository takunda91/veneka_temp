-- =============================================
-- Author:		Richard Brenchley
-- Description:	Fetch all branches by issuer and user for use on branch admin screen.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branches_for_user_admin] 
	@issuer_id int = NULL, 
	@branch_status_id int=NULL,
	@user_id bigint,
	@languageId int =null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT DISTINCT [branch].branch_code, [branch].branch_name, [branch].branch_id,
				    [branch].branch_status_id, [branch].issuer_id, bsl.language_text as branch_status,branch.branch_type_id, branch.main_branch_id
	FROM [branch] 
			INNER JOIN [branch_statuses]
				ON [branch].branch_status_id = [branch_statuses].branch_status_id
			INNER JOIN [dbo].[branch_statuses_language] bsl ON bsl.branch_status_id=[branch_statuses].branch_status_id
	WHERE issuer_id = COALESCE(@issuer_id, issuer_id) 
	AND [branch].branch_status_id =COALESCE(@branch_status_id, [branch].branch_status_id)
	AND bsl.language_id=@languageId
	AND	  branch_id IN (SELECT branch_id
						FROM user_roles_branch
						WHERE [user_id] = @user_id
								AND user_role_id = 10)

	ORDER BY [branch].branch_code

END
