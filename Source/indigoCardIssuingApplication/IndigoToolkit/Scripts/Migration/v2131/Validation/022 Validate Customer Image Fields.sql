--Get records from target and check if there are any that arent in source
SELECT [customer_account_id]
      ,[product_field_id]      
FROM [{SOURCE_DATABASE_NAME}].[dbo].[customer_image_fields]
EXCEPT
SELECT [customer_account_id]
      ,[product_field_id]
FROM [{DATABASE_NAME}].[dbo].[customer_image_fields]