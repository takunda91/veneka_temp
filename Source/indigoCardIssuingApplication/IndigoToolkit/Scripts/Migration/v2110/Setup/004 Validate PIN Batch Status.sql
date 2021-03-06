--Get records from target and check if there are any that arent in source
SELECT [pin_batch_id]
      ,[pin_batch_statuses_id]
      ,[user_id]
      ,[status_date]
      ,[status_notes]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[pin_batch_status]
EXCEPT
SELECT [pin_batch_id]
      ,[pin_batch_statuses_id]
      ,[user_id]
      ,CAST([status_date] as datetime) [status_date]
      ,[status_notes]
FROM
(SELECT [pin_batch_id]
      ,[pin_batch_statuses_id]
      ,[user_id]
      ,[status_date]
      ,[status_notes]
FROM [{DATABASE_NAME}].[dbo].[pin_batch_status]
UNION ALL
SELECT [pin_batch_id]
      ,[pin_batch_statuses_id]
      ,[user_id]
      ,[status_date]
      ,[status_notes]
FROM [{DATABASE_NAME}].[dbo].[pin_batch_status_audit]) as newDB