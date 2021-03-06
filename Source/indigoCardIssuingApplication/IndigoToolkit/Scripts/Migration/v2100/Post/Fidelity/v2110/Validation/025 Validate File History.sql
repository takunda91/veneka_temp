--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[file_history]
EXCEPT
SELECT [file_id]
      ,[issuer_id]
      ,[file_status_id]
      ,[file_type_id]
      ,[name_of_file]
      ,CAST([file_created_date] as datetime) [file_created_date]
      ,[file_size]
      ,CAST([load_date] as datetime) [load_date]
      ,[file_directory]
      ,[number_successful_records]
      ,[number_failed_records]
      ,[file_load_comments]
      ,[file_load_id]
FROM [{DATABASE_NAME}].[dbo].[file_history]