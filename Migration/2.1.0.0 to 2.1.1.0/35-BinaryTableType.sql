USE [indigo_database_main_dev]
GO

/****** Object:  UserDefinedTableType [dbo].[key_value_array]    Script Date: 2015-11-23 02:17:58 PM ******/
CREATE TYPE [dbo].[key_binary_value_array] AS TABLE(
	[key] [bigint] NULL,
	[value] [image] NULL
)
GO


