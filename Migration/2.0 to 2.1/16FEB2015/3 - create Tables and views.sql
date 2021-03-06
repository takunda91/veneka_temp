USE [indigo_database_main_dev]
GO
/****** Object:  Table [dbo].[masterkeys]    Script Date: 2015-02-23 09:46:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[masterkeys](
	[masterkey_id] [int] IDENTITY(1,1) NOT NULL,
	[masterkey_name] [varchar](250) NOT NULL,
	[masterkey] [varbinary](max) NOT NULL,
	[issuer_id] [int] NOT NULL,
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
/****** Object:  Table [dbo].[pin_mailer_reprint]    Script Date: 2015-02-23 09:46:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_mailer_reprint](
	[pin_mailer_reprint_id] [bigint] IDENTITY(1,1) NOT NULL,
	[card_id] [bigint] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[pin_mailer_reprint_status_id] [int] NOT NULL,
	[status_date] [datetime] NOT NULL,
	[comments] [varchar](1000) NULL,
 CONSTRAINT [PK_pin_mailer_reprint] PRIMARY KEY CLUSTERED 
(
	[pin_mailer_reprint_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[pin_mailer_reprint_statuses]    Script Date: 2015-02-23 09:46:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_mailer_reprint_statuses](
	[pin_mailer_reprint_status_id] [int] NOT NULL,
	[pin_mailer_reprint_status_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_pin_mailer_reprint_statuses] PRIMARY KEY CLUSTERED 
(
	[pin_mailer_reprint_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[pin_mailer_reprint_statuses_language]    Script Date: 2015-02-23 09:46:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pin_mailer_reprint_statuses_language](
	[pin_mailer_reprint_status_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_pin_mailer_reprint_statuses_language] PRIMARY KEY CLUSTERED 
(
	[pin_mailer_reprint_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[terminals]    Script Date: 2015-02-23 09:46:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[terminals](
	[terminal_id] [int] IDENTITY(1,1) NOT NULL,
	[terminal_name] [varchar](250) NOT NULL,
	[terminal_model] [varchar](250) NULL,
	[device_id] [varbinary](max) NOT NULL,
	[branch_id] [int] NOT NULL,
	[terminal_masterkey_id] [int] NOT NULL,
	[workstation] [nvarchar](250) NULL,
	[date_created] [datetime] NULL,
	[date_changed] [datetime] NULL,
 CONSTRAINT [PK_terminals] PRIMARY KEY CLUSTERED 
(
	[terminal_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[pin_mailer_reprint_status_current]    Script Date: 2015-02-23 09:46:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[pin_mailer_reprint_status_current]
AS
SELECT        dbo.pin_mailer_reprint.pin_mailer_reprint_id, dbo.pin_mailer_reprint.card_id, dbo.cards.card_priority_id,
					dbo.cards.product_id, dbo.cards.card_issue_method_id, dbo.cards.branch_id,
						 dbo.pin_mailer_reprint.pin_mailer_reprint_status_id, 
                         dbo.pin_mailer_reprint.status_date, dbo.pin_mailer_reprint.[user_id],
                         dbo.pin_mailer_reprint.comments
FROM         dbo.cards 
				INNER JOIN
				   dbo.pin_mailer_reprint ON dbo.cards.card_id = dbo.pin_mailer_reprint.card_id
WHERE        (dbo.pin_mailer_reprint.status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.pin_mailer_reprint AS bcs2
                               WHERE        (card_id = dbo.pin_mailer_reprint.card_id)))

GO
ALTER TABLE [dbo].[masterkeys]  WITH CHECK ADD  CONSTRAINT [FK_masterkeys_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[masterkeys] CHECK CONSTRAINT [FK_masterkeys_issuer]
GO
ALTER TABLE [dbo].[pin_mailer_reprint]  WITH CHECK ADD  CONSTRAINT [FK_pin_mailer_reprint_cards] FOREIGN KEY([card_id])
REFERENCES [dbo].[cards] ([card_id])
GO
ALTER TABLE [dbo].[pin_mailer_reprint] CHECK CONSTRAINT [FK_pin_mailer_reprint_cards]
GO
ALTER TABLE [dbo].[pin_mailer_reprint]  WITH CHECK ADD  CONSTRAINT [FK_pin_mailer_reprint_pin_mailer_reprint] FOREIGN KEY([pin_mailer_reprint_status_id])
REFERENCES [dbo].[pin_mailer_reprint_statuses] ([pin_mailer_reprint_status_id])
GO
ALTER TABLE [dbo].[pin_mailer_reprint] CHECK CONSTRAINT [FK_pin_mailer_reprint_pin_mailer_reprint]
GO
ALTER TABLE [dbo].[pin_mailer_reprint]  WITH CHECK ADD  CONSTRAINT [FK_pin_mailer_reprint_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[pin_mailer_reprint] CHECK CONSTRAINT [FK_pin_mailer_reprint_user]
GO
ALTER TABLE [dbo].[pin_mailer_reprint_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_pin_mailer_reprint_statuses_language_languages] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[pin_mailer_reprint_statuses_language] CHECK CONSTRAINT [FK_pin_mailer_reprint_statuses_language_languages]
GO
ALTER TABLE [dbo].[pin_mailer_reprint_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_pin_mailer_reprint_statuses_language_pin_mailer_reprint_statuses] FOREIGN KEY([pin_mailer_reprint_status_id])
REFERENCES [dbo].[pin_mailer_reprint_statuses] ([pin_mailer_reprint_status_id])
GO
ALTER TABLE [dbo].[pin_mailer_reprint_statuses_language] CHECK CONSTRAINT [FK_pin_mailer_reprint_statuses_language_pin_mailer_reprint_statuses]
GO
ALTER TABLE [dbo].[terminals]  WITH CHECK ADD  CONSTRAINT [FK_terminals_branch] FOREIGN KEY([branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO
ALTER TABLE [dbo].[terminals] CHECK CONSTRAINT [FK_terminals_branch]
GO
ALTER TABLE [dbo].[terminals]  WITH CHECK ADD  CONSTRAINT [FK_terminals_masterkeys] FOREIGN KEY([terminal_masterkey_id])
REFERENCES [dbo].[masterkeys] ([masterkey_id])
GO
ALTER TABLE [dbo].[terminals] CHECK CONSTRAINT [FK_terminals_masterkeys]
GO
