USE [indigo_database_main_dev]
GO

ALTER TABLE [dbo].[issuer] ADD [EnableCardFileLoader] BIT NOT NULL DEFAULT ((0))
GO