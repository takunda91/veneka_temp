--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[product_currency]
EXCEPT
SELECT [product_id],[currency_id]
FROM [{DATABASE_NAME}].[dbo].[product_currency]