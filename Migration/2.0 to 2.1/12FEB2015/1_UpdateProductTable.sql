USE [indigo_database_main_dev]
GO

ALTER TABLE [issuer_product]
	ADD enable_instant_pin_YN bit

GO

UPDATE [issuer_product]
SET enable_instant_pin_YN = 0
GO

ALTER TABLE [issuer_product]
	ALTER COLUMN enable_instant_pin_YN bit NOT NULL