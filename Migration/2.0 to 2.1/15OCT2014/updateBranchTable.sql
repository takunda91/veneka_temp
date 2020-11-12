USE [indigo_database_main_dev]
GO

ALTER TABLE [branch]
	ADD card_centre_branch_YN bit
GO

UPDATE [branch]
SET card_centre_branch_YN = 0
GO

ALTER TABLE [branch]
	ALTER COLUMN card_centre_branch_YN bit NOT NULL