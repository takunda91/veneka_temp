--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[export_batch]
EXCEPT
SELECT [export_batch_id]
      ,[issuer_id]
      ,[batch_reference]
      ,CAST([date_created] as datetime2) [date_created]
      ,[no_cards]
FROM [{DATABASE_NAME}].[dbo].[export_batch]