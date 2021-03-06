[{DATABASE_NAME}].[dbo].[export_batch_status_audit]
SELECT [export_batch_status_id]
      ,[export_batch_id]
      ,[export_batch_statuses_id]
      ,[user_id]
      ,ToDateTimeOffset([status_date], DATENAME(tz, SYSDATETIMEOFFSET())) as [status_date]
      ,[comments]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[export_batch_status]
WHERE [export_batch_status_id] NOT IN (SELECT [export_batch_status_id] FROM [{SOURCE_DATABASE_NAME}].[dbo].[export_batch_status_current])