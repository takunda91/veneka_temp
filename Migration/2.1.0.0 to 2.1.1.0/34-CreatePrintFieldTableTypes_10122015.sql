USE [indigo_database_main_dev]
GO

/****** Object:  UserDefinedTableType [dbo].[product_print_fields]    Script Date: 2015-12-10 04:12:30 PM ******/
CREATE TYPE [dbo].[product_print_fields] AS TABLE(
	[product_field_id] [int] NULL,
	[product_id] [int] NULL,
	[field_name] [varchar](100) NULL,
	[print_field_type_id] [int] NULL,
	[X] [decimal](18, 2) NULL,
	[Y] [decimal](18, 2) NULL,
	[width] [decimal](18, 2) NULL,
	[height] [decimal](18, 2) NULL,
	[font] [varchar](50) NULL,
	[font_size] [int] NULL,
	[mapped_name] [varchar](max) NULL,
	[editable] [bit] NULL,
	[deleted] [bit] NULL,
	[label] [varchar](100) NULL,
	[max_length] [int] NULL
)
GO


