--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[audit_control]
EXCEPT
SELECT [audit_id]
      ,[audit_action_id]
      ,[user_id]
      ,CAST([audit_date] as datetime) [audit_date]
      ,[workstation_address]
      ,[action_description]
      ,[issuer_id]
      ,[data_changed]
      ,[data_before]
      ,[data_after]
FROM [{DATABASE_NAME}].[dbo].[audit_control]