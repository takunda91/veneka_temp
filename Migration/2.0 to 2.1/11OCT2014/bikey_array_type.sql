USE [indigo_database_main_dev]
GO

/****** Object:  UserDefinedTableType [dbo].[key_value_array]    Script Date: 2014/10/11 09:39:24 AM ******/
CREATE TYPE [dbo].[bikey_value_array] AS TABLE(
	[key1] [bigint] NULL,
	[key2] [bigint] NULL,
	[value] [varchar](max) NULL
)
GO


