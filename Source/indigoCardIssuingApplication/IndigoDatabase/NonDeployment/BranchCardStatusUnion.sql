SELECT 0 as[branch_card_status_id]
      ,[card_id]
      ,[branch_card_statuses_id]
      ,[status_date]
      ,[user_id]
      ,[operator_user_id]
      ,[branch_card_code_id]
      ,[comments]
      ,[pin_auth_user_id]
      ,[branch_id]
	  ,'current' as t
  FROM [indigo_database_main_timezone].[dbo].[branch_card_status]
  where card_id in (97791)
UNION ALL
  SELECT [branch_card_status_id]
      ,[card_id]
      ,[branch_card_statuses_id]
      ,[status_date]
      ,[user_id]
      ,[operator_user_id]
      ,[branch_card_code_id]
      ,[comments]
      ,[pin_auth_user_id]
      ,[branch_id]
	  ,'audit' as t
  FROM [indigo_database_main_timezone].[dbo].[branch_card_status_audit]
  where card_id in (97791)
  ORDER BY card_id, status_date
