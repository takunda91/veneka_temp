[{DATABASE_NAME}].[dbo].[audit_control]
SELECT [audit_id]
    ,[audit_action_id]
    ,[user_id]
    ,[audit_date]
    ,[workstation_address]
    ,[action_description]
    ,[issuer_id]
    ,[data_changed]
    ,[data_before]
    ,[data_after]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[audit_control]