USE [indigo_database_main_dev]
GO

-- USE THESE SCRIPTS TO UPDATE DATABASE FROM VERSION 2.0.0.0 TO 2.1.0.0


---------------------------------------------------------------------------------------------------------------
----------------------------------------    ADD NEW DIST BATCH STATUS ----------------------------------------
---------------------------------------------------------------------------------------------------------------
INSERT INTO dist_batch_statuses(dist_batch_statuses_id, dist_batch_status_name)
	VALUES (9, 'APPROVED_FOR_PRODUCTION')
INSERT INTO dist_batch_statuses(dist_batch_statuses_id, dist_batch_status_name)
	VALUES (10, 'SENT_TO_CMS')
INSERT INTO dist_batch_statuses(dist_batch_statuses_id, dist_batch_status_name)
	VALUES (11, 'PROCESSED_IN_CMS')
INSERT INTO dist_batch_statuses(dist_batch_statuses_id, dist_batch_status_name)
	VALUES (12, 'AT_CARD_PRODUCTION')
INSERT INTO dist_batch_statuses(dist_batch_statuses_id, dist_batch_status_name)
	VALUES (13, 'CARDS_PRODUCED')
INSERT INTO dist_batch_statuses(dist_batch_statuses_id, dist_batch_status_name)
	VALUES (14, 'RECEIVED_AT_CARD_CENTER')

INSERT INTO dist_batch_statuses(dist_batch_statuses_id, dist_batch_status_name)
	VALUES (15, 'FAILED_IN_CMS')
INSERT INTO dist_batch_statuses(dist_batch_statuses_id, dist_batch_status_name)
	VALUES (16, 'REJECTED_AT_CARD_CENTER')
GO

--id	language_name
--0	English
--1	French
--2	Portuguese
--3	Spanish

INSERT INTO [dist_batch_statuses_language](dist_batch_statuses_id, [language_id], [language_text])
SELECT dist_batch_statuses_id, 0, dist_batch_status_name
FROM [dist_batch_statuses]
WHERE [dist_batch_statuses].dist_batch_statuses_id NOT IN 
	(SELECT dist_batch_statuses_id FROM [dist_batch_statuses_language] WHERE language_id = 0)


INSERT INTO [dist_batch_statuses_language]	(dist_batch_statuses_id, [language_id], [language_text])
SELECT dist_batch_statuses_id, 1, dist_batch_status_name + '_fr'
FROM [dist_batch_statuses]
WHERE [dist_batch_statuses].dist_batch_statuses_id NOT IN 
	(SELECT dist_batch_statuses_id FROM [dist_batch_statuses_language] WHERE language_id = 1)

INSERT INTO [dist_batch_statuses_language]	(dist_batch_statuses_id, [language_id], [language_text])
SELECT dist_batch_statuses_id, 2, dist_batch_status_name + '_pt'
FROM [dist_batch_statuses]
WHERE [dist_batch_statuses].dist_batch_statuses_id NOT IN 
	(SELECT dist_batch_statuses_id FROM [dist_batch_statuses_language] WHERE language_id = 2)

INSERT INTO [dist_batch_statuses_language]	(dist_batch_statuses_id, [language_id], [language_text])
SELECT dist_batch_statuses_id, 3, dist_batch_status_name + '_es'
FROM [dist_batch_statuses]
WHERE [dist_batch_statuses].dist_batch_statuses_id NOT IN 
	(SELECT dist_batch_statuses_id FROM [dist_batch_statuses_language] WHERE language_id = 3)
GO