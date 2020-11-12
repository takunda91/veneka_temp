USE [indigo_database_main_dev]
GO

/****** Object:  Table [dbo].[masterkeys]    Script Date: 2015/01/28 11:38:32 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[masterkeys](
	[masterkey_id] [int] IDENTITY(1,1) NOT NULL,
	[masterkey] [varbinary](max) NOT NULL,
	[date_created] [datetime] NULL,
	[date_changed] [datetime] NULL,
 CONSTRAINT [PK_masterkeys] PRIMARY KEY CLUSTERED 
(
	[masterkey_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


