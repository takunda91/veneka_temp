USE [indigo_database_main_timezone]
GO

UPDATE t
   SET t.[card_id] = s.[card_id]
      ,t.[branch_card_statuses_id] = s.[branch_card_statuses_id]
      ,t.[status_date] = s.[status_date]
      ,t.[user_id] = s.[user_id]
      ,t.[operator_user_id] = s.[operator_user_id]
      ,t.[branch_card_code_id] = s.[branch_card_code_id]
      ,t.[comments] = s.[comments]
      ,t.[pin_auth_user_id] = s.[pin_auth_user_id]
      ,t.[branch_id] = s.[branch_id]
FROM [dbo].[branch_card_status] as t 
		INNER JOIN [dbo].[branch_card_status_audit] as s ON t.card_id = s.card_id
 WHERE s.[branch_card_status_id] = 92125
GO

DELETE FROM [dbo].[branch_card_status_audit]
WHERE [branch_card_status_id] >= 92125


