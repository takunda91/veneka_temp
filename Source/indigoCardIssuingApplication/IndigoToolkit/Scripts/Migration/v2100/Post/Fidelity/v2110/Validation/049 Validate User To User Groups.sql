--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[users_to_users_groups]
EXCEPT
SELECT *
FROM [{DATABASE_NAME}].[dbo].[users_to_users_groups]