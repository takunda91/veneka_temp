[{DATABASE_NAME}].[dbo].[users_to_users_groups]
SELECT [user_id], [user_group_id]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[users_to_users_groups]
ORDER BY [user_id] ASC