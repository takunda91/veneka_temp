USE [{DATABASE_NAME}]
GO

/****** Object:  Table [dbo].[audit_action]    Script Date: 2018-09-13 10:10:01 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[audit_action_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_audit_action_language] PRIMARY KEY CLUSTERED 
(
	[audit_action_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[audit_control]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[audit_date] [datetimeoffset](7) NOT NULL,
	[workstation_address] [varchar](100) NOT NULL,
	[action_description] [varchar](max) NULL,
	[issuer_id] [int] NULL,
	[data_changed] [varchar](30) NULL,
	[data_before] [varchar](max) NULL,
	[data_after] [varchar](max) NULL,
 CONSTRAINT [PK_audit_control] PRIMARY KEY CLUSTERED 
(
	[audit_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[auth_configuration]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[auth_configuration](
	[authentication_configuration_id] [int] IDENTITY(1,1) NOT NULL,
	[authentication_configuration] [varchar](100) NOT NULL,
 CONSTRAINT [PK_authentication_configuration] PRIMARY KEY CLUSTERED 
(
	[authentication_configuration_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[auth_configuration_connection_parameters]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[auth_configuration_connection_parameters](
	[authentication_configuration_id] [int] NOT NULL,
	[auth_type_id] [int] NULL,
	[connection_parameter_id] [int] NULL,
	[interface_guid] [char](36) NULL,
	[external_interface_id] [char](36) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[auth_configuration_connection_parameters_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[auth_configuration_connection_parameters_audit](
	[authentication_configuration_id] [int] NOT NULL,
	[auth_type_id] [int] NULL,
	[interface_guid] [char](36) NULL,
	[connection_parameter_id] [int] NULL,
	[external_interface_id] [char](36) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[auth_type]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[auth_type](
	[auth_type_id] [int] NOT NULL,
	[auth_type_name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_auth_type] PRIMARY KEY CLUSTERED 
(
	[auth_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[branch]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[emp_branch_code] [nvarchar](10) NULL,
	[branch_type_id] [int] NULL,
	[main_branch_id] [int] NULL,
 CONSTRAINT [PK_branch] PRIMARY KEY CLUSTERED 
(
	[branch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[branch_card_code_type]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[branch_card_codes]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[branch_card_codes_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[branch_card_status]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[branch_card_status](
	[card_id] [bigint] NOT NULL,
	[branch_card_statuses_id] [int] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[operator_user_id] [bigint] NULL,
	[branch_card_code_id] [int] NULL,
	[comments] [varchar](1000) NULL,
	[pin_auth_user_id] [bigint] NULL,
	[branch_id] [int] NOT NULL,
 CONSTRAINT [PK_branch_card_status] PRIMARY KEY CLUSTERED 
(
	[card_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[branch_card_status_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[branch_card_status_audit](
	[branch_card_status_id] [bigint] IDENTITY(1,1) NOT NULL,
	[card_id] [bigint] NOT NULL,
	[branch_card_statuses_id] [int] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[operator_user_id] [bigint] NULL,
	[branch_card_code_id] [int] NULL,
	[comments] [varchar](1000) NULL,
	[pin_auth_user_id] [bigint] NULL,
	[branch_id] [int] NOT NULL,
 CONSTRAINT [PK_branch_card_status_audit] PRIMARY KEY CLUSTERED 
(
	[branch_card_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[branch_card_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[branch_card_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_branch_card_statuses_language] PRIMARY KEY CLUSTERED 
(
	[branch_card_statuses_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[branch_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[branch_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_branch_statuses_language] PRIMARY KEY CLUSTERED 
(
	[branch_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[branch_type]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[branch_type](
	[branch_type_id] [int] NOT NULL,
	[branch_type_name] [nvarchar](50) NULL,
 CONSTRAINT [PK_branch_type] PRIMARY KEY CLUSTERED 
(
	[branch_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[branch_type_language]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[branch_type_language](
	[branch_type_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](1000) NULL,
 CONSTRAINT [PK_branch_type_language] PRIMARY KEY CLUSTERED 
(
	[language_id] ASC,
	[branch_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[card_fee_charge_status]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[card_fee_charge_status](
	[card_fee_charge_status_id] [int] NOT NULL,
	[card_fee_charge_status_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_card_fee_charge_status] PRIMARY KEY CLUSTERED 
(
	[card_fee_charge_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[card_fee_charge_status_language]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[card_fee_charge_status_language](
	[card_fee_charge_status_id] [int] NOT NULL,
	[langauge_text] [nvarchar](250) NOT NULL,
	[language_id] [int] NOT NULL,
 CONSTRAINT [PK_card_fee_charge_status_language] PRIMARY KEY CLUSTERED 
(
	[card_fee_charge_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[card_issue_method]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[card_issue_method](
	[card_issue_method_id] [int] NOT NULL,
	[card_issue_method_name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_card_issue_method] PRIMARY KEY CLUSTERED 
(
	[card_issue_method_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[card_issue_method_language]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[card_issue_method_language](
	[card_issue_method_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_card_issue_method_language] PRIMARY KEY CLUSTERED 
(
	[card_issue_method_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[card_issue_reason]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[card_issue_reason_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_card_issue_reason_language] PRIMARY KEY CLUSTERED 
(
	[card_issue_reason_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[card_priority]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[card_priority](
	[card_priority_id] [int] NOT NULL,
	[card_priority_order] [int] NOT NULL,
	[card_priority_name] [varchar](50) NOT NULL,
	[default_selection] [bit] NOT NULL,
 CONSTRAINT [PK_card_priority] PRIMARY KEY CLUSTERED 
(
	[card_priority_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[card_priority_language]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[card_priority_language](
	[card_priority_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_card_priority_language] PRIMARY KEY CLUSTERED 
(
	[card_priority_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cards]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[card_issue_method_id] [int] NOT NULL,
	[card_priority_id] [int] NOT NULL,
	[card_request_reference] [varchar](100) NULL,
	[card_production_date] [varbinary](max) NULL,
	[card_expiry_date] [varbinary](max) NULL,
	[card_activation_date] [varbinary](max) NULL,
	[pvv] [varbinary](max) NULL,
	[origin_branch_id] [int] NOT NULL,
	[export_batch_id] [bigint] NULL,
	[ordering_branch_id] [int] NOT NULL,
	[delivery_branch_id] [int] NOT NULL,
	[fee_id] [bigint] NULL,
 CONSTRAINT [PK_cards] PRIMARY KEY CLUSTERED 
(
	[card_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [uq_card_request_reference] UNIQUE NONCLUSTERED 
(
	[card_request_reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[config]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[config](
	[distbatch_create_order] [bit] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[connection_parameter_type]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[connection_parameter_type](
	[connection_parameter_type_id] [int] NOT NULL,
	[connection_parameter_type_name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_connection_parameter_type] PRIMARY KEY CLUSTERED 
(
	[connection_parameter_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[connection_parameter_type_language]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[connection_parameter_type_language](
	[connection_parameter_type_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_connection_parameter_type_language] PRIMARY KEY CLUSTERED 
(
	[connection_parameter_type_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[connection_parameters]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[protocol] [int] NULL,
	[auth_type] [int] NOT NULL,
	[username] [varbinary](max) NOT NULL,
	[password] [varbinary](max) NOT NULL,
	[connection_parameter_type_id] [int] NOT NULL,
	[header_length] [int] NULL,
	[identification] [varbinary](max) NULL,
	[timeout_milli] [int] NULL,
	[buffer_size] [int] NULL,
	[doc_type] [char](1) NULL,
	[name_of_file] [varchar](100) NULL,
	[file_delete_YN] [bit] NULL,
	[file_encryption_type_id] [int] NULL,
	[duplicate_file_check_YN] [bit] NULL,
	[private_key] [varbinary](max) NULL,
	[public_key] [varbinary](max) NULL,
	[domain_name] [varchar](100) NULL,
	[is_external_auth] [bit] NULL,
	[remote_port] [int] NULL,
	[remote_username] [varbinary](max) NULL,
	[remote_password] [varbinary](max) NULL,
	[nonce] [varbinary](max) NULL,
 CONSTRAINT [PK_connection_parameters] PRIMARY KEY CLUSTERED 
(
	[connection_parameter_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[connection_parameters_additionaldata]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[connection_parameters_additionaldata](
	[connection_parameter_id] [int] NOT NULL,
	[key] [varchar](50) NULL,
	[value] [varchar](500) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[connection_parameters_additionaldata_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[connection_parameters_additionaldata_audit](
	[connection_parameter_id] [int] NULL,
	[key] [varchar](50) NULL,
	[value] [varchar](500) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[connection_parameters_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[connection_parameters_audit](
	[connection_parameter_id] [int] IDENTITY(1,1) NOT NULL,
	[connection_name] [varchar](100) NOT NULL,
	[address] [varchar](200) NOT NULL,
	[port] [int] NOT NULL,
	[path] [varchar](200) NOT NULL,
	[protocol] [int] NULL,
	[auth_type] [int] NOT NULL,
	[username] [varbinary](max) NOT NULL,
	[password] [varbinary](max) NOT NULL,
	[connection_parameter_type_id] [int] NOT NULL,
	[header_length] [int] NULL,
	[identification] [varbinary](max) NULL,
	[timeout_milli] [int] NULL,
	[buffer_size] [int] NULL,
	[doc_type] [char](1) NULL,
	[name_of_file] [varchar](100) NULL,
	[file_delete_YN] [bit] NULL,
	[file_encryption_type_id] [int] NULL,
	[duplicate_file_check_YN] [bit] NULL,
	[private_key] [varbinary](max) NULL,
	[public_key] [varbinary](max) NULL,
	[domain_name] [varchar](100) NULL,
	[is_external_auth] [bit] NULL,
	[remote_port] [int] NULL,
	[remote_username] [varbinary](max) NULL,
	[remote_password] [varbinary](max) NULL,
	[nonce] [varbinary](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[country]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[currency]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[currency](
	[currency_id] [int] NOT NULL,
	[currency_code] [char](3) NOT NULL,
	[iso_4217_numeric_code] [char](3) NOT NULL,
	[iso_4217_minor_unit] [int] NULL,
	[currency_desc] [varchar](100) NOT NULL,
	[active_YN] [bit] NOT NULL,
 CONSTRAINT [PK_currency] PRIMARY KEY CLUSTERED 
(
	[currency_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[customer_account]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_account](
	[customer_account_id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [bigint] NOT NULL,
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
	[date_issued] [datetimeoffset](7) NOT NULL,
	[cms_id] [varchar](50) NULL,
	[contract_number] [varchar](50) NULL,
	[customer_title_id] [int] NULL,
	[Id_number] [varbinary](max) NULL,
	[contact_number] [varbinary](max) NULL,
	[CustomerId] [varbinary](max) NULL,
	[domicile_branch_id] [int] NOT NULL,
 CONSTRAINT [PK_customer_account] PRIMARY KEY CLUSTERED 
(
	[customer_account_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],

) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[customer_account_cards]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[customer_account_cards](
	[customer_account_id] [bigint] NOT NULL,
	[card_id] [bigint] NOT NULL,
 CONSTRAINT [PK_customer_account_cards] PRIMARY KEY CLUSTERED 
(
	[customer_account_id] ASC,
	[card_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[customer_account_requests]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[customer_account_requests](
	[customer_account_id] [bigint] NOT NULL,
	[request_id] [bigint] NOT NULL,
 CONSTRAINT [PK_customer_account_requests] PRIMARY KEY CLUSTERED 
(
	[request_id] ASC,
	[customer_account_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[customer_account_type]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_account_type](
	[account_type_id] [int] NOT NULL,
	[account_type_name] [varchar](100) NOT NULL,
	[active_YN] [bit] NOT NULL,
 CONSTRAINT [PK_customer_account_type] PRIMARY KEY CLUSTERED 
(
	[account_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[customer_account_type_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_customer_account_type_language] PRIMARY KEY CLUSTERED 
(
	[account_type_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[customer_fields]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_fields](
	[customer_account_id] [bigint] NOT NULL,
	[product_field_id] [int] NOT NULL,
	[value] [varbinary](max) NULL,
 CONSTRAINT [PK_customer_fields] PRIMARY KEY CLUSTERED 
(
	[customer_account_id] ASC,
	[product_field_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[customer_image_fields]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[customer_image_fields](
	[customer_account_id] [bigint] NOT NULL,
	[product_field_id] [int] NOT NULL,
	[value] [image] NOT NULL,
 CONSTRAINT [PK_customer_image_fields] PRIMARY KEY CLUSTERED 
(
	[customer_account_id] ASC,
	[product_field_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[customer_residency]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_residency](
	[resident_id] [int] NOT NULL,
	[residency_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_customer_residency] PRIMARY KEY CLUSTERED 
(
	[resident_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[customer_residency_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_customer_residency_language] PRIMARY KEY CLUSTERED 
(
	[resident_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[customer_title]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_title](
	[customer_title_id] [int] NOT NULL,
	[customer_title_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_customer_title] PRIMARY KEY CLUSTERED 
(
	[customer_title_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[customer_title_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_customer_title_language] PRIMARY KEY CLUSTERED 
(
	[customer_title_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[customer_type]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customer_type](
	[customer_type_id] [int] NOT NULL,
	[customer_type_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_customer_type] PRIMARY KEY CLUSTERED 
(
	[customer_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[customer_type_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_customer_type_language] PRIMARY KEY CLUSTERED 
(
	[customer_type_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[dist_batch]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[dist_batch](
	[dist_batch_id] [bigint] IDENTITY(1,1) NOT NULL,
	[branch_id] [int] NULL,
	[no_cards] [int] NOT NULL,
	[date_created] [datetimeoffset](7) NOT NULL,
	[dist_batch_reference] [varchar](50) NOT NULL,
	[card_issue_method_id] [int] NOT NULL,
	[dist_batch_type_id] [int] NOT NULL,
	[issuer_id] [int] NOT NULL,
	[dist_version] [int] NOT NULL,
	[origin_branch_id] [int] NULL,
 CONSTRAINT [PK_distribution_batch] PRIMARY KEY CLUSTERED 
(
	[dist_batch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[dist_batch_cards]    Script Date: 2018-09-13 10:10:02 AM ******/
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
/****** Object:  Table [dbo].[dist_batch_status]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[dist_batch_status](
	[dist_batch_id] [bigint] NOT NULL,
	[dist_batch_statuses_id] [int] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[status_notes] [varchar](150) NULL,
 CONSTRAINT [PK_dist_batch_status] PRIMARY KEY CLUSTERED 
(
	[dist_batch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[dist_batch_status_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[dist_batch_status_audit](
	[dist_batch_status_id] [bigint] IDENTITY(1,1) NOT NULL,
	[dist_batch_id] [bigint] NOT NULL,
	[dist_batch_statuses_id] [int] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[status_notes] [varchar](150) NULL,
 CONSTRAINT [PK_dist_batch_status_audit] PRIMARY KEY CLUSTERED 
(
	[dist_batch_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[dist_batch_status_flow]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[dist_batch_status_flow](
	[dist_batch_status_flow_id] [int] NOT NULL,
	[dist_batch_status_flow_name] [varchar](150) NOT NULL,
	[dist_batch_type_id] [int] NOT NULL,
	[card_issue_method_id] [int] NOT NULL,
 CONSTRAINT [PK_dist_batch_status_flow] PRIMARY KEY CLUSTERED 
(
	[dist_batch_status_flow_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[dist_batch_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[dist_batch_statuses_flow]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[dist_batch_statuses_flow](
	[dist_batch_statuses_id] [int] NOT NULL,
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
	[dist_batch_status_flow_id] [int] NOT NULL,
 CONSTRAINT [PK_DistStatusesFlow] PRIMARY KEY CLUSTERED 
(
	[dist_batch_status_flow_id] ASC,
	[dist_batch_statuses_id] ASC,
	[flow_dist_batch_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[dist_batch_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_dist_batch_statuses_language] PRIMARY KEY CLUSTERED 
(
	[dist_batch_statuses_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[dist_batch_type]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[dist_card_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[dist_card_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_dist_card_statuses_language] PRIMARY KEY CLUSTERED 
(
	[dist_card_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[export_batch]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[date_created] [datetimeoffset](7) NOT NULL,
	[no_cards] [int] NOT NULL,
 CONSTRAINT [PK_export_batch] PRIMARY KEY CLUSTERED 
(
	[export_batch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[export_batch_status]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[export_batch_status](
	[export_batch_id] [bigint] NOT NULL,
	[export_batch_statuses_id] [int] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[comments] [varchar](100) NOT NULL,
 CONSTRAINT [PK_export_batch_status] PRIMARY KEY CLUSTERED 
(
	[export_batch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[export_batch_status_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[export_batch_status_audit](
	[export_batch_status_id] [bigint] IDENTITY(1,1) NOT NULL,
	[export_batch_id] [bigint] NOT NULL,
	[export_batch_statuses_id] [int] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[comments] [varchar](100) NOT NULL,
 CONSTRAINT [PK_export_batch_status_audit] PRIMARY KEY CLUSTERED 
(
	[export_batch_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[export_batch_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[export_batch_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[external_system_fields]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[external_system_types]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[external_system_types_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[external_systems]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[fee_charged]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[fee_charged](
	[fee_id] [bigint] IDENTITY(1,1) NOT NULL,
	[fee_charged] [decimal](10, 4) NULL,
	[vat] [decimal](7, 4) NULL,
	[vat_charged] [numeric](21, 10) NULL,
	[total_charged] [numeric](22, 10) NULL,
	[fee_waiver_YN] [bit] NULL,
	[fee_editable_YN] [bit] NULL,
	[fee_overridden_YN] [bit] NULL,
	[fee_reference_number] [varchar](100) NULL,
	[fee_reversal_ref_number] [varchar](100) NULL,
	[operator_user_id] [bigint] NOT NULL,
	[fee_charge_status_id] [int] NULL,
 CONSTRAINT [PK_fee_charged] PRIMARY KEY CLUSTERED 
(
	[fee_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[file_encryption_type]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[file_encryption_type](
	[file_encryption_type_id] [int] NOT NULL,
	[file_encryption_type] [varchar](250) NOT NULL,
	[file_encryption_typeid] [int] NULL,
 CONSTRAINT [PK_file_encryption_type] PRIMARY KEY CLUSTERED 
(
	[file_encryption_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[file_history]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[file_created_date] [datetimeoffset](7) NOT NULL,
	[file_size] [int] NOT NULL,
	[load_date] [datetimeoffset](7) NOT NULL,
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[file_load]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[file_load](
	[file_load_id] [int] IDENTITY(1,1) NOT NULL,
	[file_load_start] [datetimeoffset](7) NOT NULL,
	[file_load_end] [datetimeoffset](7) NULL,
	[user_id] [int] NOT NULL,
	[files_to_process] [int] NOT NULL,
 CONSTRAINT [PK_file_load] PRIMARY KEY CLUSTERED 
(
	[file_load_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[file_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[file_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_file_statuses_language] PRIMARY KEY CLUSTERED 
(
	[file_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[file_types]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[fileloader_status]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fileloader_status](
	[fileloader_status] [int] NOT NULL,
	[executed_datetime] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_fileloader_status] PRIMARY KEY CLUSTERED 
(
	[fileloader_status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[hybrid_request_status]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[hybrid_request_status](
	[hybrid_request_status_id] [int] IDENTITY(1,1) NOT NULL,
	[request_id] [bigint] NOT NULL,
	[hybrid_request_statuses_id] [int] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[comments] [varchar](1000) NULL,
	[branch_id] [int] NOT NULL,
	[operator_user_id] [bigint] NOT NULL,
 CONSTRAINT [PK_hybrid_request_status] PRIMARY KEY CLUSTERED 
(
	[hybrid_request_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[hybrid_request_status_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[hybrid_request_status_audit](
	[hybrid_request_status_id] [int] NOT NULL,
	[request_id] [bigint] NOT NULL,
	[hybrid_request_statuses_id] [int] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[comments] [varchar](1000) NULL,
	[branch_id] [int] NOT NULL,
	[operator_user_id] [bigint] NOT NULL,
 CONSTRAINT [PK_hybrid_request_status_audit] PRIMARY KEY CLUSTERED 
(
	[hybrid_request_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[hybrid_request_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[hybrid_request_statuses](
	[hybrid_request_statuses_id] [int] NOT NULL,
	[hybrid_request_statuses] [varchar](100) NULL,
 CONSTRAINT [PK_hybrid_request_statuses] PRIMARY KEY CLUSTERED 
(
	[hybrid_request_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[hybrid_request_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[hybrid_request_statuses_language](
	[hybrid_request_statuses_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NULL,
 CONSTRAINT [PK_hybrid_request_statuses_language] PRIMARY KEY CLUSTERED 
(
	[hybrid_request_statuses_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[hybrid_requests]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[hybrid_requests](
	[request_id] [bigint] IDENTITY(1,1) NOT NULL,
	[product_id] [int] NULL,
	[branch_id] [int] NULL,
	[request_reference] [varchar](100) NULL,
	[origin_branch_id] [int] NULL,
	[ordering_branch_id] [int] NULL,
	[delivery_branch_id] [int] NULL,
	[card_issue_method_id] [int] NOT NULL,
	[card_priority_id] [int] NULL,
	[fee_id] [bigint] NULL,
 CONSTRAINT [PK_hybrid_requests] PRIMARY KEY CLUSTERED 
(
	[request_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[integration]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[integration](
	[integration_id] [int] NOT NULL,
	[integration_name] [varchar](150) NOT NULL,
 CONSTRAINT [PK_integration] PRIMARY KEY CLUSTERED 
(
	[integration_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[integration_cardnumbers]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[integration_cardnumbers](
	[card_sequence_number] [varbinary](max) NOT NULL,
	[product_id] [int] NOT NULL,
	[sub_product_id] [int] NOT NULL,
 CONSTRAINT [PK_integration_cardnumbers] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC,
	[sub_product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[integration_fields]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[integration_fields](
	[integration_id] [int] NOT NULL,
	[integration_object_id] [int] NOT NULL,
	[integration_field_id] [int] NOT NULL,
	[integration_field_name] [varchar](150) NOT NULL,
	[accept_all_responses] [bit] NOT NULL,
	[integration_field_default_value] [varbinary](max) NULL,
 CONSTRAINT [PK_integration_fields] PRIMARY KEY CLUSTERED 
(
	[integration_id] ASC,
	[integration_object_id] ASC,
	[integration_field_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[integration_object]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[integration_object](
	[integration_id] [int] NOT NULL,
	[integration_object_id] [int] NOT NULL,
	[integration_object_name] [varchar](150) NOT NULL,
 CONSTRAINT [PK_integration_object] PRIMARY KEY CLUSTERED 
(
	[integration_id] ASC,
	[integration_object_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[integration_responses]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[integration_responses](
	[integration_id] [int] NOT NULL,
	[integration_object_id] [int] NOT NULL,
	[integration_field_id] [int] NOT NULL,
	[integration_response_id] [int] NOT NULL,
	[integration_response_value] [varchar](max) NOT NULL,
	[integration_response_valid_response] [bit] NOT NULL,
 CONSTRAINT [PK_integration_responses] PRIMARY KEY CLUSTERED 
(
	[integration_id] ASC,
	[integration_object_id] ASC,
	[integration_field_id] ASC,
	[integration_response_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[integration_responses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[integration_responses_language](
	[integration_id] [int] NOT NULL,
	[integration_object_id] [int] NOT NULL,
	[integration_field_id] [int] NOT NULL,
	[integration_response_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[response_text] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_integration_responses_language] PRIMARY KEY CLUSTERED 
(
	[integration_id] ASC,
	[integration_object_id] ASC,
	[integration_field_id] ASC,
	[integration_response_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[interface_type]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[interface_type_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_interface_type_language] PRIMARY KEY CLUSTERED 
(
	[interface_type_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[issuer]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[instant_card_issue_YN] [bit] NOT NULL,
	[maker_checker_YN] [bit] NOT NULL,
	[license_file] [varchar](100) NULL,
	[license_key] [varchar](1000) NULL,
	[language_id] [int] NULL,
	[card_ref_preference] [bit] NOT NULL,
	[classic_card_issue_YN] [bit] NOT NULL,
	[enable_instant_pin_YN] [bit] NOT NULL,
	[authorise_pin_issue_YN] [bit] NOT NULL,
	[authorise_pin_reissue_YN] [bit] NOT NULL,
	[back_office_pin_auth_YN] [bit] NOT NULL,
	[remote_token] [uniqueidentifier] NOT NULL,
	[remote_token_expiry] [datetime] NULL,
	[allow_branches_to_search_cards] [bit] NOT NULL,
 CONSTRAINT [PK_issuer] PRIMARY KEY CLUSTERED 
(
	[issuer_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[issuer_interface]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[issuer_interface](
	[interface_type_id] [int] NOT NULL,
	[issuer_id] [int] NOT NULL,
	[connection_parameter_id] [int] NOT NULL,
	[interface_area] [int] NOT NULL,
	[interface_guid] [char](36) NULL,
 CONSTRAINT [PK_issuer_interface] PRIMARY KEY CLUSTERED 
(
	[interface_type_id] ASC,
	[issuer_id] ASC,
	[interface_area] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[issuer_product]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[src1_id] [int] NOT NULL,
	[src2_id] [int] NOT NULL,
	[src3_id] [int] NOT NULL,
	[PVKI] [varbinary](max) NULL,
	[PVK] [varbinary](max) NULL,
	[CVKA] [varbinary](max) NULL,
	[CVKB] [varbinary](max) NULL,
	[expiry_months] [int] NULL,
	[fee_scheme_id] [int] NULL,
	[enable_instant_pin_YN] [bit] NOT NULL,
	[min_pin_length] [int] NOT NULL,
	[max_pin_length] [int] NOT NULL,
	[enable_instant_pin_reissue_YN] [bit] NOT NULL,
	[cms_exportable_YN] [bit] NOT NULL,
	[product_load_type_id] [int] NOT NULL,
	[sub_product_code] [varchar](4) NULL,
	[pin_calc_method_id] [int] NOT NULL,
	[auto_approve_batch_YN] [bit] NOT NULL,
	[account_validation_YN] [bit] NOT NULL,
	[pan_length] [smallint] NOT NULL,
	[pin_mailer_printing_YN] [bit] NOT NULL,
	[pin_mailer_reprint_YN] [bit] NOT NULL,
	[sub_product_id] [int] NULL,
	[master_product_id] [int] NULL,
	[card_issue_method_id] [int] NOT NULL,
	[decimalisation_table] [varbinary](max) NULL,
	[pin_validation_data] [varbinary](max) NULL,
	[pin_block_formatid] [int] NULL,
	[production_dist_batch_status_flow] [int] NOT NULL,
	[distribution_dist_batch_status_flow] [int] NOT NULL,
	[charge_fee_to_issuing_branch_YN] [bit] NOT NULL,
	[print_issue_card_YN] [bit] NOT NULL,
	[allow_manual_uploaded_YN] [bit] NOT NULL,
	[allow_reupload_YN] [bit] NOT NULL,
	[remote_cms_update_YN] [bit] NOT NULL,
	[charge_fee_at_cardrequest] [bit] NULL,
	[e_pin_request_YN] [bit] NULL,
 CONSTRAINT [PK_issuer_product] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[Issuer_product_font]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[issuer_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[issuer_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_issuer_statuses_language] PRIMARY KEY CLUSTERED 
(
	[issuer_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[languages]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[ldap_setting]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[auth_type_id] [int] NULL,
	[external_inteface_id] [char](36) NULL,
 CONSTRAINT [PK_ldap_setting] PRIMARY KEY CLUSTERED 
(
	[ldap_setting_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[load_batch]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[load_date] [datetimeoffset](7) NOT NULL,
	[load_batch_reference] [varchar](100) NOT NULL,
	[load_batch_type_id] [int] NOT NULL,
 CONSTRAINT [PK_load_batch] PRIMARY KEY CLUSTERED 
(
	[load_batch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[load_batch_cards]    Script Date: 2018-09-13 10:10:02 AM ******/
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
/****** Object:  Table [dbo].[load_batch_status]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[load_batch_status](
	[load_batch_id] [bigint] NOT NULL,
	[load_batch_statuses_id] [int] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[status_notes] [varchar](150) NULL,
 CONSTRAINT [PK_load_batch_status] PRIMARY KEY CLUSTERED 
(
	[load_batch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[load_batch_status_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[load_batch_status_audit](
	[load_batch_status_id] [bigint] IDENTITY(1,1) NOT NULL,
	[load_batch_id] [bigint] NOT NULL,
	[load_batch_statuses_id] [int] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[status_notes] [varchar](150) NULL,
 CONSTRAINT [PK_load_batch_status_audit] PRIMARY KEY CLUSTERED 
(
	[load_batch_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[load_batch_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[load_batch_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_load_batch_statuses_language] PRIMARY KEY CLUSTERED 
(
	[load_batch_statuses_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[load_batch_types]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[load_batch_types](
	[load_batch_type_id] [int] NOT NULL,
	[load_batch_type] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_load_batch_types] PRIMARY KEY CLUSTERED 
(
	[load_batch_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[load_card_failed]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[load_card_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[load_card_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_load_card_statuses_language] PRIMARY KEY CLUSTERED 
(
	[load_card_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[mac_index_keys]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[masterkeys]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[date_created] [datetimeoffset](7) NULL,
	[date_changed] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_masterkeys] PRIMARY KEY CLUSTERED 
(
	[masterkey_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[notification_batch_log]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[notification_batch_log](
	[added_time] [datetimeoffset](7) NOT NULL,
	[issuer_id] [int] NOT NULL,
	[dist_batch_id] [int] NOT NULL,
	[dist_batch_statuses_id] [int] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[channel_id] [int] NOT NULL,
	[notification_text] [varbinary](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[notification_batch_messages]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[notification_batch_messages](
	[issuer_id] [int] NOT NULL,
	[dist_batch_type_id] [int] NOT NULL,
	[dist_batch_statuses_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[channel_id] [int] NOT NULL,
	[notification_text] [varchar](max) NOT NULL,
	[subject_text] [varchar](max) NOT NULL,
	[from_address] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_notification_messages] PRIMARY KEY NONCLUSTERED 
(
	[issuer_id] ASC,
	[dist_batch_statuses_id] ASC,
	[language_id] ASC,
	[channel_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[notification_batch_outbox]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[notification_batch_outbox](
	[batch_message_id] [uniqueidentifier] NOT NULL,
	[added_time] [datetimeoffset](7) NOT NULL,
	[dist_batch_id] [bigint] NOT NULL,
	[issuer_id] [int] NOT NULL,
	[dist_batch_statuses_id] [int] NOT NULL,
	[dist_batch_type_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[channel_id] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[notification_branch_log]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[notification_branch_log](
	[added_time] [datetimeoffset](7) NOT NULL,
	[card_id] [bigint] NOT NULL,
	[issuer_id] [int] NOT NULL,
	[branch_card_statuses_id] [int] NOT NULL,
	[channel_id] [int] NOT NULL,
	[notification_text] [varbinary](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[notification_branch_messages]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[notification_branch_messages](
	[issuer_id] [int] NOT NULL,
	[branch_card_statuses_id] [int] NOT NULL,
	[card_issue_method_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[channel_id] [int] NOT NULL,
	[notification_text] [varchar](max) NOT NULL,
	[subject_text] [varchar](max) NOT NULL,
	[from_address] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_notification_branch_messages] PRIMARY KEY NONCLUSTERED 
(
	[issuer_id] ASC,
	[branch_card_statuses_id] ASC,
	[card_issue_method_id] ASC,
	[language_id] ASC,
	[channel_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[notification_branch_outbox]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[notification_branch_outbox](
	[branch_message_id] [uniqueidentifier] NOT NULL,
	[added_time] [datetimeoffset](7) NOT NULL,
	[card_id] [bigint] NOT NULL,
	[issuer_id] [int] NOT NULL,
	[branch_card_statuses_id] [int] NOT NULL,
	[card_issue_method_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[channel_id] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pin_batch]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_batch](
	[pin_batch_id] [bigint] IDENTITY(1,1) NOT NULL,
	[no_cards] [int] NOT NULL,
	[date_created] [datetimeoffset](7) NOT NULL,
	[pin_batch_reference] [varchar](100) NOT NULL,
	[pin_batch_type_id] [int] NOT NULL,
	[card_issue_method_id] [int] NOT NULL,
	[issuer_id] [int] NOT NULL,
	[branch_id] [int] NULL,
 CONSTRAINT [PK_pin_batch] PRIMARY KEY CLUSTERED 
(
	[pin_batch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_batch_card_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_batch_card_statuses](
	[pin_batch_card_statuses_id] [int] NOT NULL,
	[pin_batch_card_statuses_name] [varchar](250) NOT NULL,
 CONSTRAINT [PK_pin_batch_card_statuses] PRIMARY KEY CLUSTERED 
(
	[pin_batch_card_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_batch_cards]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pin_batch_cards](
	[pin_batch_id] [bigint] NOT NULL,
	[card_id] [bigint] NOT NULL,
	[pin_batch_cards_statuses_id] [int] NOT NULL,
 CONSTRAINT [PK_pin_batch_cards] PRIMARY KEY CLUSTERED 
(
	[pin_batch_id] ASC,
	[card_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pin_batch_status]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_batch_status](
	[pin_batch_id] [bigint] NOT NULL,
	[pin_batch_statuses_id] [int] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[status_notes] [varchar](250) NULL,
 CONSTRAINT [PK_pin_batch_status] PRIMARY KEY CLUSTERED 
(
	[pin_batch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_batch_status_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_batch_status_audit](
	[pin_batch_status_id] [bigint] IDENTITY(1,1) NOT NULL,
	[pin_batch_id] [bigint] NOT NULL,
	[pin_batch_statuses_id] [int] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[status_notes] [varchar](250) NULL,
 CONSTRAINT [PK_pin_batch_status_audit] PRIMARY KEY CLUSTERED 
(
	[pin_batch_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_batch_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_batch_statuses](
	[pin_batch_statuses_id] [int] NOT NULL,
	[pin_batch_statuses_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_pin_batch_statuses] PRIMARY KEY CLUSTERED 
(
	[pin_batch_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_batch_statuses_flow]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pin_batch_statuses_flow](
	[pin_batch_type_id] [int] NOT NULL,
	[pin_batch_statuses_id] [int] NOT NULL,
	[card_issue_method_id] [int] NOT NULL,
	[user_role_id] [int] NOT NULL,
	[flow_pin_batch_statuses_id] [int] NOT NULL,
	[flow_pin_batch_type_id] [int] NOT NULL,
	[main_menu_id] [smallint] NULL,
	[sub_menu_id] [smallint] NULL,
	[sub_menu_order] [smallint] NULL,
	[reject_pin_batch_statuses_id] [int] NULL,
	[reject_pin_card_statuses_id] [int] NULL,
	[flow_pin_card_statuses_id] [int] NULL,
 CONSTRAINT [PK_pin_batch_statuses_flow] PRIMARY KEY CLUSTERED 
(
	[pin_batch_type_id] ASC,
	[pin_batch_statuses_id] ASC,
	[card_issue_method_id] ASC,
	[flow_pin_batch_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pin_batch_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_batch_statuses_language](
	[pin_batch_statuses_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NOT NULL,
 CONSTRAINT [PK_pin_batch_statuses_language] PRIMARY KEY CLUSTERED 
(
	[pin_batch_statuses_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_batch_type]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_batch_type](
	[pin_batch_type_id] [int] NOT NULL,
	[pin_batch_type_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_pin_batch_type] PRIMARY KEY CLUSTERED 
(
	[pin_batch_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_block_format]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pin_block_format](
	[pin_block_formatid] [int] NOT NULL,
	[pin_block_format] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_pin_block_format] PRIMARY KEY CLUSTERED 
(
	[pin_block_formatid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pin_calc_methods]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_calc_methods](
	[pin_calc_method_id] [int] NOT NULL,
	[pin_calc_method_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_pin_calc_method] PRIMARY KEY CLUSTERED 
(
	[pin_calc_method_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_mailer]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[printed_date] [datetimeoffset](7) NULL,
	[reprinted_date] [datetimeoffset](7) NULL,
	[reprint_request_YN] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_mailer_reprint]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_mailer_reprint](
	[card_id] [bigint] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[pin_mailer_reprint_status_id] [int] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[comments] [varchar](1000) NULL,
 CONSTRAINT [PK_pin_mailer_reprint] PRIMARY KEY CLUSTERED 
(
	[card_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_mailer_reprint_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_mailer_reprint_audit](
	[pin_mailer_reprint_id] [bigint] IDENTITY(1,1) NOT NULL,
	[card_id] [bigint] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[pin_mailer_reprint_status_id] [int] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[comments] [varchar](1000) NULL,
 CONSTRAINT [PK_pin_mailer_reprint_audit] PRIMARY KEY CLUSTERED 
(
	[pin_mailer_reprint_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_mailer_reprint_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_mailer_reprint_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
/****** Object:  Table [dbo].[pin_reissue]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_reissue](
	[issuer_id] [int] NOT NULL,
	[branch_id] [int] NOT NULL,
	[product_id] [int] NOT NULL,
	[pan] [varbinary](max) NOT NULL,
	[reissue_date] [datetimeoffset](7) NOT NULL,
	[operator_user_id] [bigint] NOT NULL,
	[authorise_user_id] [bigint] NULL,
	[failed] [bit] NOT NULL,
	[notes] [varchar](500) NOT NULL,
	[pin_reissue_id] [bigint] IDENTITY(1,1) NOT NULL,
	[mobile_number] [varbinary](max) NULL,
	[primary_index_number] [varbinary](max) NULL,
	[request_expiry] [datetimeoffset](7) NOT NULL,
	[pin_reissue_type_id] [int] NULL,
 CONSTRAINT [PK_pin_reissue_id] PRIMARY KEY CLUSTERED 
(
	[pin_reissue_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_reissue_status]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_reissue_status](
	[pin_reissue_id] [bigint] NOT NULL,
	[pin_reissue_statuses_id] [int] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[audit_workstation] [varchar](100) NOT NULL,
	[comments] [varchar](1000) NULL,
 CONSTRAINT [PK_pin_reissue_status] PRIMARY KEY CLUSTERED 
(
	[pin_reissue_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_reissue_status_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pin_reissue_status_audit](
	[pin_reissue_status_id] [bigint] IDENTITY(1,1) NOT NULL,
	[pin_reissue_id] [bigint] NOT NULL,
	[pin_reissue_statuses_id] [int] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[audit_workstation] [varchar](100) NOT NULL,
	[comments] [varchar](1000) NULL,
 CONSTRAINT [PK_pin_reissue_status_audit] PRIMARY KEY CLUSTERED 
(
	[pin_reissue_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_reissue_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[pin_reissue_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[print_batch]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[print_batch](
	[print_batch_id] [bigint] IDENTITY(1,1) NOT NULL,
	[branch_id] [int] NOT NULL,
	[date_created] [datetimeoffset](7) NULL,
	[print_batch_reference] [varchar](50) NULL,
	[card_issue_method_id] [int] NOT NULL,
	[issuer_id] [int] NULL,
	[origin_branch_id] [int] NULL,
	[no_of_requests] [int] NULL,
 CONSTRAINT [PK_print_batch_requests] PRIMARY KEY CLUSTERED 
(
	[print_batch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[print_batch_request_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[print_batch_request_statuses](
	[print_batch_request_status_id] [int] NOT NULL,
	[print_batch_request_status] [varchar](250) NULL,
 CONSTRAINT [PK_print_batch_request_statuses] PRIMARY KEY CLUSTERED 
(
	[print_batch_request_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[print_batch_request_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[print_batch_request_statuses_language](
	[print_batch_request_status_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NULL,
 CONSTRAINT [PK_print_batch_request_statuses_language] PRIMARY KEY CLUSTERED 
(
	[print_batch_request_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[print_batch_requests]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[print_batch_requests](
	[print_batch_id] [bigint] NOT NULL,
	[request_id] [bigint] NOT NULL,
 CONSTRAINT [PK_print_batch_requests_1] PRIMARY KEY CLUSTERED 
(
	[print_batch_id] ASC,
	[request_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[print_batch_status]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[print_batch_status](
	[print_batch_id] [bigint] NOT NULL,
	[print_batch_statuses_id] [int] NOT NULL,
	[user_id] [bigint] NULL,
	[status_date] [datetimeoffset](7) NULL,
	[status_notes] [varchar](150) NULL,
 CONSTRAINT [PK_print_batch_status] PRIMARY KEY CLUSTERED 
(
	[print_batch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[print_batch_status_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[print_batch_status_audit](
	[print_batch_status_id] [bigint] IDENTITY(1,1) NOT NULL,
	[print_batch_id] [bigint] NULL,
	[print_batch_statuses_id] [int] NULL,
	[user_id] [bigint] NULL,
	[status_date] [varchar](150) NULL,
 CONSTRAINT [PK_print_batch_status_audit] PRIMARY KEY CLUSTERED 
(
	[print_batch_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[print_batch_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[print_batch_statuses](
	[print_batch_statuses_id] [int] NOT NULL,
	[print_batch_statuses] [varchar](150) NULL,
 CONSTRAINT [PK_print_batch_statuses] PRIMARY KEY CLUSTERED 
(
	[print_batch_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[print_batch_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[print_batch_statuses_language](
	[print_batch_statuses_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](150) NULL,
 CONSTRAINT [PK_print_batch_statuses_language] PRIMARY KEY CLUSTERED 
(
	[language_id] ASC,
	[print_batch_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[print_field_types]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[print_field_types](
	[print_field_type_id] [int] NOT NULL,
	[print_field_name] [varchar](50) NULL,
 CONSTRAINT [PK_print_field_types] PRIMARY KEY CLUSTERED 
(
	[print_field_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[print_field_types_language]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[print_field_types_language](
	[print_field_type_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](1000) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[product_currency]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product_currency](
	[product_id] [int] NOT NULL,
	[currency_id] [int] NOT NULL,
	[is_base] [bit] NOT NULL,
	[usr_field_name_1] [varchar](250) NULL,
	[usr_field_val_1] [varchar](250) NULL,
	[usr_field_name_2] [varchar](250) NULL,
	[usr_field_val_2] [varchar](250) NULL,
 CONSTRAINT [PK_product_currency] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC,
	[currency_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[product_external_system]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[product_fee_accounting]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product_fee_accounting](
	[fee_accounting_id] [int] IDENTITY(1,1) NOT NULL,
	[fee_accounting_name] [nvarchar](100) NOT NULL,
	[issuer_id] [int] NOT NULL,
	[fee_revenue_account_no] [varbinary](max) NOT NULL,
	[fee_revenue_account_type_id] [int] NOT NULL,
	[fee_revenue_branch_code] [nvarchar](10) NULL,
	[fee_revenue_narration_en] [nvarchar](150) NOT NULL,
	[fee_revenue_narration_fr] [nvarchar](150) NOT NULL,
	[fee_revenue_narration_pt] [nvarchar](150) NOT NULL,
	[fee_revenue_narration_es] [nvarchar](150) NOT NULL,
	[vat_account_no] [varbinary](max) NOT NULL,
	[vat_account_type_id] [int] NOT NULL,
	[vat_account_branch_code] [nvarchar](10) NULL,
	[vat_narration_en] [nvarchar](150) NOT NULL,
	[vat_narration_fr] [nvarchar](150) NOT NULL,
	[vat_narration_pt] [nvarchar](150) NOT NULL,
	[vat_narration_es] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_product_fee_accounting] PRIMARY KEY CLUSTERED 
(
	[fee_accounting_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[product_fee_charge]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product_fee_charge](
	[fee_detail_id] [int] NOT NULL,
	[currency_id] [int] NOT NULL,
	[card_issue_reason_id] [int] NOT NULL,
	[fee_charge] [decimal](10, 4) NOT NULL,
	[date_created] [datetimeoffset](7) NOT NULL,
	[vat] [decimal](7, 4) NOT NULL,
 CONSTRAINT [PK_product_fee_charge] PRIMARY KEY CLUSTERED 
(
	[fee_detail_id] ASC,
	[currency_id] ASC,
	[card_issue_reason_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[product_fee_detail]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product_fee_detail](
	[fee_scheme_id] [int] NOT NULL,
	[fee_detail_id] [int] IDENTITY(1,1) NOT NULL,
	[fee_detail_name] [varchar](100) NOT NULL,
	[effective_from] [datetimeoffset](7) NOT NULL,
	[fee_waiver_YN] [bit] NOT NULL,
	[fee_editable_YN] [bit] NOT NULL,
	[deleted_yn] [bit] NOT NULL,
	[effective_to] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_fee_detail] PRIMARY KEY CLUSTERED 
(
	[fee_detail_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[product_fee_scheme]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product_fee_scheme](
	[fee_scheme_id] [int] IDENTITY(1,1) NOT NULL,
	[issuer_id] [int] NOT NULL,
	[fee_scheme_name] [varchar](100) NOT NULL,
	[deleted_yn] [bit] NOT NULL,
	[fee_accounting_id] [int] NOT NULL,
 CONSTRAINT [PK_product_fee] PRIMARY KEY CLUSTERED 
(
	[fee_scheme_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[product_fee_type]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product_fee_type](
	[fee_type_id] [int] NOT NULL,
	[fee_type_name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_product_fee_type] PRIMARY KEY CLUSTERED 
(
	[fee_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[product_fields]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product_fields](
	[product_field_id] [int] IDENTITY(1,1) NOT NULL,
	[product_id] [int] NOT NULL,
	[field_name] [varchar](100) NOT NULL,
	[print_field_type_id] [int] NOT NULL,
	[X] [decimal](18, 2) NULL,
	[Y] [decimal](18, 2) NULL,
	[width] [decimal](18, 2) NULL,
	[height] [decimal](18, 2) NULL,
	[font] [varchar](50) NULL,
	[font_size] [int] NULL,
	[mapped_name] [varchar](max) NULL,
	[editable] [bit] NOT NULL,
	[deleted] [bit] NOT NULL,
	[label] [varchar](100) NULL,
	[max_length] [int] NOT NULL,
 CONSTRAINT [PK_product_fields] PRIMARY KEY CLUSTERED 
(
	[product_field_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[product_interface]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product_interface](
	[interface_type_id] [int] NOT NULL,
	[product_id] [int] NOT NULL,
	[connection_parameter_id] [int] NOT NULL,
	[interface_area] [int] NOT NULL,
	[interface_guid] [char](36) NULL,
 CONSTRAINT [PK_product_interface] PRIMARY KEY CLUSTERED 
(
	[interface_type_id] ASC,
	[product_id] ASC,
	[interface_area] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[product_issue_reason]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product_issue_reason](
	[product_id] [int] NOT NULL,
	[card_issue_reason_id] [int] NOT NULL,
 CONSTRAINT [PK_product_issue_reason] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC,
	[card_issue_reason_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[product_load_type]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[product_load_type_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[product_service_requet_code1]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product_service_requet_code1](
	[src1_id] [int] NOT NULL,
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_product_service_requet_code1] PRIMARY KEY CLUSTERED 
(
	[src1_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[product_service_requet_code2]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product_service_requet_code2](
	[src2_id] [int] NOT NULL,
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_product_service_requet_code2] PRIMARY KEY CLUSTERED 
(
	[src2_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[product_service_requet_code3]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product_service_requet_code3](
	[src3_id] [int] NOT NULL,
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_product_service_requet_code3] PRIMARY KEY CLUSTERED 
(
	[src3_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[products_account_types]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[products_account_types](
	[product_id] [int] NOT NULL,
	[account_type_id] [int] NOT NULL,
 CONSTRAINT [PK_products_account_types] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC,
	[account_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[remote_component_logging]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[remote_component_logging](
	[date_logged] [datetimeoffset](7) NOT NULL,
	[remote_address] [varchar](150) NOT NULL,
	[token] [varchar](max) NOT NULL,
	[request] [varchar](max) NOT NULL,
	[method_call_id] [int] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[remote_update_status]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[remote_update_status](
	[remote_update_statuses_id] [int] NOT NULL,
	[card_id] [bigint] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[comments] [nvarchar](max) NOT NULL,
	[remote_component] [varchar](250) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[remote_updated_time] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_remote_update_status] PRIMARY KEY CLUSTERED 
(
	[card_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[remote_update_status_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[remote_update_status_audit](
	[remote_update_status_id] [int] IDENTITY(1,1) NOT NULL,
	[remote_update_statuses_id] [int] NOT NULL,
	[card_id] [bigint] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[comments] [nvarchar](max) NOT NULL,
	[remote_component] [varchar](250) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[remote_updated_time] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_remote_update_status_audit] PRIMARY KEY CLUSTERED 
(
	[remote_update_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[remote_update_statuses]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[remote_update_statuses](
	[remote_update_statuses_id] [int] NOT NULL,
	[remote_update_statuses_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_remote_update_statuses] PRIMARY KEY CLUSTERED 
(
	[remote_update_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[remote_update_statuses_language]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[remote_update_statuses_language](
	[remote_update_statuses_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_remote_update_statuses_language] PRIMARY KEY CLUSTERED 
(
	[remote_update_statuses_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[report_fields]    Script Date: 2018-09-13 10:10:02 AM ******/
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
/****** Object:  Table [dbo].[report_reportfields]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[report_reportfields](
	[reportid] [int] NOT NULL,
	[reportfieldid] [int] NOT NULL,
	[reportfieldorderno] [int] NULL,
 CONSTRAINT [PK_report_fields] PRIMARY KEY CLUSTERED 
(
	[reportid] ASC,
	[reportfieldid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[reportfields_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
/****** Object:  Table [dbo].[reports]    Script Date: 2018-09-13 10:10:02 AM ******/
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
/****** Object:  Table [dbo].[response_messages]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[sequences]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[sequences](
	[sequence_name] [varchar](100) NOT NULL,
	[last_sequence_number] [bigint] NOT NULL,
	[last_updated] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_sequences] PRIMARY KEY CLUSTERED 
(
	[sequence_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[temp_load_cards_type]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[temp_load_cards_type](
	[card_number] [varchar](20) NULL,
	[card_reference] [varchar](100) NULL,
	[branch_id] [int] NULL,
	[card_sequence] [int] NULL,
	[expiry_date] [datetime2](7) NULL,
	[product_id] [int] NULL,
	[card_issue_method_id] [int] NULL,
	[sub_product_id] [int] NULL,
	[already_loaded] [bit] NULL,
	[card_id] [bigint] NULL,
	[load_batch_type_id] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[terminals]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[date_created] [datetimeoffset](7) NULL,
	[date_changed] [datetimeoffset](7) NULL,
	[password] [varbinary](max) NULL,
	[IsMacUsed] [bit] NULL,
 CONSTRAINT [PK_terminals] PRIMARY KEY CLUSTERED 
(
	[terminal_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[user]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[online] [bit] NOT NULL,
	[employee_id] [varbinary](256) NULL,
	[last_login_date] [datetimeoffset](7) NULL,
	[last_login_attempt] [datetimeoffset](7) NULL,
	[number_of_incorrect_logins] [int] NULL,
	[last_password_changed_date] [datetimeoffset](7) NULL,
	[workstation] [nchar](50) NULL,
	[language_id] [int] NULL,
	[username_index] [varbinary](20) NULL,
	[instant_authorisation_pin] [varbinary](256) NULL,
	[last_authorisation_pin_changed_date] [datetimeoffset](7) NULL,
	[time_zone_utcoffset] [nvarchar](50) NULL,
	[time_zone_id] [varchar](max) NULL,
	[useradmin_user_id] [bigint] NULL,
	[record_datetime] [datetimeoffset](7) NULL,
	[approved_user_id] [bigint] NULL,
	[approved_datetime] [datetimeoffset](7) NULL,
	[authentication_configuration_id] [int] NULL,
 CONSTRAINT [PK_application_user] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[user_admin]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_admin](
	[user_admin_id] [int] IDENTITY(1,1) NOT NULL,
	[PasswordValidPeriod] [int] NOT NULL,
	[PasswordMinLength] [int] NOT NULL,
	[PasswordMaxLength] [int] NOT NULL,
	[PreviousPasswordsCount] [int] NOT NULL,
	[maxInvalidPasswordAttempts] [int] NOT NULL,
	[PasswordAttemptLockoutDuration] [int] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDateTime] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_user_admin] PRIMARY KEY CLUSTERED 
(
	[user_admin_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[user_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user_audit](
	[user_id] [bigint] NOT NULL,
	[user_status_id] [int] NOT NULL,
	[user_gender_id] [int] NOT NULL,
	[username] [varbinary](256) NOT NULL,
	[first_name] [varbinary](256) NOT NULL,
	[last_name] [varbinary](256) NOT NULL,
	[password] [varbinary](256) NOT NULL,
	[user_email] [varchar](100) NOT NULL,
	[online] [bit] NOT NULL,
	[employee_id] [varbinary](256) NULL,
	[last_login_date] [datetimeoffset](7) NULL,
	[last_login_attempt] [datetimeoffset](7) NULL,
	[number_of_incorrect_logins] [int] NULL,
	[last_password_changed_date] [datetimeoffset](7) NULL,
	[workstation] [nchar](50) NULL,
	[language_id] [int] NULL,
	[username_index] [varbinary](20) NULL,
	[instant_authorisation_pin] [varbinary](256) NULL,
	[last_authorisation_pin_changed_date] [datetimeoffset](7) NULL,
	[time_zone_utcoffset] [nvarchar](50) NULL,
	[time_zone_id] [varchar](max) NULL,
	[useradmin_user_id] [bigint] NULL,
	[record_datetime] [datetimeoffset](7) NULL,
	[approved_user_id] [bigint] NULL,
	[approved_datetime] [datetimeoffset](7) NULL,
	[authentication_configuration_id] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[user_gender]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[user_group]    Script Date: 2018-09-13 10:10:02 AM ******/
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
	[can_create] [bit] NOT NULL,
	[can_read] [bit] NOT NULL,
	[can_update] [bit] NOT NULL,
	[can_delete] [bit] NOT NULL,
	[all_branch_access] [bit] NOT NULL,
	[user_group_name] [varchar](50) NOT NULL,
	[mask_screen_pan] [bit] NOT NULL,
	[mask_report_pan] [bit] NOT NULL,
 CONSTRAINT [PK_user_group] PRIMARY KEY CLUSTERED 
