[{DATABASE_NAME}].[dbo].[product_currency]
SELECT [new_product_id] as [product_id], [currency_id], 0 as [is_base]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[product_currency]
		INNER JOIN [{SOURCE_DATABASE_NAME}].[dbo].temp_migrate_products as temp 
			ON [product_currency].product_id = temp.product_id