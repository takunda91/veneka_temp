USE [indigo_database_main_dev]
GO
/****** Object:  Table [dbo].[dist_batch_statuses_flow]    Script Date: 2014/09/18 10:11:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[dist_batch_statuses_flow](
	[dist_batch_type_id] [int] NOT NULL,
	[dist_batch_statuses_id] [int] NOT NULL,
	[card_issue_method_id] [int] NOT NULL,
	[user_role_id] [int] NOT NULL,
	[flow_dist_batch_statuses_id] [int] NOT NULL,
	[flow_dist_batch_type_id] [int] NOT NULL,
 CONSTRAINT [PK_dist_batch_statuses_flow] PRIMARY KEY CLUSTERED 
(
	[dist_batch_type_id] ASC,
	[dist_batch_statuses_id] ASC,
	[card_issue_method_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[dist_batch_type]    Script Date: 2014/09/18 10:11:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[dist_batch_type](
	[dist_batch_type_id] [int] NOT NULL,
	[dist_batch_type_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_dist_batch_type] PRIMARY KEY CLUSTERED 
(
	[dist_batch_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
