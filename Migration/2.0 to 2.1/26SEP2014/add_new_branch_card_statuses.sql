USE [indigo_database_main_dev]
GO

-- USE THESE SCRIPTS TO UPDATE DATABASE FROM VERSION 2.0.0.0 TO 2.1.0.0


---------------------------------------------------------------------------------------------------------------
----------------------------------------    ADD NEW BRANCH CARD STATUS ----------------------------------------
---------------------------------------------------------------------------------------------------------------
INSERT INTO branch_card_statuses (branch_card_statuses_id, branch_card_statuses_name)
	VALUES (11, 'MAKERCHECKER_REJECT')
INSERT INTO branch_card_statuses (branch_card_statuses_id, branch_card_statuses_name)
	VALUES (12, 'CARD_REQUEST_DELETED')
GO

--id	language_name
--0	English
--1	French
--2	Portuguese
--3	Spanish

INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (11, 0, 'MAKERCHECKER_REJECT')
INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (11, 1, 'MAKERCHECKER_REJECT_fr')
INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (11, 2, 'MAKERCHECKER_REJECT_pt')
INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (11, 3, 'MAKERCHECKER_REJECT_es')

INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (12, 0, 'CARD_REQUEST_DELETED')
INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (12, 1, 'CARD_REQUEST_DELETED_fr')
INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (12, 2, 'CARD_REQUEST_DELETED_pt')
INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (12, 3, 'CARD_REQUEST_DELETED_es')
GO