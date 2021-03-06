USE [DB_NAME]


SET IDENTITY_INSERT [dbo].[dist_batch_status] ON
INSERT INTO [dbo].[dist_batch_status]
           ([dist_batch_status_id]
		   ,[dist_batch_id]
           ,[dist_batch_statuses_id]
           ,[user_id]
           ,[status_date]
           ,[status_notes])

SELECT DISTINCT [d_batch_status].[dist_batch_status_id]
      ,[d_batch_status].[dist_batch_id]
      ,[d_batch_status].[dist_batch_statuses_id]
      ,[d_batch_status].[user_id]
      ,[d_batch_status].[status_date]
      ,[d_batch_status].[status_notes]
FROM [indigo_database_group].[dbo].[dist_batch_status] [d_batch_status]
	LEFT JOIN [indigo_database_group].[dbo].[dist_batch_cards] [d_batch_cards]
		ON [d_batch_status].[dist_batch_id] = [d_batch_cards].[dist_batch_id]
	
	LEFT JOIN [indigo_database_group].[dbo].[cards] [cards]
		ON [d_batch_cards].[card_id] = [cards].[card_id]
	
	LEFT JOIN [indigo_database_group].[dbo].[issuer_product] [product]
		ON [cards].[product_id] = [product].[product_id]

WHERE [product].[issuer_id] = @selected_issuer_id
ORDER BY [dist_batch_status_id] ASC

SET IDENTITY_INSERT [dbo].[dist_batch_status] OFF