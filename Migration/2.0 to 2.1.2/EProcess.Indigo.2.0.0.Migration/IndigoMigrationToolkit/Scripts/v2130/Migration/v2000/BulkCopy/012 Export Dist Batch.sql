[{DATABASE_NAME}].[dbo].[dist_batch]
SELECT [dist_batch_id]
		,[d_batch].[branch_id]
		,[d_batch].[no_cards]
		,[d_batch].[date_created]
		,[d_batch].[dist_batch_reference]
		,1 as [card_issue_method_id]
		,1 as [dist_batch_type_id]
		,[branch].[issuer_id]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[dist_batch] [d_batch]
	INNER JOIN [{SOURCE_DATABASE_NAME}].[dbo].[branch] [branch]
		ON [d_batch].[branch_id] = [branch].[branch_id]