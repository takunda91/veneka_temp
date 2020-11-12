USE [indigo_database_main_dev]
GO

ALTER TABLE [issuer]
	ADD [back_office_pin_auth_YN] bit
GO

UPDATE [issuer]
SET [back_office_pin_auth_YN] = 1
GO

ALTER TABLE [issuer]
	ALTER COLUMN [back_office_pin_auth_YN] bit not null
GO

INSERT INTO [branch_card_statuses] (branch_card_statuses_id, branch_card_statuses_name)
	VALUES (14, 'PIN_AUTHORISED')
GO

INSERT INTO [branch_card_statuses_language] (branch_card_statuses_id, language_id, language_text)
	VALUES (14, 0, 'PIN_AUTHORISED')
INSERT INTO [branch_card_statuses_language] (branch_card_statuses_id, language_id, language_text)
	VALUES (14, 1, 'PIN_AUTHORISED_fr')
INSERT INTO [branch_card_statuses_language] (branch_card_statuses_id, language_id, language_text)
	VALUES (14, 2, 'PIN_AUTHORISED_pt')
INSERT INTO [branch_card_statuses_language] (branch_card_statuses_id, language_id, language_text)
	VALUES (14, 3, 'PIN_AUTHORISED_es')
GO

ALTER TABLE [customer_account_type]
	ADD [active_YN] bit
GO

UPDATE [customer_account_type]
	SET [active_YN] = 1
GO

ALTER TABLE [customer_account_type]
	ALTER COLUMN [active_YN] bit NOT NULL
GO