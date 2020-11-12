USE [indigo_database_main_dev]
GO

ALTER TABLE [issuer_interface]
	ADD interface_area int
GO

UPDATE [issuer_interface]
SET interface_area = 0
GO

ALTER TABLE [issuer_interface]
	ALTER COLUMN interface_area int not null
GO

ALTER TABLE [issuer_interface]
	DROP CONSTRAINT PK_issuer_interface
GO

ALTER TABLE [issuer_interface]
	ADD CONSTRAINT PK_issuer_interface
		PRIMARY KEY (interface_type_id, issuer_id, interface_area)
GO 