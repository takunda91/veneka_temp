USE [indigo_database_main_dev]
GO

ALTER TABLE [product_currency]
ADD is_base bit null
GO

UPDATE [product_currency]
SET is_base = 0
GO

ALTER TABLE [product_currency]
ALTER COLUMN is_base BIT not null
GO

ALTER TABLE [product_currency]
ADD usr_field_name_1 varchar(250)
GO

ALTER TABLE [product_currency]
ADD usr_field_val_1 varchar(250)
GO

ALTER TABLE [product_currency]
ADD usr_field_name_2 varchar(250)
GO

ALTER TABLE [product_currency]
ADD usr_field_val_2 varchar(250)
GO

/****** Object:  UserDefinedTableType [dbo].[currency_id_array]    Script Date: 2016-03-23 02:20:21 PM ******/
CREATE TYPE [dbo].[product_currency_array] AS TABLE(
	[currency_id] [int] NULL,
	[is_base] [bit] NULL,
	[usr_field_name_1] varchar(250) NULL,
	[usr_field_val_1] varchar(250) NULL,
	[usr_field_name_2] varchar(250) NULL,
	[usr_field_val_2] varchar(250) NULL
)
GO