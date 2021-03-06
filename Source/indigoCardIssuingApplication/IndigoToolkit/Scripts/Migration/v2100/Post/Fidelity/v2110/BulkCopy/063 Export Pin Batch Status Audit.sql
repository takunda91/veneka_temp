[{DATABASE_NAME}].[dbo].[pin_batch_status_audit]
SELECT [pin_batch_status_id]
      ,[pin_batch_id]
      ,[pin_batch_statuses_id]
      ,[user_id]
      ,ToDateTimeOffset([status_date], DATENAME(tz, SYSDATETIMEOFFSET())) as [status_date]
      ,[status_notes]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[pin_batch_status]
WHERE [pin_batch_status_id] NOT IN (SELECT [pin_batch_status_id] FROM [{SOURCE_DATABASE_NAME}].[dbo].[pin_batch_status_current])