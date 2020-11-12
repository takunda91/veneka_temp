USE [indigo_database_main_dev]
GO

ALTER TABLE [customer_account]
	ADD [domicile_branch_id] INT
GO

UPDATE [customer_account]
SET [domicile_branch_id] = 1
GO

ALTER TABLE [customer_account]
	ALTER COLUMN [domicile_branch_id] INT NOT NULL
GO

ALTER TABLE [customer_account]
ADD CONSTRAINT FK_customer_account_branch FOREIGN KEY ([domicile_branch_id]) 
    REFERENCES [branch] ([branch_id]) 