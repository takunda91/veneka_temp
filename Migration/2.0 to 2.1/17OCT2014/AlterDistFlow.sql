USE [indigo_database_main_dev]
GO
/****** Object:  Table [dbo].[dist_batch_statuses_flow]    Script Date: 2014/10/17 12:09:14 PM ******/
DROP TABLE [dbo].[dist_batch_statuses_flow]
GO
/****** Object:  Table [dbo].[dist_batch_statuses_flow]    Script Date: 2014/10/17 12:09:14 PM ******/
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
	[card_issue_method_id] ASC,
	[flow_dist_batch_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (0, 0, 0, 4, 9, 0)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (0, 0, 1, 4, 9, 0)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (0, 1, 1, 5, 14, 0)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (0, 9, 0, 12, 10, 0)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (0, 9, 1, 12, 10, 0)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (0, 10, 0, 12, 11, 0)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (0, 10, 1, 12, 11, 0)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (0, 11, 0, 11, 12, 0)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (0, 11, 1, 4, 1, 0)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (0, 12, 0, 11, 13, 0)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (0, 13, 0, 4, 14, 0)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (1, 0, 0, 4, 1, 1)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (1, 0, 1, 4, 1, 1)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (1, 1, 0, 5, 2, 1)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (1, 1, 1, 5, 2, 1)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (1, 2, 0, 2, 3, 1)
GO
INSERT [dbo].[dist_batch_statuses_flow] ([dist_batch_type_id], [dist_batch_statuses_id], [card_issue_method_id], [user_role_id], [flow_dist_batch_statuses_id], [flow_dist_batch_type_id]) VALUES (1, 2, 1, 2, 3, 1)
GO
