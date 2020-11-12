USE [indigo_database_group]
GO

UPDATE [user]
	SET user_status_id = 2
WHERE [user_id] = -1
GO