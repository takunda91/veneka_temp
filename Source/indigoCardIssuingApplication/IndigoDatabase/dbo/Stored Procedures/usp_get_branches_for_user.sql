-- =============================================
-- Author:		Richard Brenchley
-- Create date: 12 March 2014
-- Description:	Fetch all branches by role, issuer and user
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branches_for_user] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL, 
	@user_id bigint,
	@user_role_id int = NULL,
	@branch_type_id int = null,
	@languageId int =null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @master_user int;

	--Check if the user has enterpise wide priviliages for the specific user role.
	SELECT @master_user = COUNT(*)
	FROM [users_to_users_groups]
		INNER JOIN [user_group]
			ON [users_to_users_groups].user_group_id = [user_group].user_group_id
	WHERE [users_to_users_groups].[user_id] = @user_id
		  AND [user_group].user_role_id = @user_role_id
		  AND [user_group].issuer_id = -1


	IF @master_user > 0
		BEGIN
			SELECT [branch].branch_code, [branch].branch_name, [branch].branch_id,
				   [branch].branch_status_id, [branch].issuer_id, bsl.language_text as branch_status,branch.branch_type_id, branch.main_branch_id
			FROM [branch] INNER JOIN [branch_statuses]
				ON [branch].branch_status_id = [branch_statuses].branch_status_id
				INNER JOIN [dbo].[branch_statuses_language] bsl
				 ON bsl.branch_status_id=[branch_statuses].branch_status_id
				INNER JOIN branch_type on branch.branch_type_id=branch_type.branch_type_id
			WHERE issuer_id = COALESCE(@issuer_id, issuer_id)
					AND [branch].branch_status_id = 0  
					--AND [branch].card_centre_branch_YN = COALESCE(@card_centre_branch_YN, [branch].card_centre_branch_YN)
					AND ([branch].branch_type_id =COALESCE( @branch_type_id,branch_type.branch_type_id))
					AND bsl.language_id=@languageId
		END
	ELSE
		BEGIN
			WITH branches (branch_code, branch_name, branch_id,
							branch_status_id, issuer_id,  branch_status,branch_type_id, main_branch_id)
AS
(SELECT  [branch].branch_code, [branch].branch_name, [branch].branch_id,
							[branch].branch_status_id, [branch].issuer_id, bsl.language_text as branch_status,branch.branch_type_id, branch.main_branch_id
			FROM [branch] 
					INNER JOIN [user_roles_branch]
						ON [user_roles_branch].branch_id = [branch].branch_id
					INNER JOIN [branch_statuses]
						ON [branch].branch_status_id = [branch_statuses].branch_status_id
						INNER JOIN [dbo].[branch_statuses_language] bsl
				 ON bsl.branch_status_id=[branch_statuses].branch_status_id
				WHERE [user_roles_branch].[user_id] = @user_id
				  AND [user_roles_branch].user_role_id = COALESCE(user_role_id, [user_roles_branch].user_role_id)
				  AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND [branch].branch_status_id = 0
				  --AND [branch].card_centre_branch_YN = COALESCE(@card_centre_branch_YN, [branch].card_centre_branch_YN)
					AND ([branch].branch_type_id =COALESCE( @branch_type_id,[branch].branch_type_id))
				  AND bsl.language_id=@languageId 
				UNION ALL
				SELECT  [branch].branch_code, [branch].branch_name, [branch].branch_id,
							[branch].branch_status_id, [branch].issuer_id, bsl.language_text as branch_status,branch.branch_type_id, branch.main_branch_id
			FROM [branch] 
					INNER JOIN [user_roles_branch]
						ON [user_roles_branch].branch_id = [branch].branch_id
					INNER JOIN [branch_statuses]
						ON [branch].branch_status_id = [branch_statuses].branch_status_id
						INNER JOIN [dbo].[branch_statuses_language] bsl
				 ON bsl.branch_status_id=[branch_statuses].branch_status_id
				INNER JOIN branch_type on branch.branch_type_id=branch_type.branch_type_id
				INNER JOIN branches on branch.main_branch_id = branches.branch_id
				where  bsl.language_id=@languageId 
			)
			SELECT DISTINCT branch_code, branch_name, branch_id,
							branch_status_id, issuer_id,  branch_status,branch_type_id, main_branch_id
from branches
			
		  END
END










