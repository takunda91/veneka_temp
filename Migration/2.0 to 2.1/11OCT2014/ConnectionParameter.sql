USE [indigo_database_main_dev]
GO

ALTER TABLE [connection_parameters]
	ADD [header_length] int,
		 [identification] varbinary(max)
		 