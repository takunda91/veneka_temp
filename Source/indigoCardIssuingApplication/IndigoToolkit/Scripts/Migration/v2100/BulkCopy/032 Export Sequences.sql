[{DATABASE_NAME}].[dbo].[sequences]
SELECT [sequence_name]
      ,[last_sequence_number]
      ,[last_updated]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[sequences]