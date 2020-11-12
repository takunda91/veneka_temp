USE [indigo_database_main_dev]
GO

ALTER TABLE [issuer_product]
 ADD fee_id int
 GO

ALTER TABLE [issuer_product]
	ADD CONSTRAINT FK_product_fee FOREIGN KEY (fee_id)
    REFERENCES [product_fee] (fee_id);
GO

ALTER TABLE [sub_product]
 ADD fee_id int
 GO

ALTER TABLE [sub_product]
	ADD CONSTRAINT FK_subproduct_fee FOREIGN KEY (fee_id)
    REFERENCES [product_fee] (fee_id);