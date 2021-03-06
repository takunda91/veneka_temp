--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[pin_batch]
EXCEPT
SELECT [pin_batch_id]
      ,[no_cards]
      ,CAST([date_created] as datetime) [date_created]
      ,[pin_batch_reference]
      ,[pin_batch_type_id]
      ,[card_issue_method_id]
      ,[issuer_id]
      ,[branch_id]
FROM [{DATABASE_NAME}].[dbo].[pin_batch]