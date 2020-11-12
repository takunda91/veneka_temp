USE [indigo_database_main_dev]
GO

ALTER TABLE [cards]
	DROP COLUMN cvv
GO

ALTER TABLE [cards]
	DROP COLUMN pin