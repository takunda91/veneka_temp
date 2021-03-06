--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[load_batch]
EXCEPT
SELECT [load_batch_id]
      ,[file_id]
      ,[load_batch_status_id]
      ,[no_cards]
      ,CAST([load_date] as datetime) [load_date]
      ,[load_batch_reference]
      ,[load_batch_type_id]
FROM [{DATABASE_NAME}].[dbo].[load_batch]