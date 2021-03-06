USE [indigo_database_main_dev]
GO
/****** Object:  Table [dbo].[external_system_fields]    Script Date: 2016-03-23 11:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[external_system_fields](
	[external_system_field_id] [int] IDENTITY(1,1) NOT NULL,
	[external_system_id] [int] NOT NULL,
	[field_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_external_system_fields] PRIMARY KEY CLUSTERED 
(
	[external_system_field_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[external_system_types]    Script Date: 2016-03-23 11:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[external_system_types](
	[external_system_type_id] [int] NOT NULL,
	[system_type_name] [varchar](150) NOT NULL,
 CONSTRAINT [PK_external_system_types] PRIMARY KEY CLUSTERED 
(
	[external_system_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[external_system_types_language]    Script Date: 2016-03-23 11:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[external_system_types_language](
	[external_system_type_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](1000) NOT NULL,
 CONSTRAINT [PK_external_system_types_language] PRIMARY KEY CLUSTERED 
(
	[external_system_type_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[external_systems]    Script Date: 2016-03-23 11:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[external_systems](
	[external_system_id] [int] IDENTITY(1,1) NOT NULL,
	[external_system_type_id] [int] NOT NULL,
	[system_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_external_systems] PRIMARY KEY CLUSTERED 
(
	[external_system_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[product_external_system]    Script Date: 2016-03-23 11:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product_external_system](
	[external_system_field_id] [int] NOT NULL,
	[product_id] [int] NOT NULL,
	[field_value] [varchar](150) NOT NULL,
 CONSTRAINT [PK_product_external_system] PRIMARY KEY CLUSTERED 
(
	[external_system_field_id] ASC,
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[external_system_types] ([external_system_type_id], [system_type_name]) VALUES (0, N'Core Banking System')
GO
INSERT [dbo].[external_system_types] ([external_system_type_id], [system_type_name]) VALUES (1, N'Card Production System')
GO
INSERT [dbo].[external_system_types] ([external_system_type_id], [system_type_name]) VALUES (2, N'Card Management System')
GO
INSERT [dbo].[external_system_types] ([external_system_type_id], [system_type_name]) VALUES (3, N'Hardware Security Module')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (0, 0, N'Core Banking System')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (0, 1, N'Core Banking System_fr')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (0, 2, N'Core Banking System_pt')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (0, 3, N'Core Banking System_es')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (1, 0, N'Card Production System')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (1, 1, N'Card Production System_fr')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (1, 2, N'Card Production System_pt')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (1, 3, N'Card Production System_es')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (2, 0, N'Card Management System')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (2, 1, N'Card Management System_fr')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (2, 2, N'Card Management System_pt')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (2, 3, N'Card Management System_es')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (3, 0, N'Hardware Security Module')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (3, 1, N'Hardware Security Module_fr')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (3, 2, N'Hardware Security Module_pt')
GO
INSERT [dbo].[external_system_types_language] ([external_system_type_id], [language_id], [language_text]) VALUES (3, 3, N'Hardware Security Module_es')
GO
ALTER TABLE [dbo].[external_system_fields]  WITH CHECK ADD  CONSTRAINT [FK_external_system_fields_external_systems] FOREIGN KEY([external_system_id])
REFERENCES [dbo].[external_systems] ([external_system_id])
GO
ALTER TABLE [dbo].[external_system_fields] CHECK CONSTRAINT [FK_external_system_fields_external_systems]
GO
ALTER TABLE [dbo].[external_system_types_language]  WITH CHECK ADD  CONSTRAINT [FK_external_system_types_language_external_system_types] FOREIGN KEY([external_system_type_id])
REFERENCES [dbo].[external_system_types] ([external_system_type_id])
GO
ALTER TABLE [dbo].[external_system_types_language] CHECK CONSTRAINT [FK_external_system_types_language_external_system_types]
GO
ALTER TABLE [dbo].[external_system_types_language]  WITH CHECK ADD  CONSTRAINT [FK_external_system_types_language_languages] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[external_system_types_language] CHECK CONSTRAINT [FK_external_system_types_language_languages]
GO
ALTER TABLE [dbo].[external_systems]  WITH CHECK ADD  CONSTRAINT [FK_external_systems_external_system_types] FOREIGN KEY([external_system_type_id])
REFERENCES [dbo].[external_system_types] ([external_system_type_id])
GO
ALTER TABLE [dbo].[external_systems] CHECK CONSTRAINT [FK_external_systems_external_system_types]
GO
ALTER TABLE [dbo].[product_external_system]  WITH CHECK ADD  CONSTRAINT [FK_external_system_product] FOREIGN KEY([external_system_field_id])
REFERENCES [dbo].[external_system_fields] ([external_system_field_id])
GO
ALTER TABLE [dbo].[product_external_system] CHECK CONSTRAINT [FK_external_system_product]
GO
ALTER TABLE [dbo].[product_external_system]  WITH CHECK ADD  CONSTRAINT [FK_product_external_system] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO
ALTER TABLE [dbo].[product_external_system] CHECK CONSTRAINT [FK_product_external_system]
GO
