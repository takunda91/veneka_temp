USE [DB_NAME]

SET IDENTITY_INSERT[dbo].[load_batch] ON
INSERT INTO [dbo].[load_batch]
           ([load_batch_id]
		   ,[file_id]
           ,[load_batch_status_id]
           ,[no_cards]
           ,[load_date]
           ,[load_batch_reference]
           ,[load_batch_type_id])


SELECT [load_batch].[load_batch_id]
      ,[load_batch].[file_id]
      ,[load_batch].[load_batch_status_id]
      ,[load_batch].[no_cards]
      ,[load_batch].[load_date]
      ,[load_batch].[load_batch_reference]
	  ,1
FROM [indigo_database_group].[dbo].[load_batch] [load_batch]
	LEFT JOIN [indigo_database_group].[dbo].[file_history] [history]
		ON [load_batch].[file_id] = [history].[file_id]
WHERE [history]. [issuer_id] = @selected_issuer_id