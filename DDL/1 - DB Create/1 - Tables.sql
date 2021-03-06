USE [indigo_database_group]
GO
/****** Object:  Table [dbo].[audit_action]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[audit_action](
	[audit_action_id] [int] NOT NULL,
	[audit_action_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_audit_action] PRIMARY KEY CLUSTERED 
(
	[audit_action_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[audit_action_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[audit_action_language](
	[audit_action_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[audit_action_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[audit_control]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[audit_control](
	[audit_id] [bigint] IDENTITY(1,1) NOT NULL,
	[audit_action_id] [int] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[audit_date] [datetime] NOT NULL,
	[workstation_address] [varchar](100) NOT NULL,
	[action_description] [varchar](max) NULL,
	[issuer_id] [int] NULL,
	[data_changed] [varchar](30) NULL,
	[data_before] [varchar](450) NULL,
	[data_after] [varchar](450) NULL,
 CONSTRAINT [PK_audit_control] PRIMARY KEY CLUSTERED 
(
	[audit_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BLK_ACCOUNTS]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BLK_ACCOUNTS](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ACCOUNT] [nchar](20) NOT NULL,
	[FK_ISSUERID] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[branch]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[branch](
	[branch_id] [int] IDENTITY(1,1) NOT NULL,
	[branch_status_id] [int] NOT NULL,
	[issuer_id] [int] NOT NULL,
	[branch_code] [varchar](10) NOT NULL,
	[branch_name] [varchar](30) NOT NULL,
	[location] [varchar](20) NULL,
	[contact_person] [varchar](30) NULL,
	[contact_email] [varchar](30) NULL,
	[card_centre] [varchar](10) NULL,
 CONSTRAINT [PK_branch] PRIMARY KEY CLUSTERED 
(
	[branch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[branch_card_code_type]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[branch_card_code_type](
	[branch_card_code_type_id] [int] NOT NULL,
	[branch_card_code_name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_branch_card_code_type] PRIMARY KEY CLUSTERED 
(
	[branch_card_code_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[branch_card_codes]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[branch_card_codes](
	[branch_card_code_id] [int] NOT NULL,
	[branch_card_code_type_id] [int] NOT NULL,
	[branch_card_code_name] [varchar](50) NOT NULL,
	[branch_card_code_enabled] [bit] NOT NULL,
	[spoil_only] [bit] NOT NULL,
	[is_exception] [bit] NOT NULL,
 CONSTRAINT [PK_branch_card_codes] PRIMARY KEY CLUSTERED 
(
	[branch_card_code_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_branch_card_codes] UNIQUE NONCLUSTERED 
(
	[branch_card_code_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[branch_card_codes_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[branch_card_codes_language](
	[branch_card_code_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](1000) NULL,
 CONSTRAINT [PK_branch_card_codes_language] PRIMARY KEY CLUSTERED 
(
	[language_id] ASC,
	[branch_card_code_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[branch_card_status]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[branch_card_status](
	[branch_card_status_id] [bigint] IDENTITY(1,1) NOT NULL,
	[card_id] [bigint] NOT NULL,
	[branch_card_statuses_id] [int] NOT NULL,
	[status_date] [datetime] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[operator_user_id] [bigint] NULL,
	[branch_card_code_id] [int] NULL,
	[comments] [varchar](1000) NULL,
 CONSTRAINT [PK_branch_card_status] PRIMARY KEY CLUSTERED 
(
	[branch_card_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[branch_card_statuses]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[branch_card_statuses](
	[branch_card_statuses_id] [int] NOT NULL,
	[branch_card_statuses_name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_branch_card_statuses] PRIMARY KEY CLUSTERED 
(
	[branch_card_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[branch_card_statuses_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[branch_card_statuses_language](
	[branch_card_statuses_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[branch_card_statuses_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[branch_statuses]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[branch_statuses](
	[branch_status_id] [int] NOT NULL,
	[branch_status] [varchar](15) NOT NULL,
 CONSTRAINT [PK_branch_status] PRIMARY KEY CLUSTERED 
(
	[branch_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[branch_statuses_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[branch_statuses_language](
	[branch_status_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[branch_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[card_issue_reason]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[card_issue_reason](
	[card_issue_reason_id] [int] NOT NULL,
	[card_issuer_reason_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_card_issue_reason] PRIMARY KEY CLUSTERED 
(
	[card_issue_reason_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[card_issue_reason_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[card_issue_reason_language](
	[card_issue_reason_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[card_issue_reason_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[cards]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cards](
	[card_id] [bigint] IDENTITY(1,1) NOT NULL,
	[product_id] [int] NOT NULL,
	[branch_id] [int] NOT NULL,
	[card_number] [varbinary](max) NOT NULL,
	[card_sequence] [int] NOT NULL,
	[card_index] [varbinary](20) NOT NULL,
 CONSTRAINT [PK_cards] PRIMARY KEY CLUSTERED 
(
	[card_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[cms_connection_config]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cms_connection_config](
	[connection_ip] [varchar](15) NOT NULL,
	[connection_port] [varchar](6) NOT NULL,
	[sign_on] [varchar](5) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[connection_parameters]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[connection_parameters](
	[connection_parameter_id] [int] IDENTITY(1,1) NOT NULL,
	[connection_name] [varchar](100) NOT NULL,
	[address] [varchar](200) NOT NULL,
	[port] [int] NOT NULL,
	[path] [varchar](200) NOT NULL,
	[protocol] [varchar](50) NOT NULL,
	[auth_type] [int] NOT NULL,
	[username] [varbinary](max) NOT NULL,
	[password] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_connection_parameters] PRIMARY KEY CLUSTERED 
(
	[connection_parameter_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[country]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[country](
	[country_id] [int] IDENTITY(1,1) NOT NULL,
	[country_name] [varchar](100) NOT NULL,
	[country_code] [varchar](3) NOT NULL,
	[country_capital_city] [varchar](100) NULL,
 CONSTRAINT [PK_country] PRIMARY KEY CLUSTERED 
(
	[country_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[currency]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[currency](
	[currency_id] [int] NOT NULL,
	[currency_code] [char](3) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[currency_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[customer_account]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_account](
	[customer_account_id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[card_id] [bigint] NOT NULL,
	[card_issue_reason_id] [int] NOT NULL,
	[account_type_id] [int] NOT NULL,
	[currency_id] [int] NOT NULL,
	[resident_id] [int] NOT NULL,
	[customer_type_id] [int] NOT NULL,
	[customer_account_number] [varbinary](max) NOT NULL,
	[customer_first_name] [varbinary](max) NOT NULL,
	[customer_middle_name] [varbinary](max) NOT NULL,
	[customer_last_name] [varbinary](max) NOT NULL,
	[name_on_card] [varbinary](max) NOT NULL,
	[date_issued] [datetime] NOT NULL,
	[cms_id] [varchar](50) NULL,
	[contract_number] [varchar](50) NULL,
	[customer_title_id] [int] NULL,
 CONSTRAINT [PK_customer_account] PRIMARY KEY CLUSTERED 
(
	[customer_account_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_card_id] UNIQUE NONCLUSTERED 
(
	[card_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[customer_account_type]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_account_type](
	[account_type_id] [int] NOT NULL,
	[account_type_name] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[account_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[customer_account_type_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_account_type_language](
	[account_type_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[account_type_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[customer_residency]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_residency](
	[resident_id] [int] NOT NULL,
	[residency_name] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[resident_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[customer_residency_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_residency_language](
	[resident_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[resident_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[customer_title]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_title](
	[customer_title_id] [int] NOT NULL,
	[customer_title_name] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[customer_title_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[customer_title_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_title_language](
	[customer_title_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[customer_title_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[customer_type]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_type](
	[customer_type_id] [int] NOT NULL,
	[customer_type_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_customer_account_type] PRIMARY KEY CLUSTERED 
(
	[customer_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[customer_type_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_type_language](
	[customer_type_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[customer_type_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[debug_table]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[debug_table](
	[comment] [nchar](30) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[dist_batch]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[dist_batch](
	[dist_batch_id] [bigint] IDENTITY(1,1) NOT NULL,
	[branch_id] [int] NOT NULL,
	[no_cards] [int] NOT NULL,
	[date_created] [datetime] NOT NULL,
	[dist_batch_reference] [varchar](25) NOT NULL,
 CONSTRAINT [PK_distribution_batch] PRIMARY KEY CLUSTERED 
(
	[dist_batch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[dist_batch_cards]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[dist_batch_cards](
	[dist_batch_id] [bigint] NOT NULL,
	[card_id] [bigint] NOT NULL,
	[dist_card_status_id] [int] NOT NULL,
 CONSTRAINT [PK_dist_batch_cards] PRIMARY KEY CLUSTERED 
(
	[dist_batch_id] ASC,
	[card_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[dist_batch_status]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[dist_batch_status](
	[dist_batch_status_id] [bigint] IDENTITY(1,1) NOT NULL,
	[dist_batch_id] [bigint] NOT NULL,
	[dist_batch_statuses_id] [int] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[status_date] [datetime] NOT NULL,
	[status_notes] [varchar](150) NULL,
 CONSTRAINT [PK_dist_batch_status] PRIMARY KEY CLUSTERED 
(
	[dist_batch_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[dist_batch_statuses]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[dist_batch_statuses](
	[dist_batch_statuses_id] [int] NOT NULL,
	[dist_batch_status_name] [varchar](50) NOT NULL,
	[dist_batch_expected_statuses_id] [int] NULL,
 CONSTRAINT [PK_dist_batch_statuses] PRIMARY KEY CLUSTERED 
(
	[dist_batch_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[dist_batch_statuses_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[dist_batch_statuses_language](
	[dist_batch_statuses_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[dist_batch_statuses_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[dist_card_statuses]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[dist_card_statuses](
	[dist_card_status_id] [int] NOT NULL,
	[dist_card_status_name] [varchar](30) NOT NULL,
 CONSTRAINT [PK_dist_card_statuses] PRIMARY KEY CLUSTERED 
(
	[dist_card_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[dist_card_statuses_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[dist_card_statuses_language](
	[dist_card_status_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[dist_card_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[file_history]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[file_history](
	[file_id] [bigint] IDENTITY(1,1) NOT NULL,
	[issuer_id] [int] NULL,
	[file_status_id] [int] NOT NULL,
	[file_type_id] [int] NOT NULL,
	[name_of_file] [varchar](60) NOT NULL,
	[file_created_date] [datetime] NOT NULL,
	[file_size] [int] NOT NULL,
	[load_date] [datetime] NOT NULL,
	[file_directory] [varchar](110) NOT NULL,
	[number_successful_records] [int] NULL,
	[number_failed_records] [int] NULL,
	[file_load_comments] [varchar](max) NULL,
	[file_load_id] [int] NOT NULL,
 CONSTRAINT [PK_file_history] PRIMARY KEY CLUSTERED 
(
	[file_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[file_load]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[file_load](
	[file_load_id] [int] IDENTITY(1,1) NOT NULL,
	[file_load_start] [datetime] NOT NULL,
	[file_load_end] [datetime] NULL,
	[user_id] [int] NOT NULL,
	[files_to_process] [int] NOT NULL,
 CONSTRAINT [PK_file_load] PRIMARY KEY CLUSTERED 
(
	[file_load_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[file_statuses]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[file_statuses](
	[file_status_id] [int] NOT NULL,
	[file_status] [varchar](50) NOT NULL,
 CONSTRAINT [PK_file_statuses] PRIMARY KEY CLUSTERED 
(
	[file_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[file_statuses_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[file_statuses_language](
	[file_status_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[file_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[file_types]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[file_types](
	[file_type_id] [int] NOT NULL,
	[file_type] [varchar](15) NOT NULL,
 CONSTRAINT [PK_file_types] PRIMARY KEY CLUSTERED 
(
	[file_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[flex_affiliate_codes]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[flex_affiliate_codes](
	[issuer_id] [int] NOT NULL,
	[affiliate_code] [varchar](15) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[flex_parameters]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[flex_parameters](
	[flex_parameter_id] [int] IDENTITY(1,1) NOT NULL,
	[source_code] [varchar](10) NOT NULL,
	[request_id] [bigint] NOT NULL,
	[request_token] [varchar](100) NOT NULL,
	[request_type] [varchar](20) NOT NULL,
	[source_channel_id] [varchar](100) NOT NULL,
 CONSTRAINT [PK_flex_parameters] PRIMARY KEY CLUSTERED 
(
	[flex_parameter_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[flex_response_values]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[flex_response_values](
	[flex_response_value_id] [int] NOT NULL,
	[flex_response_id] [int] NOT NULL,
	[flex_response_value] [varchar](100) NOT NULL,
	[valid_response] [bit] NOT NULL,
 CONSTRAINT [PK_flex_response_values] PRIMARY KEY CLUSTERED 
(
	[flex_response_value_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[flex_response_values_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[flex_response_values_language](
	[flex_response_value_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[flex_response_value_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[flex_responses]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[flex_responses](
	[flex_response_id] [int] IDENTITY(1,1) NOT NULL,
	[flex_response_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_flex_responses] PRIMARY KEY CLUSTERED 
(
	[flex_response_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[interface_type]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[interface_type](
	[interface_type_id] [int] NOT NULL,
	[interface_type_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_interface_type] PRIMARY KEY CLUSTERED 
(
	[interface_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[interface_type_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[interface_type_language](
	[interface_type_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[interface_type_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[issuer]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[issuer](
	[issuer_id] [int] IDENTITY(1,1) NOT NULL,
	[issuer_status_id] [int] NOT NULL,
	[country_id] [int] NOT NULL,
	[issuer_name] [varchar](50) NOT NULL,
	[issuer_code] [varchar](10) NOT NULL,
	[auto_create_dist_batch] [bit] NOT NULL,
	[instant_card_issue_YN] [bit] NOT NULL,
	[pin_mailer_printing_YN] [bit] NOT NULL,
	[delete_pin_file_YN] [bit] NOT NULL,
	[delete_card_file_YN] [bit] NOT NULL,
	[account_validation_YN] [bit] NOT NULL,
	[maker_checker_YN] [bit] NOT NULL,
	[license_file] [varchar](100) NULL,
	[license_key] [varchar](1000) NULL,
	[cards_file_location] [varchar](100) NULL,
	[card_file_type] [varchar](20) NULL,
	[pin_file_location] [varchar](100) NULL,
	[pin_encrypted_ZPK] [varchar](40) NULL,
	[pin_mailer_file_type] [varchar](20) NULL,
	[pin_printer_name] [varchar](50) NULL,
	[pin_encrypted_PWK] [varchar](40) NULL,
	[language_id] [int] NULL,
 CONSTRAINT [PK_issuer] PRIMARY KEY CLUSTERED 
(
	[issuer_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[issuer_interface]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[issuer_interface](
	[interface_type_id] [int] NOT NULL,
	[issuer_id] [int] NOT NULL,
	[connection_parameter_id] [int] NOT NULL,
 CONSTRAINT [PK_issuer_interface] PRIMARY KEY CLUSTERED 
(
	[interface_type_id] ASC,
	[issuer_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[issuer_product]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[issuer_product](
	[product_id] [int] IDENTITY(1,1) NOT NULL,
	[product_code] [varchar](50) NOT NULL,
	[product_name] [varchar](100) NOT NULL,
	[product_bin_code] [varchar](15) NOT NULL,
	[issuer_id] [int] NOT NULL,
	[name_on_card_top] [decimal](8, 2) NULL,
	[name_on_card_left] [decimal](8, 2) NULL,
	[Name_on_card_font_size] [int] NULL,
	[font_id] [int] NULL,
	[DeletedYN] [bit] NULL,
 CONSTRAINT [PK_issuer_product] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Issuer_product_font]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Issuer_product_font](
	[font_id] [int] NOT NULL,
	[font_name] [varchar](50) NOT NULL,
	[resource_path] [varchar](200) NULL,
	[DeletedYN] [bit] NULL,
 CONSTRAINT [PK_Issuer_product_font] PRIMARY KEY CLUSTERED 
(
	[font_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[issuer_statuses]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[issuer_statuses](
	[issuer_status_id] [int] NOT NULL,
	[issuer_status_name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_issuer_statuses] PRIMARY KEY CLUSTERED 
(
	[issuer_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[issuer_statuses_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[issuer_statuses_language](
	[issuer_status_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[issuer_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[languages]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[languages](
	[id] [int] NOT NULL,
	[language_name] [nvarchar](max) NOT NULL,
	[language_name_fr] [varchar](100) NOT NULL,
	[language_name_pt] [varchar](100) NOT NULL,
	[language_name_sp] [varchar](100) NOT NULL,
 CONSTRAINT [PK_languages] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ldap_setting]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ldap_setting](
	[ldap_setting_id] [int] IDENTITY(1,1) NOT NULL,
	[ldap_setting_name] [varchar](100) NOT NULL,
	[hostname_or_ip] [varchar](200) NOT NULL,
	[path] [varchar](200) NOT NULL,
	[domain_name] [varchar](100) NULL,
	[username] [varbinary](max) NULL,
	[password] [varbinary](max) NULL,
 CONSTRAINT [PK_ldap_setting] PRIMARY KEY CLUSTERED 
(
	[ldap_setting_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[load_batch]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[load_batch](
	[load_batch_id] [bigint] IDENTITY(1,1) NOT NULL,
	[file_id] [bigint] NOT NULL,
	[load_batch_status_id] [int] NOT NULL,
	[no_cards] [int] NOT NULL,
	[load_date] [datetime] NOT NULL,
	[load_batch_reference] [varchar](100) NOT NULL,
 CONSTRAINT [PK_load_batch] PRIMARY KEY CLUSTERED 
(
	[load_batch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[load_batch_cards]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[load_batch_cards](
	[load_batch_id] [bigint] NOT NULL,
	[card_id] [bigint] NOT NULL,
	[load_card_status_id] [int] NOT NULL,
 CONSTRAINT [PK_load_batch_cards] PRIMARY KEY CLUSTERED 
(
	[load_batch_id] ASC,
	[card_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[load_batch_status]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[load_batch_status](
	[load_batch_status_id] [bigint] IDENTITY(1,1) NOT NULL,
	[load_batch_id] [bigint] NOT NULL,
	[load_batch_statuses_id] [int] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[status_date] [datetime] NOT NULL,
	[status_notes] [varchar](150) NULL,
 CONSTRAINT [PK_load_batch_status] PRIMARY KEY CLUSTERED 
(
	[load_batch_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[load_batch_statuses]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[load_batch_statuses](
	[load_batch_statuses_id] [int] NOT NULL,
	[load_batch_status_name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_batch_statuses] PRIMARY KEY CLUSTERED 
(
	[load_batch_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[load_batch_statuses_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[load_batch_statuses_language](
	[load_batch_statuses_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[load_batch_statuses_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[load_card_failed]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[load_card_failed](
	[failed_card_id] [bigint] IDENTITY(1,1) NOT NULL,
	[card_number] [varchar](19) NOT NULL,
	[card_sequence] [nchar](13) NOT NULL,
	[load_batch_reference] [varchar](25) NOT NULL,
	[card_status] [varchar](25) NOT NULL,
	[load_batch_id] [bigint] NOT NULL,
 CONSTRAINT [PK_load_card_failed] PRIMARY KEY CLUSTERED 
(
	[failed_card_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[load_card_statuses]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[load_card_statuses](
	[load_card_status_id] [int] NOT NULL,
	[load_card_status] [varchar](15) NOT NULL,
 CONSTRAINT [PK_load_card_statuses] PRIMARY KEY CLUSTERED 
(
	[load_card_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[load_card_statuses_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[load_card_statuses_language](
	[load_card_status_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[load_card_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[mac_index_keys]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[mac_index_keys](
	[table_id] [int] NOT NULL,
	[mac_key] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_mac_index_keys] PRIMARY KEY CLUSTERED 
(
	[table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[message_parameters]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[message_parameters](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[interface_message] [nvarchar](max) NOT NULL,
	[indigo_message] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_message_parameters] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[message_parameters_has_interface]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[message_parameters_has_interface](
	[message_parameters_id] [int] NOT NULL,
	[interfaces_id] [int] NOT NULL,
 CONSTRAINT [PK_message_parameters_has_interface] PRIMARY KEY NONCLUSTERED 
(
	[message_parameters_id] ASC,
	[interfaces_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[mod_interface_account_params]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[mod_interface_account_params](
	[BANK_C] [varchar](2) NOT NULL,
	[GROUPC] [varchar](2) NOT NULL,
	[issuer_id] [int] NOT NULL,
	[STAT_CHANGE] [char](1) NOT NULL,
	[LIM_INTR] [decimal](6, 2) NOT NULL,
	[NON_REDUCE_BAL] [decimal](12, 0) NOT NULL,
	[CRD] [decimal](12, 0) NOT NULL,
	[CYCLE] [varchar](50) NOT NULL,
	[DEST_ACCNT_TYPE] [int] NOT NULL,
	[REP_LANG] [char](1) NOT NULL,
 CONSTRAINT [PK_mod_interface_account_params] PRIMARY KEY CLUSTERED 
(
	[BANK_C] ASC,
	[GROUPC] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[mod_interface_cond_accnt]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[mod_interface_cond_accnt](
	[product_id] [int] NOT NULL,
	[CCY] [varchar](50) NOT NULL,
	[COND_SET] [varchar](50) NOT NULL,
 CONSTRAINT [PK_mod_interface_cond_accnt] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC,
	[CCY] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[mod_interface_general]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[mod_interface_general](
	[ID] [int] NOT NULL,
	[requestID] [varchar](50) NULL,
 CONSTRAINT [PK_mod_interface_general] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[mod_response_mapping]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[mod_response_mapping](
	[response_mapping_id] [int] NOT NULL,
	[branch_card_code_id] [int] NOT NULL,
	[response_contains] [varchar](400) NOT NULL,
 CONSTRAINT [PK_mod_response_mapping] PRIMARY KEY CLUSTERED 
(
	[response_mapping_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[pin_batch]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_batch](
	[batch_reference] [varchar](50) NOT NULL,
	[loaded_date] [datetime] NOT NULL,
	[card_count] [int] NOT NULL,
	[issuer_ID] [int] NOT NULL,
	[manager_comment] [varchar](100) NULL,
	[batch_status] [varchar](25) NOT NULL,
	[operator_comment] [varchar](100) NULL,
	[branch_code] [varchar](20) NULL,
 CONSTRAINT [PK_pin_batch] PRIMARY KEY CLUSTERED 
(
	[batch_reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[pin_batch_status]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_batch_status](
	[batch_reference] [varchar](50) NOT NULL,
	[batch_status] [varchar](25) NOT NULL,
	[status_date] [datetime] NOT NULL,
	[application_user] [varchar](25) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[pin_mailer]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_mailer](
	[pin_mailer_reference] [varchar](50) NULL,
	[batch_reference] [varchar](50) NOT NULL,
	[pin_mailer_status] [varchar](25) NOT NULL,
	[card_number] [varchar](19) NOT NULL,
	[pvv_offset] [varchar](4) NULL,
	[encrypted_pin] [varchar](25) NULL,
	[customer_name] [varchar](25) NOT NULL,
	[customer_address] [varchar](50) NULL,
	[printed_date] [datetime] NULL,
	[reprinted_date] [datetime] NULL,
	[reprint_request_YN] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[product_currency]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product_currency](
	[product_id] [int] NOT NULL,
	[currency_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[product_id] ASC,
	[currency_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[report_fields]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[report_fields](
	[reportfieldid] [int] NOT NULL,
	[reportfieldname] [nvarchar](50) NULL,
 CONSTRAINT [PK_report_fields_1] PRIMARY KEY CLUSTERED 
(
	[reportfieldid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[report_reportfields]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[report_reportfields](
	[reportid] [int] NOT NULL,
	[reportfieldid] [int] NOT NULL,
 CONSTRAINT [PK_report_fields] PRIMARY KEY CLUSTERED 
(
	[reportid] ASC,
	[reportfieldid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[reportfields_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[reportfields_language](
	[reportfieldid] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_reportfields_language] PRIMARY KEY CLUSTERED 
(
	[reportfieldid] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[reports]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[reports](
	[Reportid] [int] NOT NULL,
	[ReportName] [nvarchar](50) NULL,
 CONSTRAINT [PK_Reports] PRIMARY KEY CLUSTERED 
(
	[Reportid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[response_messages]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[response_messages](
	[system_response_code] [int] NOT NULL,
	[system_area] [int] NOT NULL,
	[english_response] [varchar](500) NOT NULL,
	[french_response] [varchar](500) NOT NULL,
	[portuguese_response] [varchar](500) NOT NULL,
	[spanish_response] [varchar](500) NOT NULL,
 CONSTRAINT [PK_response_messages] PRIMARY KEY CLUSTERED 
(
	[system_response_code] ASC,
	[system_area] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[system_parameters]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[system_parameters](
	[config_id] [int] IDENTITY(1,1) NOT NULL,
	[config] [varchar](15) NOT NULL,
	[value] [varchar](30) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TEMP_dist_cards]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TEMP_dist_cards](
	[card_id] [bigint] IDENTITY(1,1) NOT NULL,
	[card_number] [varbinary](max) NOT NULL,
	[seqeunce] [varchar](3) NULL,
	[dist_batch_reference] [varchar](25) NOT NULL,
	[card_status] [varchar](25) NOT NULL,
	[date_issued] [datetime] NULL,
	[issued_by] [varchar](20) NULL,
	[customer_first_name] [varchar](20) NULL,
	[customer_last_name] [varchar](20) NULL,
	[customer_account] [varbinary](max) NULL,
	[account_type] [varchar](10) NULL,
	[name_on_card] [varchar](30) NULL,
	[issuer_id] [int] NOT NULL,
	[branch_code] [varchar](10) NOT NULL,
	[reason_for_issue] [varchar](50) NULL,
	[customer_title] [varchar](10) NULL,
	[assigned_operator] [varchar](30) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[user]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user](
	[user_id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_status_id] [int] NOT NULL,
	[user_gender_id] [int] NOT NULL,
	[username] [varbinary](256) NOT NULL,
	[first_name] [varbinary](256) NOT NULL,
	[last_name] [varbinary](256) NOT NULL,
	[password] [varbinary](256) NOT NULL,
	[user_email] [varchar](100) NOT NULL,
	[online] [bit] NOT NULL CONSTRAINT [DF_user_online]  DEFAULT ((0)),
	[employee_id] [varbinary](256) NULL,
	[last_login_date] [datetime] NULL,
	[last_login_attempt] [datetime] NULL,
	[number_of_incorrect_logins] [int] NULL,
	[last_password_changed_date] [datetime] NULL,
	[workstation] [nchar](50) NULL,
	[language_id] [int] NULL,
	[username_index] [varbinary](20) NULL,
	[ldap_setting_id] [int] NULL,
 CONSTRAINT [PK_application_user] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[user_gender]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user_gender](
	[user_gender_id] [int] IDENTITY(1,1) NOT NULL,
	[user_gender_text] [varchar](15) NOT NULL,
 CONSTRAINT [PK_user_gender] PRIMARY KEY CLUSTERED 
(
	[user_gender_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[user_group]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user_group](
	[user_group_id] [int] IDENTITY(1,1) NOT NULL,
	[user_role_id] [int] NOT NULL,
	[issuer_id] [int] NOT NULL,
	[can_create] [bit] NOT NULL CONSTRAINT [DF_user_group_can_create]  DEFAULT ((0)),
	[can_read] [bit] NOT NULL CONSTRAINT [DF_user_group_can_read]  DEFAULT ((0)),
	[can_update] [bit] NOT NULL CONSTRAINT [DF_user_group_can_update]  DEFAULT ((0)),
	[can_delete] [bit] NOT NULL CONSTRAINT [DF_user_group_can_delete]  DEFAULT ((0)),
	[all_branch_access] [bit] NOT NULL CONSTRAINT [DF_user_group_all_branch_access]  DEFAULT ((0)),
	[user_group_name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_user_group] PRIMARY KEY CLUSTERED 
(
	[user_group_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[user_groups_branches]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_groups_branches](
	[user_group_id] [int] NOT NULL,
	[branch_id] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[user_password_history]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user_password_history](
	[user_id] [bigint] NOT NULL,
	[password_history] [varbinary](max) NOT NULL,
	[date_changed] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[user_roles]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user_roles](
	[user_role_id] [int] NOT NULL,
	[user_role] [varchar](50) NOT NULL,
	[allow_multiple_login] [bit] NOT NULL,
	[enterprise_only] [bit] NOT NULL,
 CONSTRAINT [PK_user_roles] PRIMARY KEY CLUSTERED 
(
	[user_role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[user_roles_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user_roles_language](
	[user_role_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[user_role_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[user_status]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user_status](
	[user_status_id] [int] NOT NULL,
	[user_status_text] [varchar](20) NOT NULL,
 CONSTRAINT [PK_user_status] PRIMARY KEY CLUSTERED 
(
	[user_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[user_status_language]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user_status_language](
	[user_status_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[user_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[users_to_users_groups]    Script Date: 2014/07/31 04:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users_to_users_groups](
	[user_id] [bigint] NOT NULL,
	[user_group_id] [int] NOT NULL
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[masterkeys]    Script Date: 2015/01/28 11:38:32 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[masterkeys](
	[masterkey_id] [int] IDENTITY(1,1) NOT NULL,
	[masterkey] [varbinary](max) NOT NULL,
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
/****** Object:  Table [dbo].[terminals]    Script Date: 2015/01/28 11:46:22 AM ******/
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
	[session_key] [varbinary](max) NULL,
	[device_id] [varbinary](max) NULL,
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

