[{DATABASE_NAME}].[dbo].[user_admin]
SELECT [user_admin_id]
      ,[PasswordValidPeriod]
      ,[PasswordMinLength]
      ,[PasswordMaxLength]
      ,[PreviousPasswordsCount]
      ,[maxInvalidPasswordAttempts]
      ,[PasswordAttemptLockoutDuration]
      ,[CreatedBy]
      ,ToDateTimeOffset([CreatedDateTime], DATENAME(tz, SYSDATETIMEOFFSET())) as [CreatedDateTime]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[user_admin]