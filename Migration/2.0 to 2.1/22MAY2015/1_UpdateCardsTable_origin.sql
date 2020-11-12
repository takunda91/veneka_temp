USE [indigo_database_main_dev]
GO

ALTER TABLE [cards]
 ADD origin_branch_id int 
 GO

UPDATE [cards]
SET origin_branch_id = [cards].branch_id
Go

ALTER TABLE [cards]
 ALTER COLUMN origin_branch_id int NOT NULL
 GO

ALTER TABLE [cards]
ADD CONSTRAINT FK_origin_branch_id FOREIGN KEY (origin_branch_id) 
    REFERENCES [branch] (branch_id)
GO