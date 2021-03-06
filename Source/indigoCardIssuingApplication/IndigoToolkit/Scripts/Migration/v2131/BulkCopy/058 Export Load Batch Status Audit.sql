[{DATABASE_NAME}].[dbo].[load_batch_status_audit]
SELECT [load_batch_status_id]
		,[load_batch_id]
		,[load_batch_statuses_id]
		,[user_id]
		,ToDateTimeOffset([status_date], DATENAME(tz, SYSDATETIMEOFFSET())) as [status_date]
		,[status_notes]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[load_batch_status]
WHERE [load_batch_status_id] NOT IN (SELECT [load_batch_status_id] FROM [{SOURCE_DATABASE_NAME}].[dbo].[load_batch_status_current])