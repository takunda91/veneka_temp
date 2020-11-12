USE [indigo_database_main_dev]
GO

ALTER TABLE [pin_batch_statuses_flow]
	ADD reject_pin_batch_statuses_id int 
GO

ALTER TABLE [pin_batch_statuses_flow]
	ADD reject_pin_card_statuses_id int
GO

ALTER TABLE [pin_batch_statuses_flow]
	ADD flow_pin_card_statuses_id int
GO


INSERT INTO [pin_batch_statuses] (pin_batch_statuses_id, pin_batch_statuses_name)
	VALUES (5, 'REJECTED_AT_CC')
GO

INSERT INTO [pin_batch_statuses] (pin_batch_statuses_id, pin_batch_statuses_name)
	VALUES (6, 'REJECTED_AT_BRANCH')
GO

INSERT INTO pin_batch_statuses_language (pin_batch_statuses_id, language_id, language_text)
SELECT pin_batch_statuses_id, 0, pin_batch_statuses_name
FROM [pin_batch_statuses]
WHERE pin_batch_statuses_id >= 5

INSERT INTO pin_batch_statuses_language (pin_batch_statuses_id, language_id, language_text)
SELECT pin_batch_statuses_id, 1, pin_batch_statuses_name + '_fr'
FROM [pin_batch_statuses]
WHERE pin_batch_statuses_id >= 5

INSERT INTO pin_batch_statuses_language (pin_batch_statuses_id, language_id, language_text)
SELECT pin_batch_statuses_id, 2, pin_batch_statuses_name + '_pt'
FROM [pin_batch_statuses]
WHERE pin_batch_statuses_id >= 5

INSERT INTO pin_batch_statuses_language (pin_batch_statuses_id, language_id, language_text)
SELECT pin_batch_statuses_id, 3, pin_batch_statuses_name + '_es'
FROM [pin_batch_statuses]
WHERE pin_batch_statuses_id >= 5
GO


INSERT INTO [pin_batch_card_statuses] (pin_batch_card_statuses_id, pin_batch_card_statuses_name)
	VALUES (2, 'RECEIVED_AT_CC')
INSERT INTO [pin_batch_card_statuses] (pin_batch_card_statuses_id, pin_batch_card_statuses_name)
	VALUES (3, 'ALLOCATED_TO_BRANCH')
INSERT INTO [pin_batch_card_statuses] (pin_batch_card_statuses_id, pin_batch_card_statuses_name)
	VALUES (4, 'RECEIVED_AT_BRANCH')
INSERT INTO [pin_batch_card_statuses] (pin_batch_card_statuses_id, pin_batch_card_statuses_name)
	VALUES (5, 'REJECTED')
	