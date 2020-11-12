USE [indigo_database_main_dev]
GO

INSERT INTO [dist_batch_statuses] (dist_batch_statuses_id, dist_batch_status_name)
	VALUES (17, 'SENT_TO_PRINTER')

INSERT INTO [dist_batch_statuses] (dist_batch_statuses_id, dist_batch_status_name)
	VALUES (18, 'PIN_PRINTED')
GO

INSERT INTO [dist_batch_statuses_language] (dist_batch_statuses_id, language_id, language_text)
SELECT dist_batch_statuses_id, 0, dist_batch_status_name
FROM [dist_batch_statuses]
WHERE dist_batch_statuses_id >= 17

INSERT INTO [dist_batch_statuses_language] (dist_batch_statuses_id, language_id, language_text)
SELECT dist_batch_statuses_id, 1, dist_batch_status_name + '_fr'
FROM [dist_batch_statuses]
WHERE dist_batch_statuses_id >= 17

INSERT INTO [dist_batch_statuses_language] (dist_batch_statuses_id, language_id, language_text)
SELECT dist_batch_statuses_id, 2, dist_batch_status_name + '_pt'
FROM [dist_batch_statuses]
WHERE dist_batch_statuses_id >= 17

INSERT INTO [dist_batch_statuses_language] (dist_batch_statuses_id, language_id, language_text)
SELECT dist_batch_statuses_id, 3, dist_batch_status_name + '_es'
FROM [dist_batch_statuses]
WHERE dist_batch_statuses_id >= 17
GO

INSERT INTO [dist_card_statuses] (dist_card_status_id, dist_card_status_name)
	VALUES (17, 'PIN_PRINTED')
GO

INSERT INTO [dist_card_statuses_language] (dist_card_status_id, language_id, language_text)
SELECT dist_card_status_id, 0, dist_card_status_name
FROM [dist_card_statuses]
WHERE dist_card_status_id = 17

INSERT INTO [dist_card_statuses_language] (dist_card_status_id, language_id, language_text)
SELECT dist_card_status_id, 1, dist_card_status_name + '_fr'
FROM [dist_card_statuses]
WHERE dist_card_status_id = 17

INSERT INTO [dist_card_statuses_language] (dist_card_status_id, language_id, language_text)
SELECT dist_card_status_id, 2, dist_card_status_name + '_pt'
FROM [dist_card_statuses]
WHERE dist_card_status_id = 17

INSERT INTO [dist_card_statuses_language] (dist_card_status_id, language_id, language_text)
SELECT dist_card_status_id, 3, dist_card_status_name + '_es'
FROM [dist_card_statuses]
WHERE dist_card_status_id = 17