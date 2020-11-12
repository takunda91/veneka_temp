USE [indigo_database_2.1.0.0]
GO

/****** Object:  Table [dbo].[load_batch_types]    Script Date: 2015-09-29 05:01:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[load_batch_types](
	[load_batch_type_id] [int] NOT NULL,
	[load_batch_type] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_load_batch_types] PRIMARY KEY CLUSTERED 
(
	[load_batch_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


