USE [indigo_database_main_dev]
GO

-- USE THESE SCRIPTS TO UPDATE DATABASE FROM VERSION 2.0.0.0 TO 2.1.0.0


---------------------------------------------------------------------------------------------------------------
----------------------------------------    ADD NEW BRANCH CARD STATUS ----------------------------------------
---------------------------------------------------------------------------------------------------------------
INSERT INTO branch_card_statuses (branch_card_statuses_id, branch_card_statuses_name)
	VALUES (13, 'REDISTRIBUTED')
GO

--id	language_name
--0	English
--1	French
--2	Portuguese
--3	Spanish

INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (13, 0, 'REDISTRIBUTED')
INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (13, 1, 'REDISTRIBUTED_fr')
INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (13, 2, 'REDISTRIBUTED_pt')
INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (13, 3, 'REDISTRIBUTED_es')


GO