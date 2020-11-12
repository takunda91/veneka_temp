USE [indigo_database_main_dev]
GO

ALTER TABLE [connection_parameters] DROP CONSTRAINT FK_connection_parameters_file_encryption_type

/****** Object:  Table [dbo].[file_encryption_type]    Script Date: 2016-02-23 04:35:22 PM ******/
DROP TABLE [dbo].[file_encryption_type]
GO

/****** Object:  Table [dbo].[file_encryption_type]    Script Date: 2016-02-23 04:35:22 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[file_encryption_type](
	[file_encryption_type_id] [int] NOT NULL,
	[file_encryption_type] [varchar](250) NOT NULL,
 CONSTRAINT [PK_file_encryption_type] PRIMARY KEY CLUSTERED 
(
	[file_encryption_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

INSERT [dbo].[file_encryption_type] ([file_encryption_type_id], [file_encryption_type]) VALUES (0, N'NONE')
GO
INSERT [dbo].[file_encryption_type] ([file_encryption_type_id], [file_encryption_type]) VALUES (1, N'PGP')
GO

UPDATE [connection_parameters]
SET file_encryption_type_id = 0
GO

ALTER TABLE [connection_parameters]
ADD CONSTRAINT FK_connection_parameters_file_encryption_type
FOREIGN KEY (file_encryption_type_id)
REFERENCES [file_encryption_type](file_encryption_type_id)
GO

