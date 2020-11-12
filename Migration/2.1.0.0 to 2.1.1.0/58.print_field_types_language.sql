
ALTER TABLE [dbo].[print_field_types_language] DROP CONSTRAINT [FK_print_field_types_language_print_field_types]
GO

/****** Object:  Table [dbo].[print_field_types_language]    Script Date: 3/1/2016 12:06:17 PM ******/
DROP TABLE [dbo].[print_field_types_language]
GO

/****** Object:  Table [dbo].[print_field_types_language]    Script Date: 3/1/2016 12:06:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[print_field_types_language](
	[print_field_type_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](1000) NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[print_field_types_language]  WITH CHECK ADD  CONSTRAINT [FK_print_field_types_language_print_field_types] FOREIGN KEY([print_field_type_id])
REFERENCES [dbo].[print_field_types] ([print_field_type_id])
GO

ALTER TABLE [dbo].[print_field_types_language] CHECK CONSTRAINT [FK_print_field_types_language_print_field_types]
GO


