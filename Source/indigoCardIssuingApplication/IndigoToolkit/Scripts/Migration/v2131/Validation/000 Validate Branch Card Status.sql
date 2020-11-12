--Get records from target and check if there are any that arent in source
SELECT [card_id], [branch_card_statuses_id], [status_date], [user_id], [operator_user_id], [branch_card_code_id]
      ,[comments],[pin_auth_user_id],[branch_id]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[branch_card_status]
EXCEPT
SELECT [card_id], [branch_card_statuses_id], CAST([status_date] as datetime) [status_date], [user_id], [operator_user_id], [branch_card_code_id]
      ,[comments],[pin_auth_user_id],[branch_id]
FROM
(SELECT [card_id], [branch_card_statuses_id], [status_date], [user_id], [operator_user_id], [branch_card_code_id]
      ,[comments],[pin_auth_user_id],[branch_id]
FROM [{DATABASE_NAME}].[dbo].[branch_card_status]
UNION ALL
SELECT [card_id], [branch_card_statuses_id], [status_date], [user_id], [operator_user_id], [branch_card_code_id]
      ,[comments],[pin_auth_user_id],[branch_id]
FROM [{DATABASE_NAME}].[dbo].[branch_card_status_audit]) as newDB