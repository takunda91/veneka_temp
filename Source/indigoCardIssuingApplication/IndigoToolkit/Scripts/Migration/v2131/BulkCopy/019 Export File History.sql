[{DATABASE_NAME}].[dbo].[file_history]
SELECT [file_id]
      ,[issuer_id]
      ,[file_status_id]
      ,[file_type_id]
      ,[name_of_file]
      ,ToDateTimeOffset([file_created_date], DATENAME(tz, SYSDATETIMEOFFSET())) as [file_created_date]
      ,[file_size]
      ,ToDateTimeOffset([load_date], DATENAME(tz, SYSDATETIMEOFFSET())) as [load_date]
      ,[file_directory]
      ,[number_successful_records]
      ,[number_failed_records]
      ,[file_load_comments]
      ,[file_load_id]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[file_history]