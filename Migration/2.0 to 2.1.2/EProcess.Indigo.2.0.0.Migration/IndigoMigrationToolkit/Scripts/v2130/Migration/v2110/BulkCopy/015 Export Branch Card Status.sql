[{DATABASE_NAME}].[dbo].[branch_card_status]
SELECT [branch_card_status_id]
		,[{SOURCE_DATABASE_NAME}].[dbo].[branch_card_status].[card_id]
		,[branch_card_statuses_id]
		,[status_date]
		,[user_id]
		,[operator_user_id]
		,[branch_card_code_id]
		,[comments]
		,[pin_auth_user_id]
		,[branch_id]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[branch_card_status]
	INNER JOIN [{SOURCE_DATABASE_NAME}].[dbo].[cards]
		ON [{SOURCE_DATABASE_NAME}].[dbo].[branch_card_status].[card_id] = [{SOURCE_DATABASE_NAME}].[dbo].[cards].[card_id]