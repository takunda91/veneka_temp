USE [indigo_database_main_dev]
GO

INSERT INTO [dist_batch_statuses] (dist_batch_statuses_id, dist_batch_status_name)
VALUES (20, 'LOAD_PENDING')
		,(21, 'LOAD_COMPLETE')
GO

INSERT INTO [dist_batch_statuses_language] (dist_batch_statuses_id, language_id, language_text)
SELECT dist_batch_statuses_id, 0, dist_batch_status_name
FROM [dist_batch_statuses]
WHERE dist_batch_statuses_id NOT IN (SELECT dist_batch_statuses_id 
										FROM [dist_batch_statuses_language] 
										WHERE language_id = 0)
GO

INSERT INTO [dist_batch_statuses_language] (dist_batch_statuses_id, language_id, language_text)
SELECT dist_batch_statuses_id, 1, dist_batch_status_name+'_fr'
FROM [dist_batch_statuses]
WHERE dist_batch_statuses_id NOT IN (SELECT dist_batch_statuses_id 
										FROM [dist_batch_statuses_language] 
										WHERE language_id = 1)
GO

INSERT INTO [dist_batch_statuses_language] (dist_batch_statuses_id, language_id, language_text)
SELECT dist_batch_statuses_id, 2, dist_batch_status_name+'_pt'
FROM [dist_batch_statuses]
WHERE dist_batch_statuses_id NOT IN (SELECT dist_batch_statuses_id 
										FROM [dist_batch_statuses_language] 
										WHERE language_id = 2)
GO

INSERT INTO [dist_batch_statuses_language] (dist_batch_statuses_id, language_id, language_text)
SELECT dist_batch_statuses_id, 3, dist_batch_status_name+'_es'
FROM [dist_batch_statuses]
WHERE dist_batch_statuses_id NOT IN (SELECT dist_batch_statuses_id 
										FROM [dist_batch_statuses_language] 
										WHERE language_id = 3)
GO
--------------------------------------------------------------------------------------------------------
INSERT INTO [dist_card_statuses] (dist_card_status_id, dist_card_status_name)
VALUES (20, 'LOAD_PENDING')
		,(21, 'LOAD_COMPLETE')
GO

INSERT INTO [dist_card_statuses_language] (dist_card_status_id, language_id, language_text)
SELECT dist_card_status_id, 0, dist_card_status_name
FROM [dist_card_statuses]
WHERE dist_card_status_id NOT IN (SELECT dist_card_status_id 
										FROM [dist_card_statuses_language] 
										WHERE language_id = 0)
GO

INSERT INTO [dist_card_statuses_language] (dist_card_status_id, language_id, language_text)
SELECT dist_card_status_id, 1, dist_card_status_name + '_fr'
FROM [dist_card_statuses]
WHERE dist_card_status_id NOT IN (SELECT dist_card_status_id 
										FROM [dist_card_statuses_language] 
										WHERE language_id = 1)
GO

INSERT INTO [dist_card_statuses_language] (dist_card_status_id, language_id, language_text)
SELECT dist_card_status_id, 2, dist_card_status_name + '_pt'
FROM [dist_card_statuses]
WHERE dist_card_status_id NOT IN (SELECT dist_card_status_id 
										FROM [dist_card_statuses_language] 
										WHERE language_id = 2)
GO

INSERT INTO [dist_card_statuses_language] (dist_card_status_id, language_id, language_text)
SELECT dist_card_status_id, 3, dist_card_status_name + '_es'
FROM [dist_card_statuses]
WHERE dist_card_status_id NOT IN (SELECT dist_card_status_id 
										FROM [dist_card_statuses_language] 
										WHERE language_id = 3)
GO

----------------------------------------------------------------------------------------------------
INSERT INTO [product_load_type] (product_load_type_id, product_load_type_name)
VALUES (4, 'LOAD_TO_EXISTING')
GO

INSERT INTO [product_load_type_language] (product_load_type_id, language_id, language_text)
SELECT product_load_type_id, 0, product_load_type_name
FROM [product_load_type]
WHERE product_load_type_id NOT IN (SELECT product_load_type_id 
										FROM [product_load_type_language] 
										WHERE language_id = 0)
GO

INSERT INTO [product_load_type_language] (product_load_type_id, language_id, language_text)
SELECT product_load_type_id, 1, product_load_type_name + '_fr'
FROM [product_load_type]
WHERE product_load_type_id NOT IN (SELECT product_load_type_id 
										FROM [product_load_type_language] 
										WHERE language_id = 1)
GO

INSERT INTO [product_load_type_language] (product_load_type_id, language_id, language_text)
SELECT product_load_type_id, 2, product_load_type_name + '_pt'
FROM [product_load_type]
WHERE product_load_type_id NOT IN (SELECT product_load_type_id 
										FROM [product_load_type_language] 
										WHERE language_id = 2)
GO

INSERT INTO [product_load_type_language] (product_load_type_id, language_id, language_text)
SELECT product_load_type_id, 3, product_load_type_name + '_es'
FROM [product_load_type]
WHERE product_load_type_id NOT IN (SELECT product_load_type_id 
										FROM [product_load_type_language] 
										WHERE language_id = 3)
---------------------------------------------------------------------------------------------------
--CARDS_NOT_ORDERED
INSERT INTO [file_statuses] (file_status_id, file_status)
VALUES (20, 'CARDS_NOT_ORDERED')
		,(21, 'ORDERED_CARD_REF_MISSING')
		,(22, 'ORDERED_CARD_PRODUCT_MISS_MATCH')
GO

INSERT INTO [file_statuses_language] (file_status_id, language_id, language_text)
SELECT file_status_id, 0, file_status
FROM [file_statuses]
WHERE file_status_id NOT IN (SELECT file_status_id 
										FROM [file_statuses_language] 
										WHERE language_id = 0)
GO

INSERT INTO [file_statuses_language] (file_status_id, language_id, language_text)
SELECT file_status_id, 1, file_status + '_fr'
FROM [file_statuses]
WHERE file_status_id NOT IN (SELECT file_status_id 
										FROM [file_statuses_language] 
										WHERE language_id = 1)
GO

INSERT INTO [file_statuses_language] (file_status_id, language_id, language_text)
SELECT file_status_id, 2, file_status + '_pt'
FROM [file_statuses]
WHERE file_status_id NOT IN (SELECT file_status_id 
										FROM [file_statuses_language] 
										WHERE language_id = 2)
GO

INSERT INTO [file_statuses_language] (file_status_id, language_id, language_text)
SELECT file_status_id, 3, file_status + '_es'
FROM [file_statuses]
WHERE file_status_id NOT IN (SELECT file_status_id 
										FROM [file_statuses_language] 
										WHERE language_id = 3)
GO
