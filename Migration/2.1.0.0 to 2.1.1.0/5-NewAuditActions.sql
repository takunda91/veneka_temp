USE [indigo_database_main_dev]
GO

INSERT INTO audit_action (audit_action_id, audit_action_name)
VALUES (11, 'ExportBatch')
GO

INSERT INTO audit_action_language (audit_action_id, language_id, language_text)
SELECT audit_action_id, 0, audit_action_name
FROM audit_action
WHERE audit_action_id = 11
GO

INSERT INTO audit_action_language (audit_action_id, language_id, language_text)
SELECT audit_action_id, 1, audit_action_name+'_fr'
FROM audit_action
WHERE audit_action_id = 11
GO

INSERT INTO audit_action_language (audit_action_id, language_id, language_text)
SELECT audit_action_id, 2, audit_action_name+'_pt'
FROM audit_action
WHERE audit_action_id = 11
GO

INSERT INTO audit_action_language (audit_action_id, language_id, language_text)
SELECT audit_action_id, 3, audit_action_name+'_es'
FROM audit_action
WHERE audit_action_id = 11
GO
