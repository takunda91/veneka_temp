[{DATABASE_NAME}].[dbo].[product_currency]
SELECT [product_id], [currency_id], 0 as [is_base]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[product_currency]