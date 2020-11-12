-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_useradminsettings]
	@PasswordValidPeriod int,
	@PasswordMinLength int,
	@PasswordMaxLength int,
	@PreviousPasswordsCount int,
	@maxInvalidPasswordAttempts int,
	@PasswordAttemptLockoutDuration int,
	@CreatedBy int,
	@audit_workstation nvarchar(50),
	@user_admin_id int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  INSERT INTO user_admin
                         (PasswordValidPeriod, PasswordMinLength, PasswordMaxLength, PreviousPasswordsCount,maxInvalidPasswordAttempts,PasswordAttemptLockoutDuration, CreatedBy, CreatedDateTime)
VALUES        (@PasswordValidPeriod,@PasswordMinLength,@PasswordMaxLength,@PreviousPasswordsCount,@maxInvalidPasswordAttempts,@PasswordAttemptLockoutDuration,@CreatedBy,SYSDATETIMEOFFSET())


set @user_admin_id=SCOPE_IDENTITY()

															
			EXEC usp_insert_audit @CreatedBy, 
									4,
									NULL, 
									@audit_workstation, 
									'created user admin settings.', 
									null, NULL, NULL, NULL

END