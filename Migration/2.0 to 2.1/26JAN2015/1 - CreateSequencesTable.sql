USE [indigo_database_main_dev]
GO

/****** Object:  Table [dbo].[sequences]    Script Date: 2015/01/26 02:47:34 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[sequences](
	[sequence_name] [varchar](100) NOT NULL,
	[last_sequence_number] [bigint] NOT NULL,
	[last_updated] [datetime] NOT NULL,
 CONSTRAINT [PK_sequences] PRIMARY KEY CLUSTERED 
(
	[sequence_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


