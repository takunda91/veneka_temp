USE [indigo_database_main_dev]
GO

INSERT INTO [user_roles] (user_role_id, user_role, enterprise_only, allow_multiple_login)
	VALUES (13, 'PIN_PRINTER_OPERATOR', 0, 0)

INSERT INTO [user_roles] (user_role_id, user_role, enterprise_only, allow_multiple_login)
	VALUES (14, 'CARD_CENTRE_PIN_OFFICER', 0, 0)

INSERT INTO [user_roles] (user_role_id, user_role, enterprise_only, allow_multiple_login)
	VALUES (15, 'BRANCH_PIN_OFFICER', 0, 0)
GO

INSERT INTO [user_roles_language] (user_role_id, language_id, language_text)
SELECT user_role_id, 0, user_role
FROM [user_roles]
WHERE user_role_id >= 13

INSERT INTO [user_roles_language] (user_role_id, language_id, language_text)
SELECT user_role_id, 1, user_role+'_fr'
FROM [user_roles]
WHERE user_role_id >= 13

INSERT INTO [user_roles_language] (user_role_id, language_id, language_text)
SELECT user_role_id, 2, user_role + '_pt'
FROM [user_roles]
WHERE user_role_id >= 13

INSERT INTO [user_roles_language] (user_role_id, language_id, language_text)
SELECT user_role_id, 3, user_role + '_es'
FROM [user_roles]
WHERE user_role_id >= 13