USE [indigo_database_main_dev]
GO

ALTER TABLE [dist_batch]
	ADD issuer_id int
GO

UPDATE [dist_batch]
SET issuer_id = [branch].issuer_id
FROM [dist_batch]
		INNER JOIN [branch]
			ON [dist_batch].branch_id = [branch].branch_id
GO

ALTER TABLE [dist_batch]
	ALTER COLUMN issuer_id int not null
GO
ALTER TABLE [dist_batch]
	ADD CONSTRAINT FK_dist_batch_issuer FOREIGN KEY (issuer_id)
		REFERENCES [issuer] (issuer_id) ;
GO

ALTER TABLE [dist_batch]
	ALTER COLUMN branch_id int null
GO

ALTER TABLE [dist_batch]
	DROP CONSTRAINT FK_distribution_batch_branch ;
GO

ALTER TABLE [dist_batch]
	ADD CONSTRAINT FK_distribution_batch_branch FOREIGN KEY (branch_id)
		REFERENCES [branch] (branch_id) ;
GO