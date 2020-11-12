

ALTER TABLE [dbo].[user_admin] DROP CONSTRAINT [CK_user_admin]
GO

/****** Object:  Table [dbo].[user_admin]    Script Date: 2/16/2016 9:35:29 AM ******/
DROP TABLE [dbo].[user_admin]
GO

/****** Object:  Table [dbo].[user_admin]    Script Date: 2/16/2016 9:35:29 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[user_admin](
	[user_admin_id] [int] IDENTITY(1,1) NOT NULL,
	[PasswordValidPeriod] [int] NOT NULL,
	[PasswordMinLength] [int] NOT NULL,
	[PasswordMaxLength] [int] NOT NULL,
	[PreviousPasswordsCount] [int] NOT NULL,
	[maxInvalidPasswordAttempts] [int] NOT NULL,
	[PasswordAttemptLockoutDuration] [int] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
 CONSTRAINT [PK_user_admin] PRIMARY KEY CLUSTERED 
(
	[user_admin_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[user_admin]  WITH CHECK ADD  CONSTRAINT [CK_user_admin] CHECK  (([PasswordMaxLength]>[PasswordMinLength]))
GO

ALTER TABLE [dbo].[user_admin] CHECK CONSTRAINT [CK_user_admin]
GO


