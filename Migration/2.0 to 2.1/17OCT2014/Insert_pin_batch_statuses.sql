USE [indigo_database_main_dev]
GO

INSERT INTO pin_batch_statuses_language (pin_batch_statuses_id, language_id, language_text)
SELECT pin_batch_statuses_id, 0, pin_batch_statuses_name
FROM [pin_batch_statuses]

INSERT INTO pin_batch_statuses_language (pin_batch_statuses_id, language_id, language_text)
SELECT pin_batch_statuses_id, 1, pin_batch_statuses_name + '_fr'
FROM [pin_batch_statuses]

INSERT INTO pin_batch_statuses_language (pin_batch_statuses_id, language_id, language_text)
SELECT pin_batch_statuses_id, 2, pin_batch_statuses_name + '_pt'
FROM [pin_batch_statuses]

INSERT INTO pin_batch_statuses_language (pin_batch_statuses_id, language_id, language_text)
SELECT pin_batch_statuses_id, 3, pin_batch_statuses_name + '_es'
FROM [pin_batch_statuses]