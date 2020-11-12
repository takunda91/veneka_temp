USE [indigo_database_main_dev]
GO

/****** Object:  UserDefinedTableType [dbo].[load_bulk_card_request]    Script Date: 2015-09-29 05:13:21 PM ******/
CREATE TYPE [dbo].[load_bulk_card_request] AS TABLE(
	[reference_number] [varchar](20) NULL,
	[branch_id] [int] NULL,
	[product_id] [int] NULL,
	[card_priority_id] [int] NULL,
	[customer_account_number] [varchar](27) NULL,
	[domicile_branch_id] [int] NULL,
	[account_type_id] [int] NULL,
	[card_issue_reason_id] [int] NULL,
	[customer_first_name] [varchar](50) NULL,
	[customer_middle_name] [varchar](50) NULL,
	[customer_last_name] [varchar](50) NULL,
	[name_on_card] [varchar](30) NULL,
	[customer_title_id] [int] NULL,
	[currency_id] [int] NULL,
	[resident_id] [int] NULL,
	[customer_type_id] [int] NULL,
	[cms_id] [varchar](50) NULL,
	[contract_number] [varchar](50) NULL,
	[idnumber] [varchar](50) NULL,
	[contact_number] [varchar](50) NULL,
	[customer_id] [varchar](50) NULL,
	[fee_waiver_YN] [bit] NULL,
	[fee_editable_YN] [bit] NULL,
	[fee_charged] [decimal](10, 4) NULL,
	[fee_overridden_YN] [bit] NULL,
	[audit_user_id] [bigint] NULL,
	[audit_workstation] [varchar](100) NULL,
	[already_loaded] [bit] NULL
)
GO


