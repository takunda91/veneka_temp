USE [indigo_database_main_dev]
GO

DROP PROCEDURE sp_create_load_batch
GO

/****** Object:  UserDefinedTableType [dbo].[load_cards_type]    Script Date: 2015-07-06 04:22:09 PM ******/
DROP TYPE [dbo].[load_cards_type]
GO

/****** Object:  UserDefinedTableType [dbo].[load_cards_type]    Script Date: 2015-07-06 04:22:09 PM ******/
CREATE TYPE [dbo].[load_cards_type] AS TABLE(
	[card_number] [varchar](20) NULL,
	[card_reference] [varchar](100) NULL,
	[branch_id] [int] NULL,
	[card_sequence] [int] NULL,
	[product_id] [int] NULL,
	[card_issue_method_id] [int] NULL,
	[sub_product_id] [int] NULL,
	[already_loaded] [bit] NULL
)
GO


