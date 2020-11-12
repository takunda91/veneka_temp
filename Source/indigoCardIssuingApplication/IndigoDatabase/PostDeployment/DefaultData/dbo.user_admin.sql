SET IDENTITY_INSERT [dbo].[user_admin] ON;

-- Only insert initial value
IF NOT EXISTS (SELECT [user_admin_id] FROM [dbo].[user_admin])
BEGIN
	INSERT INTO [dbo].[user_admin] ([user_admin_id],[PasswordValidPeriod],[PasswordMinLength],[PasswordMaxLength],[PreviousPasswordsCount],[maxInvalidPasswordAttempts],[PasswordAttemptLockoutDuration],[CreatedBy],[CreatedDateTime])
	VALUES (1,30,5,12,3,5,4,-1,SYSDATETIMEOFFSET())
END

SET IDENTITY_INSERT [dbo].[user_admin] OFF;
