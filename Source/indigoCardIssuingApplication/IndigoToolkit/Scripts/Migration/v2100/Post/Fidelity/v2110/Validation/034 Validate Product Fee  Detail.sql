--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[product_fee_detail]
EXCEPT
SELECT [fee_scheme_id]
      ,[fee_detail_id]
      ,[fee_detail_name]
      ,CAST([effective_from] as datetime) [effective_from]
      ,[fee_waiver_YN]
      ,[fee_editable_YN]
      ,[deleted_yn]
      ,CAST([effective_to] as datetime) [effective_to]
FROM [{DATABASE_NAME}].[dbo].[product_fee_detail]