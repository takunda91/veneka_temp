USE [indigo_database_main_dev]
GO

UPDATE [user_roles]
SET user_role = 'CONFIG_ADMIN'
WHERE user_role_id = 0
GO

UPDATE [user_roles_language]
SET language_text = 'CONFIG_ADMIN'
WHERE user_role_id = 0
AND language_id = 0
GO

UPDATE [user_roles_language]
SET language_text = 'CONFIG_ADMIN_fr'
WHERE user_role_id = 0
AND language_id = 1
GO

UPDATE [user_roles_language]
SET language_text = 'CONFIG_ADMIN_pt'
WHERE user_role_id = 0
AND language_id = 2
GO

UPDATE [user_roles_language]
SET language_text = 'CONFIG_ADMIN_es'
WHERE user_role_id = 0
AND language_id = 3