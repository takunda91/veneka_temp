
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_userroles_enterprise]
	@language_id int,
	@enterprise_only int =null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT        user_roles_language.user_role_id AS lookup_id, language_text
FROM            user_roles INNER JOIN
                         user_roles_language ON user_roles.user_role_id = user_roles_language.user_role_id
						 where user_roles_language.language_id=@language_id and enterprise_only = COALESCE(@enterprise_only, user_roles.enterprise_only)
END