USE [DB_NAME]

SET IDENTITY_INSERT [dbo].[file_load] ON
INSERT INTO [dbo].[file_load]
           ([file_load_id]
		   ,[file_load_start]
           ,[file_load_end]
           ,[user_id]
           ,[files_to_process])

SELECT DISTINCT [file_load].[file_load_id]
      ,[file_load].[file_load_start]
      ,[file_load].[file_load_end]
      ,[file_load].[user_id]
      ,[file_load].[files_to_process]
FROM [indigo_database_group].[dbo].[file_load] [file_load]
	LEFT JOIN [indigo_database_group].[dbo].[file_history] [history]
		ON [file_load].[file_load_id] = [history].[file_load_id]
WHERE [history]. [issuer_id] = @selected_issuer_id
ORDER BY [file_load].[file_load_id] ASC

SET IDENTITY_INSERT [dbo].[file_load] OFF