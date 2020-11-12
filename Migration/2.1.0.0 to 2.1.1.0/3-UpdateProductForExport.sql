USE [indigo_database_main_dev]
GO

ALTER TABLE [issuer_product]
	ADD cms_exportable_YN bit
GO

UPDATE [issuer_product]
SET cms_exportable_YN = 0
GO

ALTER TABLE [issuer_product]
	ALTER COLUMN cms_exportable_YN bit not null
GO