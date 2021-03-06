--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[dist_batch]
EXCEPT
SELECT [dist_batch_id]
      ,[branch_id]
      ,[no_cards]
      ,CAST([date_created] as datetime) [date_created]
      ,[dist_batch_reference]
      ,[card_issue_method_id]
      ,[dist_batch_type_id]
      ,[issuer_id]      
FROM [{DATABASE_NAME}].[dbo].[dist_batch]