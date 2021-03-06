USE [indigo_database_main_dev]
GO
/****** Object:  Table [dbo].[dist_batch_statuses_flow]    Script Date: 2015-07-08 11:35:52 AM ******/
DROP TABLE [dbo].[dist_batch_statuses_flow]
GO
/****** Object:  Table [dbo].[dist_batch_statuses_flow]    Script Date: 2015-07-08 11:35:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[dist_batch_statuses_flow](
	[issuer_id] [int] NOT NULL,
	[dist_batch_type_id] [int] NOT NULL,
	[dist_batch_statuses_id] [int] NOT NULL,
	[card_issue_method_id] [int] NOT NULL,
	[user_role_id] [int] NOT NULL,
	[flow_dist_batch_statuses_id] [int] NOT NULL,
	[flow_dist_batch_type_id] [int] NOT NULL,
	[main_menu_id] [smallint] NULL,
	[sub_menu_id] [smallint] NULL,
	[sub_menu_order] [smallint] NULL,
	[reject_dist_batch_statuses_id] [int] NULL,
	[flow_dist_card_statuses_id] [int] NULL,
	[reject_dist_card_statuses_id] [int] NULL,
	[branch_card_statuses_id] [int] NULL,
	[reject_branch_card_statuses_id] [int] NULL,
 CONSTRAINT [PK_dist_batch_statuses_flow] PRIMARY KEY CLUSTERED 
(
	[issuer_id] ASC,
	[dist_batch_type_id] ASC,
	[dist_batch_statuses_id] ASC,
	[card_issue_method_id] ASC,
	[flow_dist_batch_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
