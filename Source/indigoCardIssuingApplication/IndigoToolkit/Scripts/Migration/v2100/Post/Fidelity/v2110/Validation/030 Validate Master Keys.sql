--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].migration_masterkeys( )
EXCEPT
SELECT *
FROM [{DATABASE_NAME}].[dbo].validation_masterkeys( )