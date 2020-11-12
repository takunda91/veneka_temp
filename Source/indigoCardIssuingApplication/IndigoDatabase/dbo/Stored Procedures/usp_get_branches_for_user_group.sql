-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get a list of branches for a user group.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branches_for_user_group] 
	-- Add the parameters for the stored procedure here
	@user_group_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT branch_id
	FROM user_groups_branches
	WHERE user_group_id = @user_group_id
END