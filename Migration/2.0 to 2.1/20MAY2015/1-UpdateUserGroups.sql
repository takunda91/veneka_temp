USE [indigo_database_main_dev]
GO

ALTER TABLE [user_group]
	ADD mask_screen_pan bit

GO

ALTER TABLE [user_group]
	ADD mask_report_pan bit

GO

UPDATE [user_group]
SET mask_screen_pan = 1,
	mask_report_pan = 1
GO

ALTER TABLE [user_group]
	ALTER COLUMN mask_screen_pan bit NOT NULL

GO

ALTER TABLE [user_group]
	ALTER COLUMN mask_report_pan bit NOT NULL

GO