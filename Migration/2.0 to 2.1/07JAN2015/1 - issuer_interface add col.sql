USE [indigo_database_main_dev]
GO

ALTER TABLE [issuer_interface]
	ADD interface_guid char(36) null
