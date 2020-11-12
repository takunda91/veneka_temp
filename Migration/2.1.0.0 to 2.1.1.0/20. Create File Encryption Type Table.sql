USE [indigo_database_main_dev]
GO

/****** Object:  Table [dbo].[file_encryption_type]    Script Date: 2015-09-22 11:24:27 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[file_encryption_type](
	[file_encryption_type_id] [int] IDENTITY(1,1) NOT NULL,
	[file_encryption_type] [varchar](250) NOT NULL,
 CONSTRAINT [PK_file_encryption_type] PRIMARY KEY CLUSTERED 
(
	[file_encryption_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


