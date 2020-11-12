USE [indigo_database_main_dev]
GO

/****** Object:  Table [dbo].[print_field_types]    Script Date: 2015-11-20 09:42:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[print_field_types](
	[print_field_type_id] [int] NOT NULL,
	[print_field_name] [varchar](50) NULL,
 CONSTRAINT [PK_print_field_types] PRIMARY KEY CLUSTERED 
(
	[print_field_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


