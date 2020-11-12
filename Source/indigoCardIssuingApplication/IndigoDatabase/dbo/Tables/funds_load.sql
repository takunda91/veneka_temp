USE [indigo_database_2.1.4.0]
GO

/****** Object:  Table [dbo].[funds_load]    Script Date: 2019/10/18 00:05:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[funds_load](
	[funds_load_id] [bigint] IDENTITY(1,1) NOT NULL,
	[bank_account_no] [nvarchar](100) NOT NULL,
	[prepaid_card_no] [nvarchar](100) NOT NULL,
	[prepaid_account_no] [nvarchar](100) NULL,
	[amount] [decimal](18, 2) NOT NULL,
	[status] [int] NOT NULL,
	[creator_id] [int] NOT NULL,
	[create_dated] [datetime] NOT NULL,
	[reviewer_id] [int] NULL,
	[review_date] [datetime] NULL,
	[review_accepted] [bit] NULL,
	[approver_id] [int] NULL,
	[approve_date] [datetime] NULL,
	[approve_accepted] [bit] NULL,
	[loader_id] [int] NULL,
	[load_date] [datetime] NULL,
	[sms_sent_date] [datetime] NULL,
 CONSTRAINT [PK_funds_load] PRIMARY KEY CLUSTERED 
(
	[funds_load_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


