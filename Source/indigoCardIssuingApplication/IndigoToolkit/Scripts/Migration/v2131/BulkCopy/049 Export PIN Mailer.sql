[{DATABASE_NAME}].[dbo].[pin_mailer]
SELECT [pin_mailer_reference]
      ,[batch_reference]
      ,[pin_mailer_status]
      ,[card_number]
      ,[pvv_offset]
      ,[encrypted_pin]
      ,[customer_name]
      ,[customer_address]
      ,ToDateTimeOffset([printed_date], DATENAME(tz, SYSDATETIMEOFFSET())) as [printed_date]
      ,ToDateTimeOffset([reprinted_date], DATENAME(tz, SYSDATETIMEOFFSET())) as [reprinted_date]
      ,[reprint_request_YN]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[pin_mailer]