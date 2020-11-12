USE indigo_database_main_dev
GO

----START ordering branch and distribution branch
ALTER TABLE [cards]
 ADD ordering_branch_id int
GO

UPDATE [cards]
SET ordering_branch_id = branch_id
GO

ALTER TABLE [cards]
 ALTER COLUMN ordering_branch_id int not null 
GO

ALTER TABLE [cards]
 ADD CONSTRAINT [FK_ordering_branch_id] FOREIGN KEY(ordering_branch_id)
 REFERENCES [dbo].[branch] ([branch_id])
 Go
 ----END

 ----START delivery branch and distribution branch
ALTER TABLE [cards]
 ADD delivery_branch_id int
GO

UPDATE [cards]
SET delivery_branch_id = branch_id
GO

ALTER TABLE [cards]
 ALTER COLUMN delivery_branch_id int not null 
GO

ALTER TABLE [cards]
 ADD CONSTRAINT [FK_delivery_branch_id] FOREIGN KEY(delivery_branch_id)
 REFERENCES [dbo].[branch] ([branch_id])
 Go
 ----END

 ALTER TABLE [branch_card_status]
	ADD branch_id int
GO

--Set previous records to current cards branch... before upgrade some of these may be incorrect
UPDATE [branch_card_status]
SET [branch_card_status].branch_id = [cards].branch_id
FROM [branch_card_status] INNER JOIN [cards] 
	ON [branch_card_status].card_id = [cards].card_id
GO

ALTER TABLE [branch_card_status]
 ALTER COLUMN branch_id int not null 
GO

ALTER TABLE [branch_card_status]
 ADD CONSTRAINT [FK_branch_card_status_branch_id] FOREIGN KEY(branch_id)
 REFERENCES [dbo].[branch] ([branch_id])
 Go
