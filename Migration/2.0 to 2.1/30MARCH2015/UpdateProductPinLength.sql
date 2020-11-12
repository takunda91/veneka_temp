USE [indigo_database_main_dev]
GO

ALTER TABLE [issuer_product]
	ADD min_pin_length int null
	
GO

ALTER TABLE [issuer_product]
	ADD max_pin_length int null
GO

ALTER TABLE [issuer_product]
	ADD CONSTRAINT chkPinMinMaxLength CHECK (min_pin_length >= 4);
GO

ALTER TABLE [issuer_product]
	ADD CONSTRAINT chkPinMinLength CHECK (max_pin_length >= min_pin_length);
GO

UPDATE [issuer_product]
SET min_pin_length = 4,
	max_pin_length = 8

GO

ALTER TABLE [issuer_product]
	ALTER COLUMN min_pin_length int not null
GO

ALTER TABLE [issuer_product]
	ALTER COLUMN max_pin_length int not null
GO

