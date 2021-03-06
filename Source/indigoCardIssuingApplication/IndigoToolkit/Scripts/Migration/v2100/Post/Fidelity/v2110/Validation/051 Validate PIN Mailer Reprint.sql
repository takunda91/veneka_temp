--Get records from target and check if there are any that arent in source
SELECT 
	  [card_id]
      ,[user_id]
      ,[pin_mailer_reprint_status_id]
	  ,[status_date]
	  ,[comments]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[pin_mailer_reprint] 
EXCEPT
SELECT [card_id], [user_id], [pin_mailer_reprint_status_id], CAST([status_date] as datetime) [status_date],[comments]
FROM
(SELECT [card_id]
      ,[user_id]
      ,[pin_mailer_reprint_status_id]
	  ,[status_date]
	  ,[comments]
FROM [{DATABASE_NAME}].[dbo].[pin_mailer_reprint]
UNION ALL
SELECT [card_id]
      ,[user_id]
      ,[pin_mailer_reprint_status_id]
	  ,[status_date]
	  ,[comments]
FROM [{DATABASE_NAME}].[dbo].[pin_mailer_reprint_audit]) as newDB