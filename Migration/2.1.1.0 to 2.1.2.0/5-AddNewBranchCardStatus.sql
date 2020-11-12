USE [indigo_database_main_dev]
GO

INSERT INTO branch_card_statuses (branch_card_statuses_id, branch_card_statuses_name)
VALUES (15, 'CMS_REUPLOAD')
GO

INSERT INTO branch_card_statuses_language (branch_card_statuses_id, language_id, language_text)
VALUES (15, 0, 'CMS_REUPLOAD')

INSERT INTO branch_card_statuses_language (branch_card_statuses_id, language_id, language_text)
VALUES (15, 1, 'CMS_REUPLOAD_fr')

INSERT INTO branch_card_statuses_language (branch_card_statuses_id, language_id, language_text)
VALUES (15, 2, 'CMS_REUPLOAD_pt')

INSERT INTO branch_card_statuses_language (branch_card_statuses_id, language_id, language_text)
VALUES (15, 3, 'CMS_REUPLOAD_es')