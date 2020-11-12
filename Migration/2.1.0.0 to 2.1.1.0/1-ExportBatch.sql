USE [indigo_database_main_dev]
GO

ALTER TABLE [cards]
	ADD export_batch_id	bigint
GO

ALTER TABLE [cards]
ADD CONSTRAINT FK_export_batch_cards FOREIGN KEY (export_batch_id) 
    REFERENCES [export_batch] (export_batch_id) 
Go