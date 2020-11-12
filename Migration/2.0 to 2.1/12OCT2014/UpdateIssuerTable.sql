USE [indigo_database_main_dev]
GO

ALTER TABLE [issuer]
	ADD classic_card_issue_YN bit
GO

UPDATE [issuer]
SET classic_card_issue_YN = 0
GO

ALTER TABLE [issuer]
	ALTER COLUMN classic_card_issue_YN bit not null
GO

ALTER TABLE [issuer]
	ALTER COLUMN card_ref_preference bit
GO

UPDATE [issuer]
SET card_ref_preference = 0
GO

ALTER TABLE [issuer]
	ALTER COLUMN card_ref_preference bit not null
GO