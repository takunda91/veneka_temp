--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].migration_user_password_history(	)
EXCEPT
SELECT *
FROM [{DATABASE_NAME}].[dbo].validation_user_password_history(	)