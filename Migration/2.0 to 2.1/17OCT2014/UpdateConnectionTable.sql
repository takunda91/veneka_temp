USE [indigo_database_main_dev]
GO

ALTER TABLE [connection_parameters]
	ADD [timeout_milli] int
GO
ALTER TABLE [connection_parameters]
	ADD [buffer_size] int
GO

ALTER TABLE [connection_parameters]
	ADD [doc_type] char