ALTER TABLE [dbo].[audit_action_language]  WITH CHECK ADD FOREIGN KEY([audit_action_id])
REFERENCES [dbo].[audit_action] ([audit_action_id])
GO
ALTER TABLE [dbo].[audit_action_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[audit_control]  WITH NOCHECK ADD  CONSTRAINT [FK_audit_control_audit_action] FOREIGN KEY([audit_action_id])
REFERENCES [dbo].[audit_action] ([audit_action_id])
GO
ALTER TABLE [dbo].[audit_control] CHECK CONSTRAINT [FK_audit_control_audit_action]
GO
ALTER TABLE [dbo].[audit_control]  WITH NOCHECK ADD  CONSTRAINT [FK_audit_control_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[audit_control] CHECK CONSTRAINT [FK_audit_control_user]
GO
ALTER TABLE [dbo].[branch]  WITH CHECK ADD  CONSTRAINT [FK_branch_branch_statuses] FOREIGN KEY([branch_status_id])
REFERENCES [dbo].[branch_statuses] ([branch_status_id])
GO
ALTER TABLE [dbo].[branch] CHECK CONSTRAINT [FK_branch_branch_statuses]
GO
ALTER TABLE [dbo].[branch]  WITH CHECK ADD  CONSTRAINT [FK_branch_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[branch] CHECK CONSTRAINT [FK_branch_issuer]
GO
ALTER TABLE [dbo].[branch_card_codes]  WITH CHECK ADD  CONSTRAINT [FK_branch_card_codes_branch_card_code_type] FOREIGN KEY([branch_card_code_type_id])
REFERENCES [dbo].[branch_card_code_type] ([branch_card_code_type_id])
GO
ALTER TABLE [dbo].[branch_card_codes] CHECK CONSTRAINT [FK_branch_card_codes_branch_card_code_type]
GO
ALTER TABLE [dbo].[branch_card_codes_language]  WITH CHECK ADD  CONSTRAINT [FK_branch_card_codes_language_branch_card_codes] FOREIGN KEY([branch_card_code_id])
REFERENCES [dbo].[branch_card_codes] ([branch_card_code_id])
GO
ALTER TABLE [dbo].[branch_card_codes_language] CHECK CONSTRAINT [FK_branch_card_codes_language_branch_card_codes]
GO
ALTER TABLE [dbo].[branch_card_codes_language]  WITH CHECK ADD  CONSTRAINT [FK_branch_card_codes_language_languages] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[branch_card_codes_language] CHECK CONSTRAINT [FK_branch_card_codes_language_languages]
GO
ALTER TABLE [dbo].[branch_card_status]  WITH CHECK ADD  CONSTRAINT [FK_branch_card_status_branch_card_codes] FOREIGN KEY([branch_card_code_id])
REFERENCES [dbo].[branch_card_codes] ([branch_card_code_id])
GO
ALTER TABLE [dbo].[branch_card_status] CHECK CONSTRAINT [FK_branch_card_status_branch_card_codes]
GO
ALTER TABLE [dbo].[branch_card_status]  WITH CHECK ADD  CONSTRAINT [FK_branch_card_status_branch_card_statuses] FOREIGN KEY([branch_card_statuses_id])
REFERENCES [dbo].[branch_card_statuses] ([branch_card_statuses_id])
GO
ALTER TABLE [dbo].[branch_card_status] CHECK CONSTRAINT [FK_branch_card_status_branch_card_statuses]
GO
ALTER TABLE [dbo].[branch_card_status]  WITH CHECK ADD  CONSTRAINT [FK_branch_card_status_cards] FOREIGN KEY([card_id])
REFERENCES [dbo].[cards] ([card_id])
GO
ALTER TABLE [dbo].[branch_card_status] CHECK CONSTRAINT [FK_branch_card_status_cards]
GO
ALTER TABLE [dbo].[branch_card_status]  WITH CHECK ADD  CONSTRAINT [FK_branch_card_status_user] FOREIGN KEY([operator_user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[branch_card_status] CHECK CONSTRAINT [FK_branch_card_status_user]
GO
ALTER TABLE [dbo].[branch_card_status]  WITH CHECK ADD  CONSTRAINT [FK_branch_card_status_user1] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[branch_card_status] CHECK CONSTRAINT [FK_branch_card_status_user1]
GO
ALTER TABLE [dbo].[branch_card_statuses_language]  WITH CHECK ADD FOREIGN KEY([branch_card_statuses_id])
REFERENCES [dbo].[branch_card_statuses] ([branch_card_statuses_id])
GO
ALTER TABLE [dbo].[branch_card_statuses_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[branch_statuses_language]  WITH CHECK ADD FOREIGN KEY([branch_status_id])
REFERENCES [dbo].[branch_statuses] ([branch_status_id])
GO
ALTER TABLE [dbo].[branch_statuses_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[card_issue_reason_language]  WITH CHECK ADD FOREIGN KEY([card_issue_reason_id])
REFERENCES [dbo].[card_issue_reason] ([card_issue_reason_id])
GO
ALTER TABLE [dbo].[card_issue_reason_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[cards]  WITH CHECK ADD  CONSTRAINT [FK_cards_cards] FOREIGN KEY([branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO
ALTER TABLE [dbo].[cards] CHECK CONSTRAINT [FK_cards_cards]
GO
ALTER TABLE [dbo].[cards]  WITH CHECK ADD  CONSTRAINT [FK_cards_issuer_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO
ALTER TABLE [dbo].[cards] CHECK CONSTRAINT [FK_cards_issuer_product]
GO
ALTER TABLE [dbo].[customer_account]  WITH CHECK ADD FOREIGN KEY([customer_title_id])
REFERENCES [dbo].[customer_title] ([customer_title_id])
GO
ALTER TABLE [dbo].[customer_account]  WITH CHECK ADD  CONSTRAINT [FK_customer_account_card_issue_reason] FOREIGN KEY([card_issue_reason_id])
REFERENCES [dbo].[card_issue_reason] ([card_issue_reason_id])
GO
ALTER TABLE [dbo].[customer_account] CHECK CONSTRAINT [FK_customer_account_card_issue_reason]
GO
ALTER TABLE [dbo].[customer_account]  WITH CHECK ADD  CONSTRAINT [FK_customer_account_cards] FOREIGN KEY([card_id])
REFERENCES [dbo].[cards] ([card_id])
GO
ALTER TABLE [dbo].[customer_account] CHECK CONSTRAINT [FK_customer_account_cards]
GO
ALTER TABLE [dbo].[customer_account]  WITH CHECK ADD  CONSTRAINT [FK_customer_account_type] FOREIGN KEY([account_type_id])
REFERENCES [dbo].[customer_account_type] ([account_type_id])
GO
ALTER TABLE [dbo].[customer_account] CHECK CONSTRAINT [FK_customer_account_type]
GO
ALTER TABLE [dbo].[customer_account]  WITH CHECK ADD  CONSTRAINT [FK_customer_account_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[customer_account] CHECK CONSTRAINT [FK_customer_account_user]
GO
ALTER TABLE [dbo].[customer_account]  WITH CHECK ADD  CONSTRAINT [FK_customer_residency] FOREIGN KEY([resident_id])
REFERENCES [dbo].[customer_residency] ([resident_id])
GO
ALTER TABLE [dbo].[customer_account] CHECK CONSTRAINT [FK_customer_residency]
GO
ALTER TABLE [dbo].[customer_account]  WITH CHECK ADD  CONSTRAINT [FK_customer_type] FOREIGN KEY([customer_type_id])
REFERENCES [dbo].[customer_type] ([customer_type_id])
GO
ALTER TABLE [dbo].[customer_account] CHECK CONSTRAINT [FK_customer_type]
GO
ALTER TABLE [dbo].[customer_account_type_language]  WITH CHECK ADD FOREIGN KEY([account_type_id])
REFERENCES [dbo].[customer_account_type] ([account_type_id])
GO
ALTER TABLE [dbo].[customer_account_type_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[customer_residency_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[customer_residency_language]  WITH CHECK ADD FOREIGN KEY([resident_id])
REFERENCES [dbo].[customer_residency] ([resident_id])
GO
ALTER TABLE [dbo].[customer_title_language]  WITH CHECK ADD FOREIGN KEY([customer_title_id])
REFERENCES [dbo].[customer_title] ([customer_title_id])
GO
ALTER TABLE [dbo].[customer_title_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[customer_type_language]  WITH CHECK ADD FOREIGN KEY([customer_type_id])
REFERENCES [dbo].[customer_type] ([customer_type_id])
GO
ALTER TABLE [dbo].[customer_type_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[dist_batch]  WITH CHECK ADD  CONSTRAINT [FK_distribution_batch_branch] FOREIGN KEY([branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO
ALTER TABLE [dbo].[dist_batch] CHECK CONSTRAINT [FK_distribution_batch_branch]
GO
ALTER TABLE [dbo].[dist_batch_cards]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_cards_cards] FOREIGN KEY([card_id])
REFERENCES [dbo].[cards] ([card_id])
GO
ALTER TABLE [dbo].[dist_batch_cards] CHECK CONSTRAINT [FK_dist_batch_cards_cards]
GO
ALTER TABLE [dbo].[dist_batch_cards]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_cards_dist_card_statuses] FOREIGN KEY([dist_card_status_id])
REFERENCES [dbo].[dist_card_statuses] ([dist_card_status_id])
GO
ALTER TABLE [dbo].[dist_batch_cards] CHECK CONSTRAINT [FK_dist_batch_cards_dist_card_statuses]
GO
ALTER TABLE [dbo].[dist_batch_cards]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_cards_distribution_batch] FOREIGN KEY([dist_batch_id])
REFERENCES [dbo].[dist_batch] ([dist_batch_id])
GO
ALTER TABLE [dbo].[dist_batch_cards] CHECK CONSTRAINT [FK_dist_batch_cards_distribution_batch]
GO
ALTER TABLE [dbo].[dist_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_status_dist_batch_statuses] FOREIGN KEY([dist_batch_statuses_id])
REFERENCES [dbo].[dist_batch_statuses] ([dist_batch_statuses_id])
GO
ALTER TABLE [dbo].[dist_batch_status] CHECK CONSTRAINT [FK_dist_batch_status_dist_batch_statuses]
GO
ALTER TABLE [dbo].[dist_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_status_distribution_batch] FOREIGN KEY([dist_batch_id])
REFERENCES [dbo].[dist_batch] ([dist_batch_id])
GO
ALTER TABLE [dbo].[dist_batch_status] CHECK CONSTRAINT [FK_dist_batch_status_distribution_batch]
GO
ALTER TABLE [dbo].[dist_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_status_distribution_batch1] FOREIGN KEY([dist_batch_id])
REFERENCES [dbo].[dist_batch] ([dist_batch_id])
GO
ALTER TABLE [dbo].[dist_batch_status] CHECK CONSTRAINT [FK_dist_batch_status_distribution_batch1]
GO
ALTER TABLE [dbo].[dist_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_status_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[dist_batch_status] CHECK CONSTRAINT [FK_dist_batch_status_user]
GO
ALTER TABLE [dbo].[dist_batch_statuses_language]  WITH CHECK ADD FOREIGN KEY([dist_batch_statuses_id])
REFERENCES [dbo].[dist_batch_statuses] ([dist_batch_statuses_id])
GO
ALTER TABLE [dbo].[dist_batch_statuses_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[dist_card_statuses_language]  WITH CHECK ADD FOREIGN KEY([dist_card_status_id])
REFERENCES [dbo].[dist_card_statuses] ([dist_card_status_id])
GO
ALTER TABLE [dbo].[dist_card_statuses_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[file_history]  WITH CHECK ADD FOREIGN KEY([file_load_id])
REFERENCES [dbo].[file_load] ([file_load_id])
GO
ALTER TABLE [dbo].[file_history]  WITH NOCHECK ADD  CONSTRAINT [FK_file_history_file_history] FOREIGN KEY([file_type_id])
REFERENCES [dbo].[file_types] ([file_type_id])
GO
ALTER TABLE [dbo].[file_history] CHECK CONSTRAINT [FK_file_history_file_history]
GO
ALTER TABLE [dbo].[file_history]  WITH NOCHECK ADD  CONSTRAINT [FK_file_history_file_statuses] FOREIGN KEY([file_status_id])
REFERENCES [dbo].[file_statuses] ([file_status_id])
GO
ALTER TABLE [dbo].[file_history] CHECK CONSTRAINT [FK_file_history_file_statuses]
GO
ALTER TABLE [dbo].[file_history]  WITH NOCHECK ADD  CONSTRAINT [FK_file_history_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[file_history] CHECK CONSTRAINT [FK_file_history_issuer]
GO
ALTER TABLE [dbo].[file_statuses_language]  WITH NOCHECK ADD FOREIGN KEY([file_status_id])
REFERENCES [dbo].[file_statuses] ([file_status_id])
GO
ALTER TABLE [dbo].[file_statuses_language]  WITH NOCHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[flex_affiliate_codes]  WITH CHECK ADD  CONSTRAINT [FK_flex_affiliate_codes_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[flex_affiliate_codes] CHECK CONSTRAINT [FK_flex_affiliate_codes_issuer]
GO
ALTER TABLE [dbo].[flex_response_values]  WITH CHECK ADD  CONSTRAINT [FK_flex_response_values_flex_responses] FOREIGN KEY([flex_response_id])
REFERENCES [dbo].[flex_responses] ([flex_response_id])
GO
ALTER TABLE [dbo].[flex_response_values] CHECK CONSTRAINT [FK_flex_response_values_flex_responses]
GO
ALTER TABLE [dbo].[flex_response_values_language]  WITH CHECK ADD FOREIGN KEY([flex_response_value_id])
REFERENCES [dbo].[flex_response_values] ([flex_response_value_id])
GO
ALTER TABLE [dbo].[flex_response_values_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[interface_type_language]  WITH CHECK ADD FOREIGN KEY([interface_type_id])
REFERENCES [dbo].[interface_type] ([interface_type_id])
GO
ALTER TABLE [dbo].[interface_type_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[issuer]  WITH CHECK ADD  CONSTRAINT [FK_issuer_has_languages] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[issuer] CHECK CONSTRAINT [FK_issuer_has_languages]
GO
ALTER TABLE [dbo].[issuer]  WITH CHECK ADD  CONSTRAINT [FK_issuer_issuer_statuses] FOREIGN KEY([issuer_status_id])
REFERENCES [dbo].[issuer_statuses] ([issuer_status_id])
GO
ALTER TABLE [dbo].[issuer] CHECK CONSTRAINT [FK_issuer_issuer_statuses]
GO
ALTER TABLE [dbo].[issuer_interface]  WITH CHECK ADD  CONSTRAINT [FK_issuer_interface_connection_parameters] FOREIGN KEY([connection_parameter_id])
REFERENCES [dbo].[connection_parameters] ([connection_parameter_id])
GO
ALTER TABLE [dbo].[issuer_interface] CHECK CONSTRAINT [FK_issuer_interface_connection_parameters]
GO
ALTER TABLE [dbo].[issuer_interface]  WITH CHECK ADD  CONSTRAINT [FK_issuer_interface_interface_type] FOREIGN KEY([interface_type_id])
REFERENCES [dbo].[interface_type] ([interface_type_id])
GO
ALTER TABLE [dbo].[issuer_interface] CHECK CONSTRAINT [FK_issuer_interface_interface_type]
GO
ALTER TABLE [dbo].[issuer_interface]  WITH CHECK ADD  CONSTRAINT [FK_issuer_interface_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[issuer_interface] CHECK CONSTRAINT [FK_issuer_interface_issuer]
GO
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [FK_issuer_product_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [FK_issuer_product_issuer]
GO
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [FK_issuer_product_Issuer_product_font] FOREIGN KEY([font_id])
REFERENCES [dbo].[Issuer_product_font] ([font_id])
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [FK_issuer_product_Issuer_product_font]
GO
ALTER TABLE [dbo].[Issuer_product_font]  WITH CHECK ADD  CONSTRAINT [FK_Issuer_product_font_Issuer_product_font] FOREIGN KEY([font_id])
REFERENCES [dbo].[Issuer_product_font] ([font_id])
GO
ALTER TABLE [dbo].[Issuer_product_font] CHECK CONSTRAINT [FK_Issuer_product_font_Issuer_product_font]
GO
ALTER TABLE [dbo].[issuer_statuses_language]  WITH CHECK ADD FOREIGN KEY([issuer_status_id])
REFERENCES [dbo].[issuer_statuses] ([issuer_status_id])
GO
ALTER TABLE [dbo].[issuer_statuses_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[load_batch]  WITH CHECK ADD  CONSTRAINT [FK_load_batch_batch_statuses] FOREIGN KEY([load_batch_status_id])
REFERENCES [dbo].[load_batch_statuses] ([load_batch_statuses_id])
GO
ALTER TABLE [dbo].[load_batch] CHECK CONSTRAINT [FK_load_batch_batch_statuses]
GO
ALTER TABLE [dbo].[load_batch]  WITH CHECK ADD  CONSTRAINT [FK_load_batch_file_history] FOREIGN KEY([file_id])
REFERENCES [dbo].[file_history] ([file_id])
GO
ALTER TABLE [dbo].[load_batch] CHECK CONSTRAINT [FK_load_batch_file_history]
GO
ALTER TABLE [dbo].[load_batch]  WITH CHECK ADD  CONSTRAINT [FK_load_batch_load_batch] FOREIGN KEY([load_batch_id])
REFERENCES [dbo].[load_batch] ([load_batch_id])
GO
ALTER TABLE [dbo].[load_batch] CHECK CONSTRAINT [FK_load_batch_load_batch]
GO
ALTER TABLE [dbo].[load_batch_cards]  WITH CHECK ADD  CONSTRAINT [FK_load_batch_cards_cards] FOREIGN KEY([card_id])
REFERENCES [dbo].[cards] ([card_id])
GO
ALTER TABLE [dbo].[load_batch_cards] CHECK CONSTRAINT [FK_load_batch_cards_cards]
GO
ALTER TABLE [dbo].[load_batch_cards]  WITH CHECK ADD  CONSTRAINT [FK_load_batch_cards_load_batch] FOREIGN KEY([load_batch_id])
REFERENCES [dbo].[load_batch] ([load_batch_id])
GO
ALTER TABLE [dbo].[load_batch_cards] CHECK CONSTRAINT [FK_load_batch_cards_load_batch]
GO
ALTER TABLE [dbo].[load_batch_cards]  WITH CHECK ADD  CONSTRAINT [FK_load_batch_cards_load_card_statuses] FOREIGN KEY([load_card_status_id])
REFERENCES [dbo].[load_card_statuses] ([load_card_status_id])
GO
ALTER TABLE [dbo].[load_batch_cards] CHECK CONSTRAINT [FK_load_batch_cards_load_card_statuses]
GO
ALTER TABLE [dbo].[load_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_load_batch_status_batch_statuses] FOREIGN KEY([load_batch_statuses_id])
REFERENCES [dbo].[load_batch_statuses] ([load_batch_statuses_id])
GO
ALTER TABLE [dbo].[load_batch_status] CHECK CONSTRAINT [FK_load_batch_status_batch_statuses]
GO
ALTER TABLE [dbo].[load_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_load_batch_status_load_batch] FOREIGN KEY([load_batch_id])
REFERENCES [dbo].[load_batch] ([load_batch_id])
GO
ALTER TABLE [dbo].[load_batch_status] CHECK CONSTRAINT [FK_load_batch_status_load_batch]
GO
ALTER TABLE [dbo].[load_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_load_batch_status_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[load_batch_status] CHECK CONSTRAINT [FK_load_batch_status_user]
GO
ALTER TABLE [dbo].[load_batch_statuses_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[load_batch_statuses_language]  WITH CHECK ADD FOREIGN KEY([load_batch_statuses_id])
REFERENCES [dbo].[load_batch_statuses] ([load_batch_statuses_id])
GO
ALTER TABLE [dbo].[load_card_failed]  WITH CHECK ADD  CONSTRAINT [FK_load_card_failed_load_batch] FOREIGN KEY([load_batch_id])
REFERENCES [dbo].[load_batch] ([load_batch_id])
GO
ALTER TABLE [dbo].[load_card_failed] CHECK CONSTRAINT [FK_load_card_failed_load_batch]
GO
ALTER TABLE [dbo].[load_card_statuses_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[load_card_statuses_language]  WITH CHECK ADD FOREIGN KEY([load_card_status_id])
REFERENCES [dbo].[load_card_statuses] ([load_card_status_id])
GO
ALTER TABLE [dbo].[message_parameters_has_interface]  WITH CHECK ADD  CONSTRAINT [FK_message_parameters_id_message_parameters_has_interface] FOREIGN KEY([message_parameters_id])
REFERENCES [dbo].[message_parameters] ([id])
GO
ALTER TABLE [dbo].[message_parameters_has_interface] CHECK CONSTRAINT [FK_message_parameters_id_message_parameters_has_interface]
GO
ALTER TABLE [dbo].[mod_interface_account_params]  WITH CHECK ADD  CONSTRAINT [FK_mod_interface_account_params_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[mod_interface_account_params] CHECK CONSTRAINT [FK_mod_interface_account_params_issuer]
GO
ALTER TABLE [dbo].[mod_response_mapping]  WITH CHECK ADD  CONSTRAINT [FK_mod_response_mapping_branch_card_codes] FOREIGN KEY([branch_card_code_id])
REFERENCES [dbo].[branch_card_codes] ([branch_card_code_id])
GO
ALTER TABLE [dbo].[mod_response_mapping] CHECK CONSTRAINT [FK_mod_response_mapping_branch_card_codes]
GO
ALTER TABLE [dbo].[product_currency]  WITH CHECK ADD FOREIGN KEY([currency_id])
REFERENCES [dbo].[currency] ([currency_id])
GO
ALTER TABLE [dbo].[product_currency]  WITH CHECK ADD FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO
ALTER TABLE [dbo].[user]  WITH CHECK ADD FOREIGN KEY([ldap_setting_id])
REFERENCES [dbo].[ldap_setting] ([ldap_setting_id])
GO
ALTER TABLE [dbo].[user]  WITH CHECK ADD  CONSTRAINT [FK_application_user_user_status] FOREIGN KEY([user_status_id])
REFERENCES [dbo].[user_status] ([user_status_id])
GO
ALTER TABLE [dbo].[user] CHECK CONSTRAINT [FK_application_user_user_status]
GO
ALTER TABLE [dbo].[user]  WITH CHECK ADD  CONSTRAINT [FK_user_has_languages] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[user] CHECK CONSTRAINT [FK_user_has_languages]
GO
ALTER TABLE [dbo].[user]  WITH CHECK ADD  CONSTRAINT [FK_user_user_gender] FOREIGN KEY([user_gender_id])
REFERENCES [dbo].[user_gender] ([user_gender_id])
GO
ALTER TABLE [dbo].[user] CHECK CONSTRAINT [FK_user_user_gender]
GO
ALTER TABLE [dbo].[user_group]  WITH CHECK ADD  CONSTRAINT [FK_user_group_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[user_group] CHECK CONSTRAINT [FK_user_group_issuer]
GO
ALTER TABLE [dbo].[user_group]  WITH CHECK ADD  CONSTRAINT [FK_user_group_user_roles] FOREIGN KEY([user_role_id])
REFERENCES [dbo].[user_roles] ([user_role_id])
GO
ALTER TABLE [dbo].[user_group] CHECK CONSTRAINT [FK_user_group_user_roles]
GO
ALTER TABLE [dbo].[user_groups_branches]  WITH CHECK ADD  CONSTRAINT [FK_user_groups_branches_branch] FOREIGN KEY([branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO
ALTER TABLE [dbo].[user_groups_branches] CHECK CONSTRAINT [FK_user_groups_branches_branch]
GO
ALTER TABLE [dbo].[user_groups_branches]  WITH CHECK ADD  CONSTRAINT [FK_user_groups_branches_user_group] FOREIGN KEY([user_group_id])
REFERENCES [dbo].[user_group] ([user_group_id])
GO
ALTER TABLE [dbo].[user_groups_branches] CHECK CONSTRAINT [FK_user_groups_branches_user_group]
GO
ALTER TABLE [dbo].[user_password_history]  WITH CHECK ADD  CONSTRAINT [FK_user_password_history_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[user_password_history] CHECK CONSTRAINT [FK_user_password_history_user]
GO
ALTER TABLE [dbo].[user_roles_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[user_roles_language]  WITH CHECK ADD FOREIGN KEY([user_role_id])
REFERENCES [dbo].[user_roles] ([user_role_id])
GO
ALTER TABLE [dbo].[user_status_language]  WITH CHECK ADD FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[user_status_language]  WITH CHECK ADD FOREIGN KEY([user_status_id])
REFERENCES [dbo].[user_status] ([user_status_id])
GO
ALTER TABLE [dbo].[users_to_users_groups]  WITH CHECK ADD  CONSTRAINT [FK_users_to_users_groups_application_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[users_to_users_groups] CHECK CONSTRAINT [FK_users_to_users_groups_application_user]
GO
ALTER TABLE [dbo].[users_to_users_groups]  WITH CHECK ADD  CONSTRAINT [FK_users_to_users_groups_user_group] FOREIGN KEY([user_group_id])
REFERENCES [dbo].[user_group] ([user_group_id])
GO
ALTER TABLE [dbo].[users_to_users_groups] CHECK CONSTRAINT [FK_users_to_users_groups_user_group]
GO

/* Lebo's Changes */
ALTER TABLE dbo.[user]
  	ADD [instant_authorisation_pin] [varbinary](256) NULL
	, [last_authorisation_pin_changed_date] [datetime] NULL
GO


ALTER TABLE dbo.[issuer]
	ADD [enable_instant_pin_YN] [bit] NOT NULL DEFAULT 0,
	 [authorise_pin_issue_YN] [bit] NOT NULL DEFAULT 0, 
	 [authorise_pin_reissue_YN] [bit] NOT NULL DEFAULT 0
GO
