USE [indigo_database_main_dev]
GO

/****** Object:  Table [dbo].[terminals]    Script Date: 2015/02/04 02:47:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[terminals](
	[terminal_id] [int] IDENTITY(1,1) NOT NULL,
	[terminal_name] [varchar](250) NOT NULL,
	[terminal_model] [varchar](250) NULL,
	[session_key] [varchar](max) NULL,
	[device_id] [varchar](max) NULL,
	[branch_id] [int] NOT NULL,
	[terminal_masterkey_id] [int] NOT NULL,
	[workstation] [nvarchar](250) NULL,
	[date_created] [datetime] NULL,
	[date_changed] [datetime] NULL,
 CONSTRAINT [PK_terminals] PRIMARY KEY CLUSTERED 
(
	[terminal_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[terminals]  WITH CHECK ADD  CONSTRAINT [FK_terminals_branch] FOREIGN KEY([branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO

ALTER TABLE [dbo].[terminals] CHECK CONSTRAINT [FK_terminals_branch]
GO

ALTER TABLE [dbo].[terminals]  WITH CHECK ADD  CONSTRAINT [FK_terminals_masterkeys] FOREIGN KEY([terminal_masterkey_id])
REFERENCES [dbo].[masterkeys] ([masterkey_id])
GO

ALTER TABLE [dbo].[terminals] CHECK CONSTRAINT [FK_terminals_masterkeys]
GO


