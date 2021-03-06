--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[sequences]
EXCEPT
SELECT [sequence_name]
      ,[last_sequence_number]
      ,CAST([last_updated] as datetime) [last_updated]
FROM [{DATABASE_NAME}].[dbo].[sequences]