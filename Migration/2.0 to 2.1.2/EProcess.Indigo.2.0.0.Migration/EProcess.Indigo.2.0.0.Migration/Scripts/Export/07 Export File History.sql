USE [DB_NAME]

SET IDENTITY_INSERT [dbo].[file_history] ON
INSERT INTO [dbo].[file_history]
           ([file_id]
		   ,[issuer_id]
           ,[file_status_id]
           ,[file_type_id]
           ,[name_of_file]
           ,[file_created_date]
           ,[file_size]
           ,[load_date]
           ,[file_directory]
           ,[number_successful_records]
           ,[number_failed_records]
           ,[file_load_comments]
           ,[file_load_id])
SELECT [file_id]
      ,[issuer_id]
      ,[file_status_id]
      ,[file_type_id]
      ,[name_of_file]
      ,[file_created_date]
      ,[file_size]
      ,[load_date]
      ,[file_directory]
      ,[number_successful_records]
      ,[number_failed_records]
      ,[file_load_comments]
      ,[file_load_id]
FROM [indigo_database_group].[dbo].[file_history]
WHERE [issuer_id] = @selected_issuer_id

SET IDENTITY_INSERT [dbo].[file_history] OFF