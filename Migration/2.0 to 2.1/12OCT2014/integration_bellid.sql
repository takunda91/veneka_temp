USE [indigo_database_main_dev]
GO

/****** Object:  Table [dbo].[integration_bellid_batch_sequence]    Script Date: 2014/10/13 10:19:04 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[integration_bellid_batch_sequence](
	[file_generation_date] [date] NOT NULL,
	[batch_sequence_number] [smallint] NOT NULL,
 CONSTRAINT [PK_integration_bellid_batch_sequence] PRIMARY KEY CLUSTERED 
(
	[file_generation_date] ASC,
	[batch_sequence_number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


