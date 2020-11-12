-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_useradminsettingslist]

	@audit_user_id bigint,
	@audit_workstation nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select user_admin_id, PasswordValidPeriod,PasswordMinLength,PasswordMaxLength,PreviousPasswordsCount,maxInvalidPasswordAttempts,PasswordAttemptLockoutDuration
	from user_admin 
	--where user_admin_id=@user_admin_id
						
END