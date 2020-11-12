USE [indigo_database_main_dev]
GO

ALTER TABLE [connection_parameters]
	ADD private_key varbinary(max)

ALTER TABLE [connection_parameters]
	ADD public_key varbinary(max)