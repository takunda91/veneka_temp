--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[product_fee_charge]
EXCEPT
SELECT [fee_detail_id]
      ,[currency_id]
      ,[card_issue_reason_id]
      ,[fee_charge]
      ,CAST([date_created] as datetime) [date_created]
      ,[vat]
FROM [{DATABASE_NAME}].[dbo].[product_fee_charge]