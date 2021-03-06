--Get records from target and check if there are any that arent in source
SELECT [export_batch_id]
      ,[export_batch_statuses_id]
      ,[user_id]
      ,[status_date]
      ,[comments]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[export_batch_status]
EXCEPT
SELECT [export_batch_id]
      ,[export_batch_statuses_id]
      ,[user_id]
      ,CAST([status_date] as datetime2) [status_date]
      ,[comments]
FROM
(SELECT [export_batch_id]
      ,[export_batch_statuses_id]
      ,[user_id]
      ,[status_date]
      ,[comments]
FROM [{DATABASE_NAME}].[dbo].[export_batch_status]
UNION ALL
SELECT [export_batch_id]
      ,[export_batch_statuses_id]
      ,[user_id]
      ,[status_date]
      ,[comments]
FROM [{DATABASE_NAME}].[dbo].[export_batch_status_audit]) as newDB