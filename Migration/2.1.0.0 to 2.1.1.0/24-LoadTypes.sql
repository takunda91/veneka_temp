USE [indigo_database_main_dev]
GO

INSERT INTO [product_load_type] (product_load_type_id, product_load_type_name)
VALUES (5, 'LOAD_REQUESTS')
GO

INSERT INTO [product_load_type_language] (product_load_type_id, language_id, language_text)
SELECT product_load_type_id, 0, product_load_type_name
FROM [product_load_type]
WHERE product_load_type_id = 5
GO

INSERT INTO [product_load_type_language] (product_load_type_id, language_id, language_text)
SELECT product_load_type_id, 1, product_load_type_name+'_fr'
FROM [product_load_type]
WHERE product_load_type_id = 5
GO

INSERT INTO [product_load_type_language] (product_load_type_id, language_id, language_text)
SELECT product_load_type_id, 2, product_load_type_name+'_es'
FROM [product_load_type]
WHERE product_load_type_id = 5
GO

INSERT INTO [product_load_type_language] (product_load_type_id, language_id, language_text)
SELECT product_load_type_id, 3, product_load_type_name+'_pt'
FROM [product_load_type]
WHERE product_load_type_id = 5
GO