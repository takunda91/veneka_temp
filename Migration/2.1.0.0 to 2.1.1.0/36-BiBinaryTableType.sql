USE [indigo_database_main_dev]
GO

/****** Object:  UserDefinedTableType [dbo].[key_binary_value_array]    Script Date: 2015-12-07 02:10:12 PM ******/
CREATE TYPE [dbo].[bi_key_binary_value_array] AS TABLE(
	[key1] [bigint] NULL,
	[key2] [bigint] NULL,
	[value] [image] NULL
)
GO


