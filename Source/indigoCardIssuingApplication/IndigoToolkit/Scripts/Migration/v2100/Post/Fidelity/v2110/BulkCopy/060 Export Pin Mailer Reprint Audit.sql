[{DATABASE_NAME}].[dbo].[pin_mailer_reprint_audit]
SELECT [pin_mailer_reprint_id]
      ,[card_id]
      ,[user_id]
      ,[pin_mailer_reprint_status_id]
      ,ToDateTimeOffset([status_date], DATENAME(tz, SYSDATETIMEOFFSET())) as [status_date]
      ,[comments]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[pin_mailer_reprint]
WHERE [pin_mailer_reprint_id] NOT IN (SELECT [pin_mailer_reprint_id] FROM [{SOURCE_DATABASE_NAME}].[dbo].[pin_mailer_reprint_status_current])