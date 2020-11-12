USE [indigo_database_main_dev]
GO

INSERT INTO [product_load_type] (product_load_type_id, product_load_type_name)
VALUES (0, 'NO_LOAD'),
(1, 'LOAD_TO_PROD'),
(2, 'LOAD_TO_DIST'),
(3, 'LOAD_TO_CENTRE')
GO

INSERT INTO [product_load_type_language] (product_load_type_id, language_id, language_text)
SELECT product_load_type_id, 0, product_load_type_name
FROM [product_load_type]
GO

INSERT INTO [product_load_type_language] (product_load_type_id, language_id, language_text)
SELECT product_load_type_id, 1, product_load_type_name+'_fr'
FROM [product_load_type]
GO

INSERT INTO [product_load_type_language] (product_load_type_id, language_id, language_text)
SELECT product_load_type_id, 2, product_load_type_name+'_es'
FROM [product_load_type]
GO

INSERT INTO [product_load_type_language] (product_load_type_id, language_id, language_text)
SELECT product_load_type_id, 3, product_load_type_name+'_pt'
FROM [product_load_type]
GO

ALTER TABLE [issuer_product]
	ADD product_load_type_id int 
GO

UPDATE [issuer_product]
SET product_load_type_id = 0
GO

ALTER TABLE [issuer_product]
	ALTER COLUMN product_load_type_id int NOT NULL
GO

ALTER TABLE [issuer_product]
	ADD CONSTRAINT FK_issuer_product_product_batch_type FOREIGN KEY (product_load_type_id) 
    REFERENCES [product_load_type] (product_load_type_id) 