USE [indigo_database_main_dev]
GO

/****** Object:  UserDefinedTableType [dbo].[product_external_fields_array]    Script Date: 2016-04-05 12:43:59 PM ******/
CREATE TYPE [dbo].[product_external_fields_array] AS TABLE(
	[external_system_field_id] [int] NULL,
	[field_name] [varchar](250) NULL,
	[field_value] [varchar](250) NULL
)
GO