(
	[user_group_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[user_groups_branches]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_groups_branches](
	[user_group_id] [int] NOT NULL,
	[branch_id] [int] NOT NULL,
 CONSTRAINT [PK_user_groups_branches] PRIMARY KEY CLUSTERED 
(
	[user_group_id] ASC,
	[branch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[user_password_history]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user_password_history](
	[user_id] [bigint] NOT NULL,
	[password_history] [varbinary](max) NOT NULL,
	[date_changed] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_user_password_history] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC,
	[date_changed] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[user_pending]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user_pending](
	[pending_user_id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_status_id] [int] NOT NULL,
	[user_gender_id] [int] NOT NULL,
	[username] [varbinary](256) NOT NULL,
	[first_name] [varbinary](256) NOT NULL,
	[last_name] [varbinary](256) NOT NULL,
	[password] [varbinary](256) NOT NULL,
	[user_email] [varchar](100) NOT NULL,
	[online] [bit] NOT NULL,
	[employee_id] [varbinary](256) NULL,
	[last_login_date] [datetimeoffset](7) NULL,
	[last_login_attempt] [datetimeoffset](7) NULL,
	[number_of_incorrect_logins] [int] NULL,
	[last_password_changed_date] [datetimeoffset](7) NULL,
	[workstation] [nchar](50) NULL,
	[language_id] [int] NULL,
	[username_index] [varbinary](20) NULL,
	[instant_authorisation_pin] [varbinary](256) NULL,
	[last_authorisation_pin_changed_date] [datetimeoffset](7) NULL,
	[time_zone_utcoffset] [nvarchar](50) NULL,
	[time_zone_id] [varchar](max) NULL,
	[user_id] [bigint] NULL,
	[useradmin_user_id] [bigint] NULL,
	[record_datetime] [datetimeoffset](7) NULL,
	[authentication_configuration_id] [int] NOT NULL,
 CONSTRAINT [PK_application_user_pending] PRIMARY KEY CLUSTERED 
(
	[pending_user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[user_roles]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[user_roles_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_user_roles_language] PRIMARY KEY CLUSTERED 
(
	[user_role_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[user_status]    Script Date: 2018-09-13 10:10:02 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[user_status_language]    Script Date: 2018-09-13 10:10:02 AM ******/
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
 CONSTRAINT [PK_user_status_language] PRIMARY KEY CLUSTERED 
(
	[user_status_id] ASC,
	[language_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[user_to_user_group_audit]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_to_user_group_audit](
	[user_id] [bigint] NOT NULL,
	[user_group_id] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[user_to_user_group_pending]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_to_user_group_pending](
	[pending_user_id] [bigint] NOT NULL,
	[user_group_id] [int] NOT NULL,
 CONSTRAINT [PK_user_to_user_group_pending] PRIMARY KEY CLUSTERED 
(
	[pending_user_id] ASC,
	[user_group_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[users_to_users_groups]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users_to_users_groups](
	[user_id] [bigint] NOT NULL,
	[user_group_id] [int] NOT NULL,
 CONSTRAINT [PK_users_to_users_groups] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC,
	[user_group_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[workstation_keys]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[workstation_keys](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[workstation] [varbinary](max) NOT NULL,
	[key] [varbinary](max) NULL,
	[additional_data] [varbinary](max) NULL,
 CONSTRAINT [PK_workstation_keys] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[zone_keys]    Script Date: 2018-09-13 10:10:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[zone_keys](
	[issuer_id] [int] NOT NULL,
	[zone] [varbinary](max) NOT NULL,
	[final] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_zone_keys] PRIMARY KEY CLUSTERED 
(
	[issuer_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[connection_parameters] ADD  CONSTRAINT [DF__connectio__file___228BC4B3]  DEFAULT ((0)) FOR [file_delete_YN]
GO
ALTER TABLE [dbo].[connection_parameters] ADD  CONSTRAINT [DF__connectio__file___237FE8EC]  DEFAULT ((0)) FOR [file_encryption_type_id]
GO
ALTER TABLE [dbo].[connection_parameters] ADD  CONSTRAINT [DF__connectio__dupli__24740D25]  DEFAULT ((0)) FOR [duplicate_file_check_YN]
GO
ALTER TABLE [dbo].[dist_batch] ADD  DEFAULT ((0)) FOR [dist_version]
GO
ALTER TABLE [dbo].[issuer] ADD  DEFAULT ((0)) FOR [enable_instant_pin_YN]
GO
ALTER TABLE [dbo].[issuer] ADD  DEFAULT ((0)) FOR [authorise_pin_issue_YN]
GO
ALTER TABLE [dbo].[issuer] ADD  DEFAULT ((0)) FOR [authorise_pin_reissue_YN]
GO
ALTER TABLE [dbo].[issuer] ADD  DEFAULT (newid()) FOR [remote_token]
GO
ALTER TABLE [dbo].[issuer_product] ADD  DEFAULT ((1)) FOR [print_issue_card_YN]
GO
ALTER TABLE [dbo].[issuer_product] ADD  CONSTRAINT [DF_issuer_product_allow_manual_uploaded_YN]  DEFAULT ((0)) FOR [allow_manual_uploaded_YN]
GO
ALTER TABLE [dbo].[issuer_product] ADD  CONSTRAINT [DF_issuer_product_allow_reupload_YN]  DEFAULT ((0)) FOR [allow_reupload_YN]
GO
ALTER TABLE [dbo].[issuer_product] ADD  DEFAULT ((0)) FOR [remote_cms_update_YN]
GO
ALTER TABLE [dbo].[issuer_product] ADD  CONSTRAINT [DF_issuer_product_e_pin_request_YN]  DEFAULT ((0)) FOR [e_pin_request_YN]
GO
ALTER TABLE [dbo].[load_batch] ADD  DEFAULT ((1)) FOR [load_batch_type_id]
GO
ALTER TABLE [dbo].[product_fee_charge] ADD  CONSTRAINT [DF_product_fee_charge_vat]  DEFAULT ((0)) FOR [vat]
GO
ALTER TABLE [dbo].[product_fields] ADD  CONSTRAINT [DF_product_fields_max_length]  DEFAULT ((1)) FOR [max_length]
GO
ALTER TABLE [dbo].[remote_component_logging] ADD  DEFAULT (sysdatetimeoffset()) FOR [date_logged]
GO
ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF_user_online]  DEFAULT ((0)) FOR [online]
GO
ALTER TABLE [dbo].[user_audit] ADD  CONSTRAINT [DF_user_audit_online]  DEFAULT ((0)) FOR [online]
GO
ALTER TABLE [dbo].[user_group] ADD  CONSTRAINT [DF_user_group_can_create]  DEFAULT ((0)) FOR [can_create]
GO
ALTER TABLE [dbo].[user_group] ADD  CONSTRAINT [DF_user_group_can_read]  DEFAULT ((0)) FOR [can_read]
GO
ALTER TABLE [dbo].[user_group] ADD  CONSTRAINT [DF_user_group_can_update]  DEFAULT ((0)) FOR [can_update]
GO
ALTER TABLE [dbo].[user_group] ADD  CONSTRAINT [DF_user_group_can_delete]  DEFAULT ((0)) FOR [can_delete]
GO
ALTER TABLE [dbo].[user_group] ADD  CONSTRAINT [DF_user_group_all_branch_access]  DEFAULT ((0)) FOR [all_branch_access]
GO
ALTER TABLE [dbo].[user_pending] ADD  CONSTRAINT [DF_user_pending_online]  DEFAULT ((0)) FOR [online]
GO
ALTER TABLE [dbo].[audit_control]  WITH CHECK ADD  CONSTRAINT [FK_audit_control_audit_action] FOREIGN KEY([audit_action_id])
REFERENCES [dbo].[audit_action] ([audit_action_id])
GO
ALTER TABLE [dbo].[audit_control] CHECK CONSTRAINT [FK_audit_control_audit_action]
GO
ALTER TABLE [dbo].[audit_control]  WITH CHECK ADD  CONSTRAINT [FK_audit_control_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[audit_control] CHECK CONSTRAINT [FK_audit_control_user]
GO
ALTER TABLE [dbo].[auth_configuration_connection_parameters]  WITH CHECK ADD  CONSTRAINT [FK_auth_configuration_connection_parameters_auth_configuration] FOREIGN KEY([authentication_configuration_id])
REFERENCES [dbo].[auth_configuration] ([authentication_configuration_id])
GO
ALTER TABLE [dbo].[auth_configuration_connection_parameters] CHECK CONSTRAINT [FK_auth_configuration_connection_parameters_auth_configuration]
GO
ALTER TABLE [dbo].[auth_configuration_connection_parameters]  WITH CHECK ADD  CONSTRAINT [FK_auth_configuration_connection_parameters_auth_type] FOREIGN KEY([auth_type_id])
REFERENCES [dbo].[auth_type] ([auth_type_id])
GO
ALTER TABLE [dbo].[auth_configuration_connection_parameters] CHECK CONSTRAINT [FK_auth_configuration_connection_parameters_auth_type]
GO
ALTER TABLE [dbo].[auth_configuration_connection_parameters]  WITH CHECK ADD  CONSTRAINT [FK_auth_configuration_connection_parameters_connection_parameters] FOREIGN KEY([connection_parameter_id])
REFERENCES [dbo].[connection_parameters] ([connection_parameter_id])
GO
ALTER TABLE [dbo].[auth_configuration_connection_parameters] CHECK CONSTRAINT [FK_auth_configuration_connection_parameters_connection_parameters]
GO
ALTER TABLE [dbo].[branch]  WITH CHECK ADD  CONSTRAINT [FK_branch_branch_statuses] FOREIGN KEY([branch_status_id])
REFERENCES [dbo].[branch_statuses] ([branch_status_id])
GO
ALTER TABLE [dbo].[branch] CHECK CONSTRAINT [FK_branch_branch_statuses]
GO
ALTER TABLE [dbo].[branch]  WITH CHECK ADD  CONSTRAINT [FK_branch_branch_type] FOREIGN KEY([branch_type_id])
REFERENCES [dbo].[branch_type] ([branch_type_id])
GO
ALTER TABLE [dbo].[branch] CHECK CONSTRAINT [FK_branch_branch_type]
GO
ALTER TABLE [dbo].[branch]  WITH CHECK ADD  CONSTRAINT [FK_branch_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[branch] CHECK CONSTRAINT [FK_branch_issuer]
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
ALTER TABLE [dbo].[branch_card_status]  WITH CHECK ADD  CONSTRAINT [FK_branch_card_status_branch_id] FOREIGN KEY([branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO
ALTER TABLE [dbo].[branch_card_status] CHECK CONSTRAINT [FK_branch_card_status_branch_id]
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
ALTER TABLE [dbo].[branch_card_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_branch_card_statuses_language_branch_status] FOREIGN KEY([branch_card_statuses_id])
REFERENCES [dbo].[branch_card_statuses] ([branch_card_statuses_id])
GO
ALTER TABLE [dbo].[branch_card_statuses_language] CHECK CONSTRAINT [FK_branch_card_statuses_language_branch_status]
GO
ALTER TABLE [dbo].[branch_card_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_branch_card_statuses_language_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[branch_card_statuses_language] CHECK CONSTRAINT [FK_branch_card_statuses_language_language_id]
GO

ALTER TABLE [dbo].[branch_type_language]  WITH CHECK ADD  CONSTRAINT [FK_branch_type_language_language] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[branch_type_language] CHECK CONSTRAINT [FK_branch_type_language_language]
GO
ALTER TABLE [dbo].[card_fee_charge_status_language]  WITH CHECK ADD  CONSTRAINT [FK_card_fee_charge_status_language_card_fee_charge_status] FOREIGN KEY([card_fee_charge_status_id])
REFERENCES [dbo].[card_fee_charge_status] ([card_fee_charge_status_id])
GO
ALTER TABLE [dbo].[card_fee_charge_status_language] CHECK CONSTRAINT [FK_card_fee_charge_status_language_card_fee_charge_status]
GO
ALTER TABLE [dbo].[card_priority_language]  WITH CHECK ADD  CONSTRAINT [FK_card_priority_language] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[card_priority_language] CHECK CONSTRAINT [FK_card_priority_language]
GO
ALTER TABLE [dbo].[cards]  WITH CHECK ADD  CONSTRAINT [FK_cards_cards] FOREIGN KEY([card_id])
REFERENCES [dbo].[cards] ([card_id])
GO
ALTER TABLE [dbo].[cards] CHECK CONSTRAINT [FK_cards_cards]
GO
ALTER TABLE [dbo].[cards]  WITH CHECK ADD  CONSTRAINT [FK_cards_fee_charged] FOREIGN KEY([fee_id])
REFERENCES [dbo].[fee_charged] ([fee_id])
GO
ALTER TABLE [dbo].[cards] CHECK CONSTRAINT [FK_cards_fee_charged]
GO
ALTER TABLE [dbo].[connection_parameters]  WITH CHECK ADD  CONSTRAINT [FK_connection_parameters_connection_parameter_type] FOREIGN KEY([connection_parameter_type_id])
REFERENCES [dbo].[connection_parameter_type] ([connection_parameter_type_id])
GO
ALTER TABLE [dbo].[connection_parameters] CHECK CONSTRAINT [FK_connection_parameters_connection_parameter_type]
GO
ALTER TABLE [dbo].[connection_parameters]  WITH CHECK ADD  CONSTRAINT [FK_connection_parameters_file_encryption_type] FOREIGN KEY([file_encryption_type_id])
REFERENCES [dbo].[file_encryption_type] ([file_encryption_type_id])
GO
ALTER TABLE [dbo].[connection_parameters] CHECK CONSTRAINT [FK_connection_parameters_file_encryption_type]
GO
ALTER TABLE [dbo].[connection_parameters_additionaldata]  WITH CHECK ADD  CONSTRAINT [FK_connection_parameters _additionaldata_connection_parameters] FOREIGN KEY([connection_parameter_id])
REFERENCES [dbo].[connection_parameters] ([connection_parameter_id])
GO
ALTER TABLE [dbo].[connection_parameters_additionaldata] CHECK CONSTRAINT [FK_connection_parameters _additionaldata_connection_parameters]
GO
ALTER TABLE [dbo].[customer_account]  WITH NOCHECK ADD  CONSTRAINT [FK_customer_account_branch] FOREIGN KEY([domicile_branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO
ALTER TABLE [dbo].[customer_account] NOCHECK CONSTRAINT [FK_customer_account_branch]
GO
ALTER TABLE [dbo].[customer_account]  WITH NOCHECK ADD  CONSTRAINT [FK_customer_account_card_issue_reason] FOREIGN KEY([card_issue_reason_id])
REFERENCES [dbo].[card_issue_reason] ([card_issue_reason_id])
GO
ALTER TABLE [dbo].[customer_account] NOCHECK CONSTRAINT [FK_customer_account_card_issue_reason]
GO

ALTER TABLE [dbo].[customer_account]  WITH NOCHECK ADD  CONSTRAINT [FK_customer_account_type] FOREIGN KEY([account_type_id])
REFERENCES [dbo].[customer_account_type] ([account_type_id])
GO
ALTER TABLE [dbo].[customer_account] NOCHECK CONSTRAINT [FK_customer_account_type]
GO
ALTER TABLE [dbo].[customer_account]  WITH NOCHECK ADD  CONSTRAINT [FK_customer_account_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[customer_account] NOCHECK CONSTRAINT [FK_customer_account_user]
GO
ALTER TABLE [dbo].[customer_account]  WITH CHECK ADD  CONSTRAINT [FK_customer_customer_title] FOREIGN KEY([customer_title_id])
REFERENCES [dbo].[customer_title] ([customer_title_id])
GO
ALTER TABLE [dbo].[customer_account] CHECK CONSTRAINT [FK_customer_customer_title]
GO
ALTER TABLE [dbo].[customer_account]  WITH NOCHECK ADD  CONSTRAINT [FK_customer_residency] FOREIGN KEY([resident_id])
REFERENCES [dbo].[customer_residency] ([resident_id])
GO
ALTER TABLE [dbo].[customer_account] NOCHECK CONSTRAINT [FK_customer_residency]
GO
ALTER TABLE [dbo].[customer_account]  WITH NOCHECK ADD  CONSTRAINT [FK_customer_type] FOREIGN KEY([customer_type_id])
REFERENCES [dbo].[customer_type] ([customer_type_id])
GO
ALTER TABLE [dbo].[customer_account] NOCHECK CONSTRAINT [FK_customer_type]
GO
ALTER TABLE [dbo].[customer_account_cards]  WITH CHECK ADD  CONSTRAINT [FK_customer_account_cards_cards] FOREIGN KEY([customer_account_id])
REFERENCES [dbo].[customer_account] ([customer_account_id])
GO


ALTER TABLE [dbo].[customer_account_requests]  WITH CHECK ADD  CONSTRAINT [FK_customer_account_requests_customer_account] FOREIGN KEY([customer_account_id])
REFERENCES [dbo].[customer_account] ([customer_account_id])
GO
ALTER TABLE [dbo].[customer_account_requests] CHECK CONSTRAINT [FK_customer_account_requests_customer_account]
GO
ALTER TABLE [dbo].[customer_account_requests]  WITH CHECK ADD  CONSTRAINT [FK_customer_account_requests_hybrid_requests] FOREIGN KEY([request_id])
REFERENCES [dbo].[hybrid_requests] ([request_id])
GO
ALTER TABLE [dbo].[customer_account_requests] CHECK CONSTRAINT [FK_customer_account_requests_hybrid_requests]
GO
ALTER TABLE [dbo].[customer_fields]  WITH CHECK ADD  CONSTRAINT [FK_customer_fields_customer_account] FOREIGN KEY([customer_account_id])
REFERENCES [dbo].[customer_account] ([customer_account_id])
GO
ALTER TABLE [dbo].[customer_fields] CHECK CONSTRAINT [FK_customer_fields_customer_account]
GO
ALTER TABLE [dbo].[load_card_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_load_card_statuses_language_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[load_card_statuses_language] CHECK CONSTRAINT [FK_load_card_statuses_language_language_id]
GO

ALTER TABLE [dbo].[load_card_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_load_card_statuses_language_load_card_status_id] FOREIGN KEY([load_card_status_id])
REFERENCES [dbo].[load_card_statuses] ([load_card_status_id])
GO

ALTER TABLE [dbo].[load_card_statuses_language] CHECK CONSTRAINT [FK_load_card_statuses_language_load_card_status_id]
GO
ALTER TABLE [dbo].[customer_fields]  WITH CHECK ADD  CONSTRAINT [FK_customer_fields_product_fields] FOREIGN KEY([product_field_id])
REFERENCES [dbo].[product_fields] ([product_field_id])
GO
ALTER TABLE [dbo].[customer_fields] CHECK CONSTRAINT [FK_customer_fields_product_fields]
GO
ALTER TABLE [dbo].[customer_image_fields]  WITH CHECK ADD  CONSTRAINT [FK_customer_image_fields_customer_account] FOREIGN KEY([customer_account_id])
REFERENCES [dbo].[customer_account] ([customer_account_id])
GO
ALTER TABLE [dbo].[customer_image_fields] CHECK CONSTRAINT [FK_customer_image_fields_customer_account]
GO
ALTER TABLE [dbo].[customer_image_fields]  WITH CHECK ADD  CONSTRAINT [FK_customer_image_fields_product_fields] FOREIGN KEY([product_field_id])
REFERENCES [dbo].[product_fields] ([product_field_id])
GO
ALTER TABLE [dbo].[customer_image_fields] CHECK CONSTRAINT [FK_customer_image_fields_product_fields]
GO
ALTER TABLE [dbo].[dist_batch]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_card_issue_method] FOREIGN KEY([card_issue_method_id])
REFERENCES [dbo].[card_issue_method] ([card_issue_method_id])
GO
ALTER TABLE [dbo].[dist_batch] CHECK CONSTRAINT [FK_dist_batch_card_issue_method]
GO
ALTER TABLE [dbo].[dist_batch]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_dist_batch_type] FOREIGN KEY([dist_batch_type_id])
REFERENCES [dbo].[dist_batch_type] ([dist_batch_type_id])
GO
ALTER TABLE [dbo].[dist_batch] CHECK CONSTRAINT [FK_dist_batch_dist_batch_type]
GO
ALTER TABLE [dbo].[dist_batch]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[dist_batch] CHECK CONSTRAINT [FK_dist_batch_issuer]
GO
ALTER TABLE [dbo].[dist_batch]  WITH CHECK ADD  CONSTRAINT [FK_distribution_batch_branch] FOREIGN KEY([branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO
ALTER TABLE [dbo].[dist_batch] CHECK CONSTRAINT [FK_distribution_batch_branch]
GO
ALTER TABLE [dbo].[dist_batch]  WITH CHECK ADD  CONSTRAINT [FK_distribution_batch_origin_branch] FOREIGN KEY([origin_branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO
ALTER TABLE [dbo].[dist_batch] CHECK CONSTRAINT [FK_distribution_batch_origin_branch]
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
ALTER TABLE [dbo].[dist_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_status_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[dist_batch_status] CHECK CONSTRAINT [FK_dist_batch_status_user]
GO
ALTER TABLE [dbo].[dist_card_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_dist_card_status_dist_card_status_id] FOREIGN KEY([dist_card_status_id])
REFERENCES [dbo].[dist_card_statuses] ([dist_card_status_id])
GO
ALTER TABLE [dbo].[dist_card_statuses_language] CHECK CONSTRAINT [FK_dist_card_status_dist_card_status_id]
GO
ALTER TABLE [dbo].[dist_card_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_dist_card_status_language] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[dist_card_statuses_language] CHECK CONSTRAINT [FK_dist_card_status_language]
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
ALTER TABLE [dbo].[external_system_fields]  WITH CHECK ADD  CONSTRAINT [FK_external_system_fields_external_systems] FOREIGN KEY([external_system_id])
REFERENCES [dbo].[external_systems] ([external_system_id])
GO
ALTER TABLE [dbo].[external_system_fields] CHECK CONSTRAINT [FK_external_system_fields_external_systems]
GO
ALTER TABLE [dbo].[external_systems]  WITH CHECK ADD  CONSTRAINT [FK_external_systems_external_system_types] FOREIGN KEY([external_system_type_id])
REFERENCES [dbo].[external_system_types] ([external_system_type_id])
GO
ALTER TABLE [dbo].[external_systems] CHECK CONSTRAINT [FK_external_systems_external_system_types]
GO
ALTER TABLE [dbo].[file_history]  WITH CHECK ADD  CONSTRAINT [FK_file_history_file_history] FOREIGN KEY([file_type_id])
REFERENCES [dbo].[file_types] ([file_type_id])
GO
ALTER TABLE [dbo].[file_history] CHECK CONSTRAINT [FK_file_history_file_history]
GO
ALTER TABLE [dbo].[file_history]  WITH CHECK ADD  CONSTRAINT [FK_file_history_file_load] FOREIGN KEY([file_load_id])
REFERENCES [dbo].[file_load] ([file_load_id])
GO
ALTER TABLE [dbo].[file_history] CHECK CONSTRAINT [FK_file_history_file_load]
GO
ALTER TABLE [dbo].[file_history]  WITH CHECK ADD  CONSTRAINT [FK_file_history_file_statuses] FOREIGN KEY([file_status_id])
REFERENCES [dbo].[file_statuses] ([file_status_id])
GO
ALTER TABLE [dbo].[file_history] CHECK CONSTRAINT [FK_file_history_file_statuses]
GO
ALTER TABLE [dbo].[file_history]  WITH CHECK ADD  CONSTRAINT [FK_file_history_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[file_history] CHECK CONSTRAINT [FK_file_history_issuer]
GO
ALTER TABLE [dbo].[hybrid_request_status]  WITH CHECK ADD  CONSTRAINT [FK_hybrid_request_status_branch] FOREIGN KEY([branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO
ALTER TABLE [dbo].[hybrid_request_status] CHECK CONSTRAINT [FK_hybrid_request_status_branch]
GO
ALTER TABLE [dbo].[hybrid_request_status]  WITH CHECK ADD  CONSTRAINT [FK_hybrid_request_status_hybrid_request_statuses] FOREIGN KEY([hybrid_request_statuses_id])
REFERENCES [dbo].[hybrid_request_statuses] ([hybrid_request_statuses_id])
GO
ALTER TABLE [dbo].[hybrid_request_status] CHECK CONSTRAINT [FK_hybrid_request_status_hybrid_request_statuses]
GO
ALTER TABLE [dbo].[hybrid_request_status]  WITH CHECK ADD  CONSTRAINT [FK_hybrid_request_status_hybrid_requests] FOREIGN KEY([request_id])
REFERENCES [dbo].[hybrid_requests] ([request_id])
GO
ALTER TABLE [dbo].[hybrid_request_status] CHECK CONSTRAINT [FK_hybrid_request_status_hybrid_requests]
GO
ALTER TABLE [dbo].[hybrid_request_status]  WITH CHECK ADD  CONSTRAINT [FK_hybrid_request_status_users] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[hybrid_request_status] CHECK CONSTRAINT [FK_hybrid_request_status_users]
GO
ALTER TABLE [dbo].[hybrid_request_status]  WITH CHECK ADD  CONSTRAINT [FK_hybrid_request_status_users_1] FOREIGN KEY([operator_user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[hybrid_request_status] CHECK CONSTRAINT [FK_hybrid_request_status_users_1]
GO
ALTER TABLE [dbo].[hybrid_request_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_hybrid_request_statuses_language_hybrid_request_status] FOREIGN KEY([hybrid_request_statuses_id])
REFERENCES [dbo].[hybrid_request_statuses] ([hybrid_request_statuses_id])
GO
ALTER TABLE [dbo].[hybrid_request_statuses_language] CHECK CONSTRAINT [FK_hybrid_request_statuses_language_hybrid_request_status]
GO
ALTER TABLE [dbo].[integration_cardnumbers]  WITH CHECK ADD  CONSTRAINT [FK_integration_cardnumbers_issuer_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO
ALTER TABLE [dbo].[integration_cardnumbers] CHECK CONSTRAINT [FK_integration_cardnumbers_issuer_product]
GO
ALTER TABLE [dbo].[integration_fields]  WITH CHECK ADD  CONSTRAINT [FK_integration_fields_integration_object] FOREIGN KEY([integration_id], [integration_object_id])
REFERENCES [dbo].[integration_object] ([integration_id], [integration_object_id])
GO
ALTER TABLE [dbo].[integration_fields] CHECK CONSTRAINT [FK_integration_fields_integration_object]
GO
ALTER TABLE [dbo].[integration_object]  WITH CHECK ADD  CONSTRAINT [FK_integration_object_integration] FOREIGN KEY([integration_id])
REFERENCES [dbo].[integration] ([integration_id])
GO
ALTER TABLE [dbo].[integration_object] CHECK CONSTRAINT [FK_integration_object_integration]
GO
ALTER TABLE [dbo].[integration_responses]  WITH CHECK ADD  CONSTRAINT [FK_integration_responses_integration_fields] FOREIGN KEY([integration_id], [integration_object_id], [integration_field_id])
REFERENCES [dbo].[integration_fields] ([integration_id], [integration_object_id], [integration_field_id])
GO
ALTER TABLE [dbo].[integration_responses] CHECK CONSTRAINT [FK_integration_responses_integration_fields]
GO
ALTER TABLE [dbo].[integration_responses_language]  WITH CHECK ADD  CONSTRAINT [FK_integration_responses_language_integration_responses] FOREIGN KEY([integration_id], [integration_object_id], [integration_field_id], [integration_response_id])
REFERENCES [dbo].[integration_responses] ([integration_id], [integration_object_id], [integration_field_id], [integration_response_id])
GO
ALTER TABLE [dbo].[integration_responses_language] CHECK CONSTRAINT [FK_integration_responses_language_integration_responses]
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
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_status_flow_issuer_product] FOREIGN KEY([production_dist_batch_status_flow])
REFERENCES [dbo].[dist_batch_status_flow] ([dist_batch_status_flow_id])
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [FK_dist_batch_status_flow_issuer_product]
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
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [FK_issuer_product_pin_block_format] FOREIGN KEY([pin_block_formatid])
REFERENCES [dbo].[pin_block_format] ([pin_block_formatid])
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [FK_issuer_product_pin_block_format]
GO
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [FK_issuer_product_pin_calc_methods] FOREIGN KEY([pin_calc_method_id])
REFERENCES [dbo].[pin_calc_methods] ([pin_calc_method_id])
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [FK_issuer_product_pin_calc_methods]
GO
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [FK_issuer_product_product_batch_type] FOREIGN KEY([product_load_type_id])
REFERENCES [dbo].[product_load_type] ([product_load_type_id])
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [FK_issuer_product_product_batch_type]
GO
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [FK_issuer_product_product_service_request_1] FOREIGN KEY([src1_id])
REFERENCES [dbo].[product_service_requet_code1] ([src1_id])
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [FK_issuer_product_product_service_request_1]
GO
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [FK_issuer_product_product_service_request_2] FOREIGN KEY([src2_id])
REFERENCES [dbo].[product_service_requet_code2] ([src2_id])
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [FK_issuer_product_product_service_request_2]
GO
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [FK_issuer_product_product_service_request_3] FOREIGN KEY([src3_id])
REFERENCES [dbo].[product_service_requet_code3] ([src3_id])
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [FK_issuer_product_product_service_request_3]
GO
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [FK_product_fee_scheme] FOREIGN KEY([fee_scheme_id])
REFERENCES [dbo].[product_fee_scheme] ([fee_scheme_id])
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [FK_product_fee_scheme]
GO
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [FK_product_issue_method] FOREIGN KEY([card_issue_method_id])
REFERENCES [dbo].[card_issue_method] ([card_issue_method_id])
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [FK_product_issue_method]
GO
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [FK_production_dist_batch_status_flow_issuer_product] FOREIGN KEY([distribution_dist_batch_status_flow])
REFERENCES [dbo].[dist_batch_status_flow] ([dist_batch_status_flow_id])
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [FK_production_dist_batch_status_flow_issuer_product]
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
ALTER TABLE [dbo].[load_batch]  WITH CHECK ADD  CONSTRAINT [FK_load_batch_type_id] FOREIGN KEY([load_batch_type_id])
REFERENCES [dbo].[load_batch_types] ([load_batch_type_id])
GO
ALTER TABLE [dbo].[load_batch] CHECK CONSTRAINT [FK_load_batch_type_id]
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
ALTER TABLE [dbo].[load_batch_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_load_batch_statuses_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[load_batch_statuses_language] CHECK CONSTRAINT [FK_load_batch_statuses_language_id]
GO

ALTER TABLE [dbo].[load_card_failed]  WITH CHECK ADD  CONSTRAINT [FK_load_card_failed_load_batch] FOREIGN KEY([load_batch_id])
REFERENCES [dbo].[load_batch] ([load_batch_id])
GO
ALTER TABLE [dbo].[load_card_failed] CHECK CONSTRAINT [FK_load_card_failed_load_batch]
GO
ALTER TABLE [dbo].[masterkeys]  WITH CHECK ADD  CONSTRAINT [FK_masterkeys_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[masterkeys] CHECK CONSTRAINT [FK_masterkeys_issuer]
GO
ALTER TABLE [dbo].[notification_batch_messages]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_statuses] FOREIGN KEY([dist_batch_statuses_id])
REFERENCES [dbo].[dist_batch_statuses] ([dist_batch_statuses_id])
GO
ALTER TABLE [dbo].[notification_batch_messages] CHECK CONSTRAINT [FK_dist_batch_statuses]
GO
ALTER TABLE [dbo].[notification_batch_messages]  WITH CHECK ADD  CONSTRAINT [FK_issuer_id] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[notification_batch_messages] CHECK CONSTRAINT [FK_issuer_id]
GO
ALTER TABLE [dbo].[notification_batch_messages]  WITH CHECK ADD  CONSTRAINT [FK_languages] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[notification_batch_messages] CHECK CONSTRAINT [FK_languages]
GO
ALTER TABLE [dbo].[notification_batch_messages]  WITH CHECK ADD  CONSTRAINT [FK_notification_batch_messages_dist_batch_type_id] FOREIGN KEY([dist_batch_type_id])
REFERENCES [dbo].[dist_batch_type] ([dist_batch_type_id])
GO
ALTER TABLE [dbo].[notification_batch_messages] CHECK CONSTRAINT [FK_notification_batch_messages_dist_batch_type_id]
GO
ALTER TABLE [dbo].[notification_branch_messages]  WITH CHECK ADD  CONSTRAINT [FK_notification_branch_messages_branch_card_statuses] FOREIGN KEY([branch_card_statuses_id])
REFERENCES [dbo].[branch_card_statuses] ([branch_card_statuses_id])
GO
ALTER TABLE [dbo].[notification_branch_messages] CHECK CONSTRAINT [FK_notification_branch_messages_branch_card_statuses]
GO
ALTER TABLE [dbo].[notification_branch_messages]  WITH CHECK ADD  CONSTRAINT [FK_notification_branch_messages_card_issue_method] FOREIGN KEY([card_issue_method_id])
REFERENCES [dbo].[card_issue_method] ([card_issue_method_id])
GO
ALTER TABLE [dbo].[notification_branch_messages] CHECK CONSTRAINT [FK_notification_branch_messages_card_issue_method]
GO
ALTER TABLE [dbo].[notification_branch_messages]  WITH CHECK ADD  CONSTRAINT [FK_notification_branch_messages_issuer_id] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[notification_branch_messages] CHECK CONSTRAINT [FK_notification_branch_messages_issuer_id]
GO
ALTER TABLE [dbo].[notification_branch_messages]  WITH CHECK ADD  CONSTRAINT [FK_notification_branch_messages_languages] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[notification_branch_messages] CHECK CONSTRAINT [FK_notification_branch_messages_languages]
GO
ALTER TABLE [dbo].[pin_batch]  WITH CHECK ADD  CONSTRAINT [FK_pin_batch_branch] FOREIGN KEY([branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO
ALTER TABLE [dbo].[pin_batch] CHECK CONSTRAINT [FK_pin_batch_branch]
GO
ALTER TABLE [dbo].[pin_batch]  WITH CHECK ADD  CONSTRAINT [FK_pin_batch_card_issue_method] FOREIGN KEY([card_issue_method_id])
REFERENCES [dbo].[card_issue_method] ([card_issue_method_id])
GO
ALTER TABLE [dbo].[pin_batch] CHECK CONSTRAINT [FK_pin_batch_card_issue_method]
GO
ALTER TABLE [dbo].[pin_batch]  WITH CHECK ADD  CONSTRAINT [FK_pin_batch_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[pin_batch] CHECK CONSTRAINT [FK_pin_batch_issuer]
GO
ALTER TABLE [dbo].[pin_batch]  WITH CHECK ADD  CONSTRAINT [FK_pin_batch_pin_batch_type] FOREIGN KEY([pin_batch_type_id])
REFERENCES [dbo].[pin_batch_type] ([pin_batch_type_id])
GO
ALTER TABLE [dbo].[pin_batch] CHECK CONSTRAINT [FK_pin_batch_pin_batch_type]
GO
ALTER TABLE [dbo].[pin_batch_cards]  WITH CHECK ADD  CONSTRAINT [FK_pin_batch_cards_cards] FOREIGN KEY([card_id])
REFERENCES [dbo].[cards] ([card_id])
GO
ALTER TABLE [dbo].[pin_batch_cards] CHECK CONSTRAINT [FK_pin_batch_cards_cards]
GO
ALTER TABLE [dbo].[pin_batch_cards]  WITH CHECK ADD  CONSTRAINT [FK_pin_batch_cards_pin_batch] FOREIGN KEY([pin_batch_id])
REFERENCES [dbo].[pin_batch] ([pin_batch_id])
GO
ALTER TABLE [dbo].[pin_batch_cards] CHECK CONSTRAINT [FK_pin_batch_cards_pin_batch]
GO
ALTER TABLE [dbo].[pin_batch_cards]  WITH CHECK ADD  CONSTRAINT [FK_pin_batch_cards_pin_batch_card_statuses] FOREIGN KEY([pin_batch_cards_statuses_id])
REFERENCES [dbo].[pin_batch_card_statuses] ([pin_batch_card_statuses_id])
GO
ALTER TABLE [dbo].[pin_batch_cards] CHECK CONSTRAINT [FK_pin_batch_cards_pin_batch_card_statuses]
GO
ALTER TABLE [dbo].[pin_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_pin_batch_status_pin_batch_status] FOREIGN KEY([pin_batch_id])
REFERENCES [dbo].[pin_batch] ([pin_batch_id])
GO
ALTER TABLE [dbo].[pin_batch_status] CHECK CONSTRAINT [FK_pin_batch_status_pin_batch_status]
GO
ALTER TABLE [dbo].[pin_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_pin_batch_status_pin_batch_statuses] FOREIGN KEY([pin_batch_statuses_id])
REFERENCES [dbo].[pin_batch_statuses] ([pin_batch_statuses_id])
GO
ALTER TABLE [dbo].[pin_batch_status] CHECK CONSTRAINT [FK_pin_batch_status_pin_batch_statuses]
GO
ALTER TABLE [dbo].[pin_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_pin_batch_status_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[pin_batch_status] CHECK CONSTRAINT [FK_pin_batch_status_user]
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
ALTER TABLE [dbo].[pin_reissue]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_branch] FOREIGN KEY([branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO
ALTER TABLE [dbo].[pin_reissue] CHECK CONSTRAINT [FK_pin_reissue_branch]
GO
ALTER TABLE [dbo].[pin_reissue]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[pin_reissue] CHECK CONSTRAINT [FK_pin_reissue_issuer]
GO
ALTER TABLE [dbo].[pin_reissue]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_issuer_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO
ALTER TABLE [dbo].[pin_reissue] CHECK CONSTRAINT [FK_pin_reissue_issuer_product]
GO
ALTER TABLE [dbo].[pin_reissue]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_user] FOREIGN KEY([operator_user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[pin_reissue] CHECK CONSTRAINT [FK_pin_reissue_user]
GO
ALTER TABLE [dbo].[pin_reissue]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_user1] FOREIGN KEY([authorise_user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[pin_reissue] CHECK CONSTRAINT [FK_pin_reissue_user1]
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
ALTER TABLE [dbo].[print_batch]  WITH CHECK ADD  CONSTRAINT [FK_print_batch_requests_branch] FOREIGN KEY([branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO
ALTER TABLE [dbo].[print_batch] CHECK CONSTRAINT [FK_print_batch_requests_branch]
GO
ALTER TABLE [dbo].[print_batch]  WITH CHECK ADD  CONSTRAINT [FK_print_batch_requests_card_issue_method] FOREIGN KEY([card_issue_method_id])
REFERENCES [dbo].[card_issue_method] ([card_issue_method_id])
GO
ALTER TABLE [dbo].[print_batch] CHECK CONSTRAINT [FK_print_batch_requests_card_issue_method]
GO
ALTER TABLE [dbo].[print_batch]  WITH CHECK ADD  CONSTRAINT [FK_print_batch_requests_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[print_batch] CHECK CONSTRAINT [FK_print_batch_requests_issuer]
GO
ALTER TABLE [dbo].[print_batch]  WITH CHECK ADD  CONSTRAINT [FK_print_batch_requests_origin_branch] FOREIGN KEY([origin_branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO
ALTER TABLE [dbo].[print_batch] CHECK CONSTRAINT [FK_print_batch_requests_origin_branch]
GO
ALTER TABLE [dbo].[print_batch_request_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_print_batch_request_statuses_language_languages] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[print_batch_request_statuses_language] CHECK CONSTRAINT [FK_print_batch_request_statuses_language_languages]
GO
ALTER TABLE [dbo].[print_batch_request_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_print_batch_request_statuses_language_print_batch] FOREIGN KEY([print_batch_request_status_id])
REFERENCES [dbo].[print_batch_request_statuses] ([print_batch_request_status_id])
GO
ALTER TABLE [dbo].[print_batch_request_statuses_language] CHECK CONSTRAINT [FK_print_batch_request_statuses_language_print_batch]
GO
ALTER TABLE [dbo].[print_batch_requests]  WITH CHECK ADD  CONSTRAINT [FK_print_batch_requests_hybrid_requests] FOREIGN KEY([request_id])
REFERENCES [dbo].[hybrid_requests] ([request_id])
GO
ALTER TABLE [dbo].[print_batch_requests] CHECK CONSTRAINT [FK_print_batch_requests_hybrid_requests]
GO
ALTER TABLE [dbo].[print_batch_requests]  WITH CHECK ADD  CONSTRAINT [FK_print_batch_requests_print_batch] FOREIGN KEY([print_batch_id])
REFERENCES [dbo].[print_batch] ([print_batch_id])
GO
ALTER TABLE [dbo].[print_batch_requests] CHECK CONSTRAINT [FK_print_batch_requests_print_batch]
GO
ALTER TABLE [dbo].[print_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_print_batch_status_print_batch] FOREIGN KEY([print_batch_id])
REFERENCES [dbo].[print_batch] ([print_batch_id])
GO
ALTER TABLE [dbo].[print_batch_status] CHECK CONSTRAINT [FK_print_batch_status_print_batch]
GO
ALTER TABLE [dbo].[print_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_print_batch_status_print_batch_statuses] FOREIGN KEY([print_batch_statuses_id])
REFERENCES [dbo].[print_batch_statuses] ([print_batch_statuses_id])
GO
ALTER TABLE [dbo].[print_batch_status] CHECK CONSTRAINT [FK_print_batch_status_print_batch_statuses]
GO
ALTER TABLE [dbo].[print_batch_status]  WITH CHECK ADD  CONSTRAINT [FK_print_batch_status_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[print_batch_status] CHECK CONSTRAINT [FK_print_batch_status_user]
GO
ALTER TABLE [dbo].[print_batch_statuses_language]  WITH CHECK ADD  CONSTRAINT [[FK_print_batch_statuses_language_language]]] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[print_batch_statuses_language] CHECK CONSTRAINT [[FK_print_batch_statuses_language_language]]]
GO
ALTER TABLE [dbo].[print_batch_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_print_batch_statuses_language_print_batch] FOREIGN KEY([print_batch_statuses_id])
REFERENCES [dbo].[print_batch_statuses] ([print_batch_statuses_id])
GO
ALTER TABLE [dbo].[print_batch_statuses_language] CHECK CONSTRAINT [FK_print_batch_statuses_language_print_batch]
GO
ALTER TABLE [dbo].[print_field_types_language]  WITH CHECK ADD  CONSTRAINT [FK_print_field_types_language_print_field_types] FOREIGN KEY([print_field_type_id])
REFERENCES [dbo].[print_field_types] ([print_field_type_id])
GO
ALTER TABLE [dbo].[print_field_types_language] CHECK CONSTRAINT [FK_print_field_types_language_print_field_types]
GO
ALTER TABLE [dbo].[product_currency]  WITH NOCHECK ADD  CONSTRAINT [FK__product_c__curre__08162EEB] FOREIGN KEY([currency_id])
REFERENCES [dbo].[currency] ([currency_id])
GO
ALTER TABLE [dbo].[product_currency] NOCHECK CONSTRAINT [FK__product_c__curre__08162EEB]
GO
ALTER TABLE [dbo].[product_currency]  WITH NOCHECK ADD  CONSTRAINT [FK__product_c__produ__090A5324] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO
ALTER TABLE [dbo].[product_currency] NOCHECK CONSTRAINT [FK__product_c__produ__090A5324]
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
ALTER TABLE [dbo].[product_fee_accounting]  WITH CHECK ADD  CONSTRAINT [FK_product_fee_accounting_customer_account_type] FOREIGN KEY([fee_revenue_account_type_id])
REFERENCES [dbo].[customer_account_type] ([account_type_id])
GO
ALTER TABLE [dbo].[product_fee_accounting] CHECK CONSTRAINT [FK_product_fee_accounting_customer_account_type]
GO
ALTER TABLE [dbo].[product_fee_accounting]  WITH CHECK ADD  CONSTRAINT [FK_product_fee_accounting_customer_account_type1] FOREIGN KEY([vat_account_type_id])
REFERENCES [dbo].[customer_account_type] ([account_type_id])
GO
ALTER TABLE [dbo].[product_fee_accounting] CHECK CONSTRAINT [FK_product_fee_accounting_customer_account_type1]
GO
ALTER TABLE [dbo].[product_fee_accounting]  WITH CHECK ADD  CONSTRAINT [FK_product_fee_accounting_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[product_fee_accounting] CHECK CONSTRAINT [FK_product_fee_accounting_issuer]
GO
ALTER TABLE [dbo].[product_fee_charge]  WITH NOCHECK ADD  CONSTRAINT [FK_product_fee_charge_currency] FOREIGN KEY([currency_id])
REFERENCES [dbo].[currency] ([currency_id])
GO
ALTER TABLE [dbo].[product_fee_charge] NOCHECK CONSTRAINT [FK_product_fee_charge_currency]
GO
ALTER TABLE [dbo].[product_fee_charge]  WITH NOCHECK ADD  CONSTRAINT [FK_product_fee_charge_product_fee_charge] FOREIGN KEY([fee_detail_id])
REFERENCES [dbo].[product_fee_detail] ([fee_detail_id])
GO
ALTER TABLE [dbo].[product_fee_charge] NOCHECK CONSTRAINT [FK_product_fee_charge_product_fee_charge]
GO
ALTER TABLE [dbo].[product_fee_detail]  WITH CHECK ADD  CONSTRAINT [FK_fee_detail_fee_detail] FOREIGN KEY([fee_scheme_id])
REFERENCES [dbo].[product_fee_scheme] ([fee_scheme_id])
GO
ALTER TABLE [dbo].[product_fee_detail] CHECK CONSTRAINT [FK_fee_detail_fee_detail]
GO
ALTER TABLE [dbo].[product_fee_scheme]  WITH CHECK ADD  CONSTRAINT [FK_product_fee_scheme_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO
ALTER TABLE [dbo].[product_fee_scheme] CHECK CONSTRAINT [FK_product_fee_scheme_issuer]
GO
ALTER TABLE [dbo].[product_fee_scheme]  WITH CHECK ADD  CONSTRAINT [FK_product_fee_scheme_product_fee_accounting] FOREIGN KEY([fee_accounting_id])
REFERENCES [dbo].[product_fee_accounting] ([fee_accounting_id])
GO
ALTER TABLE [dbo].[product_fee_scheme] CHECK CONSTRAINT [FK_product_fee_scheme_product_fee_accounting]
GO
ALTER TABLE [dbo].[product_fields]  WITH CHECK ADD  CONSTRAINT [FK_product_fields_issuer_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO
ALTER TABLE [dbo].[product_fields] CHECK CONSTRAINT [FK_product_fields_issuer_product]
GO
ALTER TABLE [dbo].[product_fields]  WITH CHECK ADD  CONSTRAINT [FK_product_fields_print_field_types] FOREIGN KEY([print_field_type_id])
REFERENCES [dbo].[print_field_types] ([print_field_type_id])
GO
ALTER TABLE [dbo].[product_fields] CHECK CONSTRAINT [FK_product_fields_print_field_types]
GO
ALTER TABLE [dbo].[product_interface]  WITH CHECK ADD  CONSTRAINT [FK_product_interface_connection_parameters] FOREIGN KEY([connection_parameter_id])
REFERENCES [dbo].[connection_parameters] ([connection_parameter_id])
GO
ALTER TABLE [dbo].[product_interface] CHECK CONSTRAINT [FK_product_interface_connection_parameters]
GO
ALTER TABLE [dbo].[product_interface]  WITH CHECK ADD  CONSTRAINT [FK_product_interface_interface_type] FOREIGN KEY([interface_type_id])
REFERENCES [dbo].[interface_type] ([interface_type_id])
GO
ALTER TABLE [dbo].[product_interface] CHECK CONSTRAINT [FK_product_interface_interface_type]
GO
ALTER TABLE [dbo].[product_interface]  WITH CHECK ADD  CONSTRAINT [FK_product_interface_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO
ALTER TABLE [dbo].[product_interface] CHECK CONSTRAINT [FK_product_interface_product]
GO
ALTER TABLE [dbo].[product_issue_reason]  WITH CHECK ADD  CONSTRAINT [FK_product_issue_reason_card_issue_reason] FOREIGN KEY([card_issue_reason_id])
REFERENCES [dbo].[card_issue_reason] ([card_issue_reason_id])
GO
ALTER TABLE [dbo].[product_issue_reason] CHECK CONSTRAINT [FK_product_issue_reason_card_issue_reason]
GO
ALTER TABLE [dbo].[product_issue_reason]  WITH CHECK ADD  CONSTRAINT [FK_product_issue_reason_issuer_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO
ALTER TABLE [dbo].[product_issue_reason] CHECK CONSTRAINT [FK_product_issue_reason_issuer_product]
GO
ALTER TABLE [dbo].[products_account_types]  WITH CHECK ADD  CONSTRAINT [FK_products_account_types_customer_account_type] FOREIGN KEY([account_type_id])
REFERENCES [dbo].[customer_account_type] ([account_type_id])
GO
ALTER TABLE [dbo].[products_account_types] CHECK CONSTRAINT [FK_products_account_types_customer_account_type]
GO
ALTER TABLE [dbo].[products_account_types]  WITH CHECK ADD  CONSTRAINT [FK_products_account_types_issuer_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO
ALTER TABLE [dbo].[products_account_types] CHECK CONSTRAINT [FK_products_account_types_issuer_product]
GO
ALTER TABLE [dbo].[remote_update_status]  WITH CHECK ADD  CONSTRAINT [FK_remote_update_status_cards] FOREIGN KEY([card_id])
REFERENCES [dbo].[cards] ([card_id])
GO
ALTER TABLE [dbo].[remote_update_status] CHECK CONSTRAINT [FK_remote_update_status_cards]
GO
ALTER TABLE [dbo].[remote_update_status]  WITH CHECK ADD  CONSTRAINT [FK_remote_update_status_remote_component_statuses] FOREIGN KEY([remote_update_statuses_id])
REFERENCES [dbo].[remote_update_statuses] ([remote_update_statuses_id])
GO
ALTER TABLE [dbo].[remote_update_status] CHECK CONSTRAINT [FK_remote_update_status_remote_component_statuses]
GO
ALTER TABLE [dbo].[remote_update_status]  WITH CHECK ADD  CONSTRAINT [FK_remote_update_status_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([user_id])
GO
ALTER TABLE [dbo].[remote_update_status] CHECK CONSTRAINT [FK_remote_update_status_user]
GO
ALTER TABLE [dbo].[remote_update_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_remote_update_statuses_language_languages] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[remote_update_statuses_language] CHECK CONSTRAINT [FK_remote_update_statuses_language_languages]
GO
ALTER TABLE [dbo].[remote_update_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_remote_update_statuses_language_remote_update_statuses] FOREIGN KEY([remote_update_statuses_id])
REFERENCES [dbo].[remote_update_statuses] ([remote_update_statuses_id])
GO
ALTER TABLE [dbo].[remote_update_statuses_language] CHECK CONSTRAINT [FK_remote_update_statuses_language_remote_update_statuses]
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
ALTER TABLE [dbo].[user_pending]  WITH CHECK ADD  CONSTRAINT [FK_application_user_pending_user_status] FOREIGN KEY([user_status_id])
REFERENCES [dbo].[user_status] ([user_status_id])
GO
ALTER TABLE [dbo].[user_pending] CHECK CONSTRAINT [FK_application_user_pending_user_status]
GO
ALTER TABLE [dbo].[user_pending]  WITH CHECK ADD  CONSTRAINT [FK_user_pending_has_languages] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO
ALTER TABLE [dbo].[user_pending] CHECK CONSTRAINT [FK_user_pending_has_languages]
GO
ALTER TABLE [dbo].[user_pending]  WITH CHECK ADD  CONSTRAINT [FK_user_pending_user_gender] FOREIGN KEY([user_gender_id])
REFERENCES [dbo].[user_gender] ([user_gender_id])
GO
ALTER TABLE [dbo].[user_pending] CHECK CONSTRAINT [FK_user_pending_user_gender]
GO
ALTER TABLE [dbo].[user_to_user_group_pending]  WITH CHECK ADD  CONSTRAINT [FK_users_to_users_groups_user_group_pending] FOREIGN KEY([user_group_id])
REFERENCES [dbo].[user_group] ([user_group_id])
GO
ALTER TABLE [dbo].[user_to_user_group_pending] CHECK CONSTRAINT [FK_users_to_users_groups_user_group_pending]
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
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [chkPinMinLength] CHECK  (([max_pin_length]>=[min_pin_length]))
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [chkPinMinLength]
GO
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [chkPinMinMaxLength] CHECK  (([min_pin_length]>=(4)))
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [chkPinMinMaxLength]
GO
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [CK_BIN_LENGTH] CHECK  (([product_bin_code] IS NOT NULL AND len([product_bin_code])=(6)))

ALTER TABLE [dbo].[user_admin]  WITH CHECK ADD  CONSTRAINT [CK_user_admin] CHECK  (([PasswordMaxLength]>[PasswordMinLength]))
GO
ALTER TABLE [dbo].[user_admin] CHECK CONSTRAINT [CK_user_admin]
GO

ALTER TABLE [dbo].[branch_card_codes]  WITH CHECK ADD  CONSTRAINT [FK_branch_card_codes_branch_card_code_type] FOREIGN KEY([branch_card_code_type_id])
REFERENCES [dbo].[branch_card_code_type] ([branch_card_code_type_id])
GO

ALTER TABLE [dbo].[branch_card_codes] CHECK CONSTRAINT [FK_branch_card_codes_branch_card_code_type]
GO
GO

CREATE NONCLUSTERED INDEX [INDEX_CARD_OPERATOR]
    ON [dbo].[branch_card_status]([branch_card_statuses_id] ASC, [status_date] ASC)
    INCLUDE([card_id], [operator_user_id]);
GO

CREATE NONCLUSTERED INDEX [INDEX_CARD_STATUS_DATE]
    ON [dbo].[branch_card_status]([card_id] ASC)
    INCLUDE([status_date]);
GO

ALTER TABLE [dbo].[audit_action_language]  WITH CHECK ADD  CONSTRAINT [FK_audit_action_language_audit_action] FOREIGN KEY([audit_action_id])
REFERENCES [dbo].[audit_action] ([audit_action_id])
GO

ALTER TABLE [dbo].[audit_action_language] CHECK CONSTRAINT [FK_audit_action_language_audit_action]
GO

ALTER TABLE [dbo].[audit_action_language]  WITH CHECK ADD  CONSTRAINT [FK_audit_action_language_user] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[audit_action_language] CHECK CONSTRAINT [FK_audit_action_language_user]
GO

ALTER TABLE [dbo].[branch_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_branch_statuses_language_branch_status] FOREIGN KEY([branch_status_id])
REFERENCES [dbo].[branch_statuses] ([branch_status_id])
GO

ALTER TABLE [dbo].[branch_statuses_language] CHECK CONSTRAINT [FK_branch_statuses_language_branch_status]
GO

ALTER TABLE [dbo].[branch_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_branch_statuses_language_language] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[branch_statuses_language] CHECK CONSTRAINT [FK_branch_statuses_language_language]
GO
ALTER TABLE [dbo].[branch_type_language]  WITH CHECK ADD  CONSTRAINT [FK_branch_type_language_branch_type] FOREIGN KEY([branch_type_id])
REFERENCES [dbo].[branch_type] ([branch_type_id])
GO

ALTER TABLE [dbo].[branch_type_language] CHECK CONSTRAINT [FK_branch_type_language_branch_type]
GO
ALTER TABLE [dbo].[customer_account_cards]  WITH CHECK ADD  CONSTRAINT [FK_customer_account_cards_customer_account] FOREIGN KEY([customer_account_id])
REFERENCES [dbo].[customer_account] ([customer_account_id])
GO

ALTER TABLE [dbo].[customer_account_cards] CHECK CONSTRAINT [FK_customer_account_cards_customer_account]
GO


ALTER TABLE [dbo].[card_issue_method_language]  WITH CHECK ADD  CONSTRAINT [FK_card_issue_method_language_card_issue_method_id] FOREIGN KEY([card_issue_method_id])
REFERENCES [dbo].[card_issue_method] ([card_issue_method_id])
GO

ALTER TABLE [dbo].[card_issue_method_language] CHECK CONSTRAINT [FK_card_issue_method_language_card_issue_method_id]
GO

ALTER TABLE [dbo].[card_issue_method_language]  WITH CHECK ADD  CONSTRAINT [FK_card_issue_method_language_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[card_issue_method_language] CHECK CONSTRAINT [FK_card_issue_method_language_language_id]
GO
ALTER TABLE [dbo].[card_issue_reason_language]  WITH CHECK ADD  CONSTRAINT [FK_card_issue_reason_language_card_issue_reason_id] FOREIGN KEY([card_issue_reason_id])
REFERENCES [dbo].[card_issue_reason] ([card_issue_reason_id])
GO

ALTER TABLE [dbo].[card_issue_reason_language] CHECK CONSTRAINT [FK_card_issue_reason_language_card_issue_reason_id]
GO

ALTER TABLE [dbo].[card_issue_reason_language]  WITH CHECK ADD  CONSTRAINT [FK_card_issue_reason_language_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[card_issue_reason_language] CHECK CONSTRAINT [FK_card_issue_reason_language_language_id]
GO


ALTER TABLE [dbo].[connection_parameter_type_language]  WITH CHECK ADD  CONSTRAINT [FK_connection_parameter_type_connection_parameter_type_id] FOREIGN KEY([connection_parameter_type_id])
REFERENCES [dbo].[connection_parameter_type] ([connection_parameter_type_id])
GO

ALTER TABLE [dbo].[connection_parameter_type_language] CHECK CONSTRAINT [FK_connection_parameter_type_connection_parameter_type_id]
GO

ALTER TABLE [dbo].[connection_parameter_type_language]  WITH CHECK ADD  CONSTRAINT [FK_connection_parameter_type_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[connection_parameter_type_language] CHECK CONSTRAINT [FK_connection_parameter_type_language_id]
GO
ALTER TABLE [dbo].[customer_account_type_language]  WITH CHECK ADD  CONSTRAINT [FK_customer_account_type_language_account_type_id] FOREIGN KEY([account_type_id])
REFERENCES [dbo].[customer_account_type] ([account_type_id])
GO

ALTER TABLE [dbo].[customer_account_type_language] CHECK CONSTRAINT [FK_customer_account_type_language_account_type_id]
GO

ALTER TABLE [dbo].[customer_account_type_language]  WITH CHECK ADD  CONSTRAINT [FK_customer_account_type_language_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[customer_account_type_language] CHECK CONSTRAINT [FK_customer_account_type_language_language_id]
GO

ALTER TABLE [dbo].[customer_residency_language]  WITH CHECK ADD  CONSTRAINT [FK_customer_residency_language_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[customer_residency_language] CHECK CONSTRAINT [FK_customer_residency_language_language_id]
GO

ALTER TABLE [dbo].[customer_residency_language]  WITH CHECK ADD  CONSTRAINT [FK_customer_residency_language_resident_id] FOREIGN KEY([resident_id])
REFERENCES [dbo].[customer_residency] ([resident_id])
GO

ALTER TABLE [dbo].[customer_residency_language] CHECK CONSTRAINT [FK_customer_residency_language_resident_id]
GO
ALTER TABLE [dbo].[customer_title_language]  WITH CHECK ADD  CONSTRAINT [FK_customer_title_language_customer_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[customer_title_language] CHECK CONSTRAINT [FK_customer_title_language_customer_language_id]
GO

ALTER TABLE [dbo].[customer_title_language]  WITH CHECK ADD  CONSTRAINT [FK_customer_title_language_customer_title_id] FOREIGN KEY([customer_title_id])
REFERENCES [dbo].[customer_title] ([customer_title_id])
GO

ALTER TABLE [dbo].[customer_title_language] CHECK CONSTRAINT [FK_customer_title_language_customer_title_id]
GO
ALTER TABLE [dbo].[customer_type_language]  WITH CHECK ADD  CONSTRAINT [FK_customer_type_language_customer_type_id] FOREIGN KEY([customer_type_id])
REFERENCES [dbo].[customer_type] ([customer_type_id])
GO

ALTER TABLE [dbo].[customer_type_language] CHECK CONSTRAINT [FK_customer_type_language_customer_type_id]
GO

ALTER TABLE [dbo].[customer_type_language]  WITH CHECK ADD  CONSTRAINT [FK_customer_type_language_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[customer_type_language] CHECK CONSTRAINT [FK_customer_type_language_language_id]
GO
GO

CREATE STATISTICS [_dta_stat_430624577_3_1_2]
    ON [dbo].[dist_batch_cards]([dist_card_status_id], [dist_batch_id], [card_id]);
GO

CREATE STATISTICS [_dta_stat_430624577_3_2]
    ON [dbo].[dist_batch_cards]([dist_card_status_id], [card_id]);
GO
GO

ALTER TABLE [dbo].[dist_batch_status_flow]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_status_flow_card_issue_method] FOREIGN KEY([card_issue_method_id])
REFERENCES [dbo].[card_issue_method] ([card_issue_method_id])
GO

ALTER TABLE [dbo].[dist_batch_status_flow] CHECK CONSTRAINT [FK_dist_batch_status_flow_card_issue_method]
GO

ALTER TABLE [dbo].[dist_batch_status_flow]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_status_flow_dist_batcht_ype] FOREIGN KEY([dist_batch_type_id])
REFERENCES [dbo].[dist_batch_type] ([dist_batch_type_id])
GO

ALTER TABLE [dbo].[dist_batch_status_flow] CHECK CONSTRAINT [FK_dist_batch_status_flow_dist_batcht_ype]
GO

ALTER TABLE [dbo].[dist_batch_statuses_flow]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_status_flow_dist_batch_statuses_flow] FOREIGN KEY([dist_batch_status_flow_id])
REFERENCES [dbo].[dist_batch_status_flow] ([dist_batch_status_flow_id])
GO

ALTER TABLE [dbo].[dist_batch_statuses_flow] CHECK CONSTRAINT [FK_dist_batch_status_flow_dist_batch_statuses_flow]
GO
ALTER TABLE [dbo].[dist_batch_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_statuses_language_dist_batch_statuses_id] FOREIGN KEY([dist_batch_statuses_id])
REFERENCES [dbo].[dist_batch_statuses] ([dist_batch_statuses_id])
GO

ALTER TABLE [dbo].[dist_batch_statuses_language] CHECK CONSTRAINT [FK_dist_batch_statuses_language_dist_batch_statuses_id]
GO

ALTER TABLE [dbo].[dist_batch_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_dist_batch_statuses_language_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[dist_batch_statuses_language] CHECK CONSTRAINT [FK_dist_batch_statuses_language_language_id]
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
ALTER TABLE [dbo].[file_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_file_statuses_language_file_status_id] FOREIGN KEY([file_status_id])
REFERENCES [dbo].[file_statuses] ([file_status_id])
GO

ALTER TABLE [dbo].[file_statuses_language] CHECK CONSTRAINT [FK_file_statuses_language_file_status_id]
GO

ALTER TABLE [dbo].[file_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_file_statuses_language_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[file_statuses_language] CHECK CONSTRAINT [FK_file_statuses_language_language_id]
GO
GO

ALTER TABLE [dbo].[interface_type_language]  WITH CHECK ADD  CONSTRAINT [FK_interface_type_language_interface_type_id] FOREIGN KEY([interface_type_id])
REFERENCES [dbo].[interface_type] ([interface_type_id])
GO

ALTER TABLE [dbo].[interface_type_language] CHECK CONSTRAINT [FK_interface_type_language_interface_type_id]
GO

ALTER TABLE [dbo].[interface_type_language]  WITH CHECK ADD  CONSTRAINT [FK_interface_type_language_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[interface_type_language] CHECK CONSTRAINT [FK_interface_type_language_language_id]
GO

ALTER TABLE [dbo].[Issuer_product_font]  WITH CHECK ADD  CONSTRAINT [FK_Issuer_product_font_Issuer_product_font] FOREIGN KEY([font_id])
REFERENCES [dbo].[Issuer_product_font] ([font_id])
GO

ALTER TABLE [dbo].[Issuer_product_font] CHECK CONSTRAINT [FK_Issuer_product_font_Issuer_product_font]
GO
GO

ALTER TABLE [dbo].[issuer_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_issuer_statuses_issuer_status_id] FOREIGN KEY([issuer_status_id])
REFERENCES [dbo].[issuer_statuses] ([issuer_status_id])
GO

ALTER TABLE [dbo].[issuer_statuses_language] CHECK CONSTRAINT [FK_issuer_statuses_issuer_status_id]
GO

ALTER TABLE [dbo].[issuer_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_issuer_statuses_language_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[issuer_statuses_language] CHECK CONSTRAINT [FK_issuer_statuses_language_language_id]
GO


ALTER TABLE [dbo].[load_batch_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_load_batch_statuses_load_batch_statuses_id] FOREIGN KEY([load_batch_statuses_id])
REFERENCES [dbo].[load_batch_statuses] ([load_batch_statuses_id])
GO

ALTER TABLE [dbo].[load_batch_statuses_language] CHECK CONSTRAINT [FK_load_batch_statuses_load_batch_statuses_id]
GO
ALTER TABLE [dbo].[pin_batch_statuses_flow]  WITH CHECK ADD  CONSTRAINT [FK_pin_batch_statuses_flow_card_issue_method] FOREIGN KEY([card_issue_method_id])
REFERENCES [dbo].[card_issue_method] ([card_issue_method_id])
GO

ALTER TABLE [dbo].[pin_batch_statuses_flow] CHECK CONSTRAINT [FK_pin_batch_statuses_flow_card_issue_method]
GO

ALTER TABLE [dbo].[pin_batch_statuses_flow]  WITH CHECK ADD  CONSTRAINT [FK_pin_batch_statuses_flow_pin_batch_type] FOREIGN KEY([pin_batch_type_id])
REFERENCES [dbo].[pin_batch_type] ([pin_batch_type_id])
GO

ALTER TABLE [dbo].[pin_batch_statuses_flow] CHECK CONSTRAINT [FK_pin_batch_statuses_flow_pin_batch_type]
GO
ALTER TABLE [dbo].[pin_batch_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_pin_batch_statuses_language_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[pin_batch_statuses_language] CHECK CONSTRAINT [FK_pin_batch_statuses_language_language_id]
GO

ALTER TABLE [dbo].[pin_batch_statuses_language]  WITH CHECK ADD  CONSTRAINT [FK_pin_batch_statuses_pin_batch_statuses_id] FOREIGN KEY([pin_batch_statuses_id])
REFERENCES [dbo].[pin_batch_statuses] ([pin_batch_statuses_id])
GO

ALTER TABLE [dbo].[pin_batch_statuses_language] CHECK CONSTRAINT [FK_pin_batch_statuses_pin_batch_statuses_id]
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
GO

CREATE NONCLUSTERED INDEX [NonClusteredIndex-20180820-114942]
    ON [dbo].[temp_load_cards_type]([product_id] ASC);
GO

CREATE NONCLUSTERED INDEX [NonClusteredIndex-20180820-125139]
    ON [dbo].[temp_load_cards_type]([card_number] ASC, [card_reference] ASC);
GO
ALTER TABLE [dbo].[user_roles_language]  WITH CHECK ADD  CONSTRAINT [FK_user_roles_language_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[user_roles_language] CHECK CONSTRAINT [FK_user_roles_language_language_id]
GO

ALTER TABLE [dbo].[user_roles_language]  WITH CHECK ADD  CONSTRAINT [FK_user_roles_language_user_role_id] FOREIGN KEY([user_role_id])
REFERENCES [dbo].[user_roles] ([user_role_id])
GO

ALTER TABLE [dbo].[user_roles_language] CHECK CONSTRAINT [FK_user_roles_language_user_role_id]
GO
ALTER TABLE [dbo].[user_status_language]  WITH CHECK ADD  CONSTRAINT [FK_user_status_language_language_id] FOREIGN KEY([language_id])
REFERENCES [dbo].[languages] ([id])
GO

ALTER TABLE [dbo].[user_status_language] CHECK CONSTRAINT [FK_user_status_language_language_id]
GO

ALTER TABLE [dbo].[user_status_language]  WITH CHECK ADD  CONSTRAINT [FK_user_status_language_user_status_id] FOREIGN KEY([user_status_id])
REFERENCES [dbo].[user_status] ([user_status_id])
GO

ALTER TABLE [dbo].[user_status_language] CHECK CONSTRAINT [FK_user_status_language_user_status_id]
GO
