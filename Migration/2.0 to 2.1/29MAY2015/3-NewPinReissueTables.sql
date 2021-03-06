USE [indigo_database_main_dev]
GO
/****** Object:  Table [dbo].[pin_reissue_status]    Script Date: 2015-06-01 10:07:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_reissue_status](
	[pin_reissue_status_id] [bigint] IDENTITY(1,1) NOT NULL,
	[pin_reissue_id] [bigint] NOT NULL,
	[pin_reissue_statuses_id] [int] NOT NULL,
	[status_date] [datetime2](7) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[audit_workstation] [varchar](100) NOT NULL,
	[comments] [varchar](1000) NULL,
 CONSTRAINT [PK_pin_reissue_status] PRIMARY KEY CLUSTERED 
(
	[pin_reissue_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[pin_reissue_status]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_status_pin_reissue] FOREIGN KEY([pin_reissue_id])
REFERENCES [dbo].[pin_reissue] ([pin_reissue_id])
GO

ALTER TABLE [dbo].[pin_reissue_status] CHECK CONSTRAINT [FK_pin_reissue_status_pin_reissue]
GO

ALTER TABLE [dbo].[pin_reissue_status]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_status_pin_reissue_statuses] FOREIGN KEY([pin_reissue_statuses_id])
REFERENCES [dbo].[pin_reissue_statuses] ([pin_reissue_statuses_id])
GO

ALTER TABLE [dbo].[pin_reissue_status] CHECK CONSTRAINT [FK_pin_reissue_status_pin_reissue_statuses]
GO
CREATE TABLE [dbo].[pin_reissue_statuses](
	[pin_reissue_statuses_id] [int] NOT NULL,
	[pin_reissue_statuses_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_pin_reissue_statuses] PRIMARY KEY CLUSTERED 
(
	[pin_reissue_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[pin_reissue_statuses_language]    Script Date: 2015-06-01 10:07:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_reissue_statuses_language](
	[pin_reissue_statuses_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
 CONSTRAINT [PK_pin_reissue_statuses_language] PRIMARY KEY CLUSTERED 
(
	[pin_reissue_statuses_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[pin_reissue_status]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_status_pin_reissue] FOREIGN KEY([pin_reissue_id])
REFERENCES [dbo].[pin_reissue] ([pin_reissue_id])
GO
ALTER TABLE [dbo].[pin_reissue_status] CHECK CONSTRAINT [FK_pin_reissue_status_pin_reissue]
GO
ALTER TABLE [dbo].[pin_reissue_status]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_status_pin_reissue_statuses] FOREIGN KEY([pin_reissue_statuses_id])
REFERENCES [dbo].[pin_reissue_statuses] ([pin_reissue_statuses_id])
GO
ALTER TABLE [dbo].[pin_reissue_status] CHECK CONSTRAINT [FK_pin_reissue_status_pin_reissue_statuses]
GO
ALTER TABLE [dbo].[pin_reissue_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_statuses_language_languages] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[pin_reissue_statuses_language] CHECK CONSTRAINT [FK_pin_reissue_statuses_language_languages]
GO
ALTER TABLE [dbo].[pin_reissue_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_statuses_language_pin_reissue_statuses] FOREIGN KEY([pin_reissue_statuses_id])
REFERENCES [dbo].[pin_reissue_statuses] ([pin_reissue_statuses_id])
GO
ALTER TABLE [dbo].[pin_reissue_statuses_language] CHECK CONSTRAINT [FK_pin_reissue_statuses_language_pin_reissue_statuses]
GO
