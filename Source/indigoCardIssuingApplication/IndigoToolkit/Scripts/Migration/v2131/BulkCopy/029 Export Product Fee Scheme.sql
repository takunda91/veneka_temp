[{DATABASE_NAME}].[dbo].[product_fee_scheme]
SELECT [fee_scheme_id]      
      ,[issuer_id]
	  ,[fee_scheme_name]
      ,[deleted_yn]
	  ,[fee_accounting_id]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[product_fee_scheme] 

