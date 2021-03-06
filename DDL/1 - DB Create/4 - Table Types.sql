USE [indigo_database_group]
GO
/****** Object:  UserDefinedTableType [dbo].[branch_id_array]    Script Date: 2014/07/31 05:25:07 PM ******/
CREATE TYPE [dbo].[branch_id_array] AS TABLE(
	[branch_id] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[card_id_array]    Script Date: 2014/07/31 05:25:07 PM ******/
CREATE TYPE [dbo].[card_id_array] AS TABLE(
	[card_id] [bigint] NULL,
	[branch_card_statuses_id] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[currency_id_array]    Script Date: 2014/07/31 05:25:07 PM ******/
CREATE TYPE [dbo].[currency_id_array] AS TABLE(
	[Currency_id] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[DistBatchCards]    Script Date: 2014/07/31 05:25:07 PM ******/
CREATE TYPE [dbo].[DistBatchCards] AS TABLE(
	[card_number] [varchar](20) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[load_cards_type]    Script Date: 2014/07/31 05:25:07 PM ******/
CREATE TYPE [dbo].[load_cards_type] AS TABLE(
	[card_number] [varchar](20) NULL,
	[branch_id] [int] NULL,
	[card_sequence] [int] NULL,
	[product_id] [int] NULL,
	[already_loaded] [bit] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[parm_card_expected_status]    Script Date: 2014/07/31 05:25:07 PM ******/
CREATE TYPE [dbo].[parm_card_expected_status] AS TABLE(
	[card_status] [varchar](20) NOT NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[user_group_id_array]    Script Date: 2014/07/31 05:25:07 PM ******/
CREATE TYPE [dbo].[user_group_id_array] AS TABLE(
	[user_group_id] [int] NULL
)
GO
