--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].migration_customer_accounts(	)
EXCEPT
SELECT *
FROM [{DATABASE_NAME}].[dbo].validation_customer_accounts(	)