[{DATABASE_NAME}].[dbo].[product_fee_detail]
SELECT [fee_scheme_id]
      ,[fee_detail_id]
      ,[effective_from]
      ,[fee_waiver_YN]
      ,[fee_editable_YN]
      ,[effective_to]
      ,[deleted_yn]
      ,[fee_detail_name]	  
FROM [{SOURCE_DATABASE_NAME}].[dbo].[product_fee_detail]