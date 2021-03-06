USE [indigo_database_main_dev]
GO
/****** Object:  Table [dbo].[product_load_type]    Script Date: 2015-06-30 12:37:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product_load_type](
	[product_load_type_id] [int] NOT NULL,
	[product_load_type_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_product_load_type] PRIMARY KEY CLUSTERED 
(
	[product_load_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

USE [indigo_database_main_dev]
GO
/****** Object:  Table [dbo].[product_load_type_language]    Script Date: 2015-07-09 04:56:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product_load_type_language](
	[product_load_type_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
 CONSTRAINT [PK_product_load_type_language] PRIMARY KEY CLUSTERED 
(
	[product_load_type_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[product_load_type_language]  WITH CHECK ADD  CONSTRAINT [FK_product_load_type_language_languages] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[product_load_type_language] CHECK CONSTRAINT [FK_product_load_type_language_languages]
GO
ALTER TABLE [dbo].[product_load_type_language]  WITH CHECK ADD  CONSTRAINT [FK_product_load_type_language_product_load_type_language] FOREIGN KEY([product_load_type_id])
REFERENCES [dbo].[product_load_type] ([product_load_type_id])
GO
ALTER TABLE [dbo].[product_load_type_language] CHECK CONSTRAINT [FK_product_load_type_language_product_load_type_language]
GO
