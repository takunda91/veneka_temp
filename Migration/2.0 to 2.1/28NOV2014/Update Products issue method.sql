USE [indigo_database_main_dev]
GO
--Product
ALTER TABLE [issuer_product]
	ADD card_issue_method_id int 
GO

ALTER TABLE [issuer_product]
	ADD CONSTRAINT FK_product_issue_method FOREIGN KEY (card_issue_method_id)
    REFERENCES [card_issue_method] (card_issue_method_id);
GO

UPDATE [issuer_product]
SET card_issue_method_id = 0
GO

ALTER TABLE [issuer_product]
	ALTER COLUMN card_issue_method_id int NOT NULL
GO

--Sub_product
ALTER TABLE [sub_product]
	ADD card_issue_method_id int 
GO

ALTER TABLE [sub_product]
	ADD CONSTRAINT FK_sub_product_issue_method FOREIGN KEY (card_issue_method_id)
    REFERENCES [card_issue_method] (card_issue_method_id);
GO

UPDATE [sub_product]
SET card_issue_method_id = 0
GO

ALTER TABLE [sub_product]
	ALTER COLUMN card_issue_method_id int NOT NULL
GO

--Remove previous
ALTER TABLE [issuer_product_issue_method]
	DROP CONSTRAINT FK_issuer_product_issue_method_issuer_product
GO

ALTER TABLE [issuer_product_issue_method]
	DROP CONSTRAINT FK_issuer_product_issue_method_issuer_product_issue_method
GO

ALTER TABLE [issuer_product]
	DROP CONSTRAINT FK_product_issue_method
GO

ALTER TABLE [issuer_product_issue_method]
	DROP CONSTRAINT FK_issuer_product_issue_method_issuer_product
GO

DROP TABLE [issuer_product_issue_method]
GO