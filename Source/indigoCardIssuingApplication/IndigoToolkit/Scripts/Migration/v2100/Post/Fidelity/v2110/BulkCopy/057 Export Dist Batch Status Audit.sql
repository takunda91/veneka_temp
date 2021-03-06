[{DATABASE_NAME}].[dbo].[dist_batch_status_audit]
SELECT [dist_batch_status_id]
	  ,[dist_batch_id]
      ,[dist_batch_statuses_id]
      ,[user_id]
      ,ToDateTimeOffset([status_date], DATENAME(tz, SYSDATETIMEOFFSET())) as [status_date] 
      ,[status_notes]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[dist_batch_status] 
WHERE [dist_batch_status_id] NOT IN (SELECT [dist_batch_status_id] FROM [{SOURCE_DATABASE_NAME}].[dbo].[dist_batch_status_current])