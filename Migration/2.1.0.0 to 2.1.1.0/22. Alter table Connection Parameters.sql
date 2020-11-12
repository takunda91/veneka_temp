USE [indigo_database_main_dev]
GO

ALTER TABLE connection_parameters
	ADD [file_delete_YN] BIT DEFAULT 1
GO

ALTER TABLE connection_parameters
	ADD  [file_encryption_type_id] INT DEFAULT 1
GO

ALTER TABLE connection_parameters
	ADD [duplicate_file_check_YN] BIT DEFAULT 1
GO

ALTER TABLE connection_parameters
ADD FOREIGN KEY ([file_encryption_type_id])
REFERENCES file_encryption_type([file_encryption_type_id])
GO