--Get records from target and check if there are any that arent in source
SELECT [load_batch_id]
      ,[load_batch_statuses_id]
      ,[user_id]
      ,[status_date]
      ,[status_notes]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[load_batch_status]
EXCEPT
SELECT [load_batch_id]
      ,[load_batch_statuses_id]
      ,[user_id]
      ,CAST([status_date] as datetime) [status_date]
      ,[status_notes]
FROM
(SELECT [load_batch_id]
      ,[load_batch_statuses_id]
      ,[user_id]
      ,[status_date]
      ,[status_notes]
FROM [{DATABASE_NAME}].[dbo].[load_batch_status]
UNION ALL
SELECT [load_batch_id]
      ,[load_batch_statuses_id]
      ,[user_id]
      ,[status_date]
      ,[status_notes]
FROM [{DATABASE_NAME}].[dbo].[load_batch_status_audit]) as newDB