USE [indigo_database_main_dev]
GO


INSERT INTO [user_roles] (user_role_id, user_role, allow_multiple_login, enterprise_only)
	VALUES (12, 'CMS_OPERATOR', 0, 0)
GO


--id	language_name
--0	English
--1	French
--2	Portuguese
--3	Spanish

INSERT INTO [user_roles_language](user_role_id, [language_id], [language_text])
SELECT user_role_id, 0, user_role
FROM [user_roles]
WHERE [user_roles].user_role_id NOT IN 
	(SELECT user_role_id FROM [user_roles_language] WHERE language_id = 0)


INSERT INTO [user_roles_language]	(user_role_id, [language_id], [language_text])
SELECT user_role_id, 1, user_role + '_fr'
FROM [user_roles]
WHERE [user_roles].user_role_id NOT IN 
	(SELECT user_role_id FROM [user_roles_language] WHERE language_id = 1)

INSERT INTO [user_roles_language]	(user_role_id, [language_id], [language_text])
SELECT user_role_id, 2, user_role + '_pt'
FROM [user_roles]
WHERE [user_roles].user_role_id NOT IN 
	(SELECT user_role_id FROM [user_roles_language] WHERE language_id = 2)

INSERT INTO [user_roles_language]	(user_role_id, [language_id], [language_text])
SELECT user_role_id, 3, user_role + '_es'
FROM [user_roles]
WHERE [user_roles].user_role_id NOT IN 
	(SELECT user_role_id FROM [user_roles_language] WHERE language_id = 3)
GO