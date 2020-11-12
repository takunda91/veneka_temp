-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_useradminsettings]
	@user_admin_id int,
	@PasswordValidPeriod int,
	@PasswordMinLength int,
	@PasswordMaxLength int,
	@PreviousPasswordsCount int,	
	@maxInvalidPasswordAttempts int,
	@PasswordAttemptLockoutDuration int,
	@audit_user_id int,
	@audit_workstation nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  update user_admin set PasswordValidPeriod=@PasswordValidPeriod,
										PasswordMinLength=@PasswordMinLength,
										PasswordMaxLength=@PasswordMaxLength,
										PreviousPasswordsCount=@PreviousPasswordsCount,
										maxInvalidPasswordAttempts=@maxInvalidPasswordAttempts,
										PasswordAttemptLockoutDuration=@PasswordAttemptLockoutDuration,
										CreatedBy=@audit_user_id
										where user_admin_id=@user_admin_id


			EXEC usp_insert_audit @audit_user_id, 
									4,
									NULL, 
									@audit_workstation, 
									'updated user admin settings.', 
									null, NULL, NULL, NULL

END