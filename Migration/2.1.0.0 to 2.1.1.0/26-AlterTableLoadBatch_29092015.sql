USE [indigo_database_main_dev]
GO

ALTER TABLE [load_batch]
	ADD load_batch_type_id INT NOT NULL DEFAULT 1
GO

ALTER TABLE [dbo].[load_batch]  WITH CHECK ADD  CONSTRAINT [FK_load_batch_type_id] FOREIGN KEY([load_batch_type_id])
REFERENCES [dbo].[load_batch_types] ([load_batch_type_id])
GO