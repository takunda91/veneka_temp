[{DATABASE_NAME}].[dbo].[product_fee_scheme]
SELECT [fee_scheme_id]
      ,[fee_scheme_name]
      ,[issuer_id]
      ,[deleted_yn]
	  ,[issuer_id] as [fee_accounting_id]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[product_fee_scheme]