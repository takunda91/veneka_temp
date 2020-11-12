USE [{DATABASE_NAME}]
GO

declare @objid int
SET @objid = object_id('cards')
EXEC	[dbo].[AddMacForTable]
		@Table_id = @objid
GO

declare @objid int
SET @objid = object_id('user')
EXEC	[dbo].[AddMacForTable]
		@Table_id = @objid
GO

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
           ,GETDATE())
