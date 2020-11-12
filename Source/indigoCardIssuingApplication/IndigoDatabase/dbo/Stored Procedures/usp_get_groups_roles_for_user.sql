-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_groups_roles_for_user] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@languageId int =null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [user_group].user_group_id, [user_group].user_group_name, url.language_text as user_role, [issuer].issuer_code, 
		   [issuer].issuer_name
	FROM [user_group] 
			INNER JOIN [issuer]
				ON [user_group].issuer_id = [issuer].issuer_id
			INNER JOIN [users_to_users_groups]
				ON [user_group].user_group_id = [users_to_users_groups].user_group_id
			INNER JOIN [user_roles]
				ON [user_group].user_role_id = [user_roles].user_role_id
				INNER JOIN [user_roles_language] url
				ON url.user_role_id = [user_roles].user_role_id
	WHERE [users_to_users_groups].[user_id] = @user_id
	AND url.language_id=@languageId
END