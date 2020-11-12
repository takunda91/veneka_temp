USE [indigo_database_main_dev]
GO

ALTER TABLE [cards]
	ADD fee_charged DECIMAL(10,4) NULL,
		fee_waiver_YN bit NULL,
		fee_editable_YN bit NULL