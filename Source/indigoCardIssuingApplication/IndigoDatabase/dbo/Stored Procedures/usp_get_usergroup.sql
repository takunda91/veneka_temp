CREATE PROCEDURE [dbo].[usp_get_usergroup]
 @user_group_id int,
 @audit_user_id bigint,
 @audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT *
	FROM [user_group]
	WHERE user_group_id = @user_group_id

END