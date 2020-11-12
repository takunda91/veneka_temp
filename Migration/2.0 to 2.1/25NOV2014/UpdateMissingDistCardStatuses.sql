USE [indigo_database_main_dev]
GO

INSERT INTO [dist_card_statuses_language] (dist_card_status_id, [language_id], [language_text])
SELECT dist_card_status_id, 0, dist_card_status_name
FROM [dist_card_statuses]
WHERE [dist_card_statuses].dist_card_status_id NOT IN 
	(SELECT dist_card_status_id FROM [dist_card_statuses_language] WHERE language_id = 0)

INSERT INTO [dist_card_statuses_language] (dist_card_status_id, [language_id], [language_text])
SELECT dist_card_status_id, 1, dist_card_status_name + '_fr'
FROM [dist_card_statuses]
WHERE [dist_card_statuses].dist_card_status_id NOT IN 
	(SELECT dist_card_status_id FROM [dist_card_statuses_language] WHERE language_id = 1)

INSERT INTO [dist_card_statuses_language] (dist_card_status_id, [language_id], [language_text])
SELECT dist_card_status_id, 2, dist_card_status_name + '_pt'
FROM [dist_card_statuses]
WHERE [dist_card_statuses].dist_card_status_id NOT IN 
	(SELECT dist_card_status_id FROM [dist_card_statuses_language] WHERE language_id = 2)

INSERT INTO [dist_card_statuses_language] (dist_card_status_id, [language_id], [language_text])
SELECT dist_card_status_id, 3, dist_card_status_name + 'es'
FROM [dist_card_statuses]
WHERE [dist_card_statuses].dist_card_status_id NOT IN 
	(SELECT dist_card_status_id FROM [dist_card_statuses_language] WHERE language_id = 3)
