USE [indigo_database_main_dev]
GO

ALTER TABLE [pin_reissue]
	ADD [pin_reissue_id] bigint IDENTITY
GO

ALTER TABLE [pin_reissue]
   ADD CONSTRAINT PK_pin_reissue_id
   PRIMARY KEY([pin_reissue_id])
GO

ALTER TABLE [pin_reissue]
	ADD primary_index_number varbinary(max)
GO

ALTER TABLE [pin_reissue]
	ADD [request_expiry] datetime2
GO

UPDATE [pin_reissue]
SET [request_expiry] = DATEADD(mi, 60, reissue_date)
GO

ALTER TABLE [pin_reissue]
 ALTER COLUMN [request_expiry] datetime2 NOT NULL

