USE [indigo_database_main_dev]
GO

ALTER TABLE [issuer_product]
	ADD fee_scheme_id int
GO

ALTER TABLE [issuer_product]
	ADD CONSTRAINT FK_product_fee_scheme FOREIGN KEY (fee_scheme_id)
    REFERENCES [product_fee_scheme] (fee_scheme_id);
GO

ALTER TABLE [sub_product]
	ADD fee_scheme_id int
GO

ALTER TABLE [sub_product]
	ADD CONSTRAINT FK_sub_product_fee_scheme FOREIGN KEY (fee_scheme_id)
    REFERENCES [product_fee_scheme] (fee_scheme_id);