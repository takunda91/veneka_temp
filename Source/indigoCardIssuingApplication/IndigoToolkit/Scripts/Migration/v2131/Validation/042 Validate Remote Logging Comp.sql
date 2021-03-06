--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[remote_component_logging]
EXCEPT
SELECT CAST([date_logged] as datetime2) [date_logged]
      ,[remote_address]
      ,[token]
      ,[request]
      ,[method_call_id]
FROM [{DATABASE_NAME}].[dbo].[remote_component_logging]