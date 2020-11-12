USE [indigo_database_main_dev]
GO

ALTER TABLE [issuer_product]
	ADD charge_fee_to_issuing_branch_YN bit
GO

UPDATE [issuer_product]
	SET charge_fee_to_issuing_branch_YN = 0
GO

ALTER TABLE [issuer_product]
	ALTER COLUMN charge_fee_to_issuing_branch_YN bit NOT NULL
GO