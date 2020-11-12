USE [indigo_database_main_dev]
GO

ALTER TABLE [dist_batch]
	ADD [dist_batch_type_id] int
		REFERENCES [dist_batch_type] ([dist_batch_type_id])
GO

INSERT INTO dist_batch_type (dist_batch_type_id, dist_batch_type_name)
VALUES (0, 'PRODUCTION')

INSERT INTO dist_batch_type (dist_batch_type_id, dist_batch_type_name)
VALUES (1, 'DISTRIBUTION')
GO

UPDATE [dist_batch]
SET dist_batch_type_id = 0

ALTER TABLE [dist_batch]
	ALTER COLUMN [dist_batch_type_id] int NOT NULL