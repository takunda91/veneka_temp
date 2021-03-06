--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[file_load]
EXCEPT
SELECT [file_load_id]
      ,CAST([file_load_start] as datetime) [file_load_start]
      ,CAST([file_load_end] as datetime) [file_load_end]
      ,[user_id]
      ,[files_to_process]
FROM [{DATABASE_NAME}].[dbo].[file_load]