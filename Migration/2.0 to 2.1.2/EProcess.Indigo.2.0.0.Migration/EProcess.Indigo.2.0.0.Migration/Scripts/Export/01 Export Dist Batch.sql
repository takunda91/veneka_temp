USE [DB_NAME]

SET IDENTITY_INSERT [dbo].[dist_batch] ON

INSERT INTO [dbo].[dist_batch]
           ([dist_batch_id]
		   ,[branch_id]
           ,[no_cards]
           ,[date_created]
           ,[dist_batch_reference]
           ,[card_issue_method_id]
           ,[dist_batch_type_id]
           ,[issuer_id])	

SELECT [dist_batch_id]
		  ,[d_batch].[branch_id]
		  ,[d_batch].[no_cards]
		  ,[d_batch].[date_created]
		  ,[d_batch].[dist_batch_reference]
		  ,1
		  ,1
		  ,[branch].[issuer_id]
	FROM [indigo_database_group].[dbo].[dist_batch] [d_batch]
		LEFT JOIN [indigo_database_group].[dbo].[branch] [branch]
			ON [d_batch].[branch_id] = [branch].[branch_id]
	WHERE [branch].[issuer_id] = @selected_issuer_id
	ORDER BY [dist_batch_id] ASC

SET IDENTITY_INSERT [dbo].[dist_batch] OFF