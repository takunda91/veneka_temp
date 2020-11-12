USE [indigo_database_main_dev]
GO

INSERT INTO [audit_action] (audit_action_id, audit_action_name)
	VALUES (9, 'Administration')
GO

INSERT INTO [audit_action_language] (audit_action_id, language_id, language_text)
	VALUES (9, 0, 'Administration')
GO
INSERT INTO [audit_action_language] (audit_action_id, language_id, language_text)
	VALUES (9, 1, 'Administration_fr')
GO
INSERT INTO [audit_action_language] (audit_action_id, language_id, language_text)
	VALUES (9, 2, 'Administration_pt')
GO
INSERT INTO [audit_action_language] (audit_action_id, language_id, language_text)
	VALUES (9, 3, 'Administration_es')
GO