USE [indigo_database_main_dev]
GO
/****** Object:  Table [dbo].[export_batch]    Script Date: 2015-06-24 02:03:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[export_batch](
	[export_batch_id] [bigint] IDENTITY(1,1) NOT NULL,
	[issuer_id] [int] NOT NULL,
	[batch_reference] [varchar](100) NOT NULL,
	[date_created] [datetime2](7) NOT NULL,
	[no_cards] [int] NOT NULL,
 CONSTRAINT [PK_export_batch] PRIMARY KEY CLUSTERED 
(
	[export_batch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[export_batch_status]    Script Date: 2015-06-24 02:03:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[export_batch_status](
	[export_batch_status_id] [bigint] IDENTITY(1,1) NOT NULL,
	[export_batch_id] [bigint] NOT NULL,
	[export_batch_statuses_id] [int] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[status_date] [datetime2](7) NOT NULL,
	[comments] [varchar](100) NOT NULL,
 CONSTRAINT [PK_export_batch_status] PRIMARY KEY CLUSTERED 
(
	[export_batch_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[export_batch_statuses]    Script Date: 2015-06-24 02:03:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[export_batch_statuses](
	[export_batch_statuses_id] [int] NOT NULL,
	[export_batch_statuses_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_export_batch_statuses] PRIMARY KEY CLUSTERED 
(
	[export_batch_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[export_batch_statuses_language]    Script Date: 2015-06-24 02:03:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[export_batch_statuses_language](
	[export_batch_statuses_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
 CONSTRAINT [PK_export_batch_statuses_language] PRIMARY KEY CLUSTERED 
(
	[export_batch_statuses_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[export_batch]  WITH CHECK ADD  CONSTRAINT [FK_export_batch_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[export_batch] CHECK CONSTRAINT [FK_export_batch_issuer]
GO
ALTER TABLE [dbo].[export_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_export_batch_status_export_batch] FOREIGN KEY([export_batch_id])
REFERENCES [dbo].[export_batch] ([export_batch_id])
GO
ALTER TABLE [dbo].[export_batch_status] CHECK CONSTRAINT [FK_export_batch_status_export_batch]
GO
ALTER TABLE [dbo].[export_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_export_batch_status_export_batch_statuses] FOREIGN KEY([export_batch_statuses_id])
REFERENCES [dbo].[export_batch_statuses] ([export_batch_statuses_id])
GO
ALTER TABLE [dbo].[export_batch_status] CHECK CONSTRAINT [FK_export_batch_status_export_batch_statuses]
GO
ALTER TABLE [dbo].[export_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_export_batch_status_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[export_batch_status] CHECK CONSTRAINT [FK_export_batch_status_user]
GO
ALTER TABLE [dbo].[export_batch_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_export_batch_statuses_language_export_batch_statuses] FOREIGN KEY([export_batch_statuses_id])
REFERENCES [dbo].[export_batch_statuses] ([export_batch_statuses_id])
GO
ALTER TABLE [dbo].[export_batch_statuses_language] CHECK CONSTRAINT [FK_export_batch_statuses_language_export_batch_statuses]
GO
ALTER TABLE [dbo].[export_batch_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_export_batch_statuses_language_languages] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[export_batch_statuses_language] CHECK CONSTRAINT [FK_export_batch_statuses_language_languages]
GO
