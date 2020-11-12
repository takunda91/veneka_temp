

INSERT INTO [dbo].[user_admin]
           ([PasswordValidPeriod]
           ,[PasswordMinLength]
           ,[PasswordMaxLength]
           ,[PreviousPasswordsCount]
           ,[maxInvalidPasswordAttempts]
           ,[PasswordAttemptLockoutDuration]
           ,[CreatedBy]
           ,[CreatedDateTime])
     VALUES
           (30
           ,5
           ,12
           ,3
           ,5
           ,4
           ,-1
           ,GetDate())
GO


