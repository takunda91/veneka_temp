USE [indigo_database_main_dev]
GO

/****** Object:  StoredProcedure [dbo].[sp_create_load_batch]    Script Date: 2014/12/15 05:53:20 PM ******/
DROP PROCEDURE [dbo].[sp_create_load_batch]
GO

/****** Object:  UserDefinedTableType [dbo].[load_cards_type]    Script Date: 2014/12/15 05:58:55 PM ******/
DROP TYPE [dbo].[load_cards_type]
GO

/****** Object:  UserDefinedTableType [dbo].[load_cards_type]    Script Date: 2014/12/15 05:52:01 PM ******/
CREATE TYPE [dbo].[load_cards_type] AS TABLE(
	[card_number] [varchar](20) NULL,
	[branch_id] [int] NULL,
	[card_sequence] [int] NULL,
	[product_id] [int] NULL,
	[card_issue_method_id] [int] NUll,
	[sub_product_id] [int] NULL,
	[already_loaded] [bit] NULL
)
GO


