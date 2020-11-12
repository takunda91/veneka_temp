

/****** Object:  StoredProcedure [dbo].[sp_get_useradminsettingslist]    Script Date: 2/16/2016 9:38:01 AM ******/
DROP PROCEDURE [dbo].[sp_get_useradminsettingslist]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_useradminsettingslist]    Script Date: 2/16/2016 9:38:01 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_useradminsettingslist]

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

GO

/****** Object:  StoredProcedure [dbo].[sp_create_useradminsettings]    Script Date: 2/16/2016 9:38:37 AM ******/
DROP PROCEDURE [dbo].[sp_create_useradminsettings]
GO

/****** Object:  StoredProcedure [dbo].[sp_create_useradminsettings]    Script Date: 2/16/2016 9:38:37 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_create_useradminsettings]
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
VALUES        (@PasswordValidPeriod,@PasswordMinLength,@PasswordMaxLength,@PreviousPasswordsCount,@maxInvalidPasswordAttempts,@PasswordAttemptLockoutDuration,@CreatedBy,GETDATE())


set @user_admin_id=SCOPE_IDENTITY()

															
			EXEC sp_insert_audit @CreatedBy, 
									4,
									NULL, 
									@audit_workstation, 
									'created user admin settings.', 
									null, NULL, NULL, NULL

END

GO


/****** Object:  StoredProcedure [dbo].[sp_update_useradminsettings]    Script Date: 2/16/2016 9:39:09 AM ******/
DROP PROCEDURE [dbo].[sp_update_useradminsettings]
GO

/****** Object:  StoredProcedure [dbo].[sp_update_useradminsettings]    Script Date: 2/16/2016 9:39:09 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_update_useradminsettings]
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
										createdby=@audit_user_id
										where user_admin_id=@user_admin_id


			EXEC sp_insert_audit @audit_user_id, 
									4,
									NULL, 
									@audit_workstation, 
									'updated user admin settings.', 
									null, NULL, NULL, NULL

END


GO




