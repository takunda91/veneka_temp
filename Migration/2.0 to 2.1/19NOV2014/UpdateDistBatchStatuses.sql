USE [indigo_database_main_dev]
GO

INSERT INTO [dist_batch_statuses] (dist_batch_statuses_id, dist_batch_status_name)
	VALUES (19, 'DISPATCHED_TO_CC')
GO

INSERT INTO [dist_batch_statuses_language] (dist_batch_statuses_id, language_id, language_text)
SELECT dist_batch_statuses_id, 0, dist_batch_status_name
FROM [dist_batch_statuses]
WHERE dist_batch_statuses_id = 19

INSERT INTO [dist_batch_statuses_language] (dist_batch_statuses_id, language_id, language_text)
SELECT dist_batch_statuses_id, 1, dist_batch_status_name + '_fr'
FROM [dist_batch_statuses]
WHERE dist_batch_statuses_id = 19

INSERT INTO [dist_batch_statuses_language] (dist_batch_statuses_id, language_id, language_text)
SELECT dist_batch_statuses_id, 2, dist_batch_status_name + '_pt'
FROM [dist_batch_statuses]
WHERE dist_batch_statuses_id = 19

INSERT INTO [dist_batch_statuses_language] (dist_batch_statuses_id, language_id, language_text)
SELECT dist_batch_statuses_id, 3, dist_batch_status_name + '_es'
FROM [dist_batch_statuses]
WHERE dist_batch_statuses_id = 19
GO
