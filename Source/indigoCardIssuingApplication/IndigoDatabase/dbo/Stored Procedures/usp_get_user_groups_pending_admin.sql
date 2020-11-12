-- =============================================
-- Author:		
-- Create date: 
-- Description:	Gets a list of user groups and indicates if the user has been assigned to it.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_user_groups_pending_admin] 
	-- Add the parameters for the stored procedure here
	@user_id bigint = null,
	@issuer_id int = NULL,	
	@branch_id int = NULL,
	@user_role_id int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT ug.user_group_id, ug.user_group_name, ug.user_role_id, ug.issuer_id,
	CASE WHEN [user_to_user_group_pending].[pending_user_id] IS NULL THEN 0 ELSE 1 END 'is_in_group'
	FROM [user_group] ug
		LEFT OUTER JOIN [user_to_user_group_pending]
					ON ug.user_group_id = [user_to_user_group_pending].user_group_id
						AND [user_to_user_group_pending].pending_user_id = @user_id
	WHERE (@branch_id IS NULL OR 
		((ug.all_branch_access = 1 AND
			EXISTS (SELECT b.*
					FROM [branch] b
					WHERE b.issuer_id = ug.issuer_id
						  AND b.branch_id = COALESCE(@branch_id, b.branch_id)))
		 OR 
		  (ug.all_branch_access = 0 AND
			  EXISTS (SELECT ugb.* 
					  FROM [user_groups_branches] ugb
					  WHERE ugb.user_group_id = ug.user_group_id
							AND ugb.branch_id = COALESCE(@branch_id, ugb.branch_id)))))
		 AND ug.issuer_id = COALESCE(@issuer_id, ug.issuer_id)
		 AND ug.user_role_id = COALESCE(@user_role_id, ug.user_role_id)
		  
END










