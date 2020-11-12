--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[product_fee_scheme]
EXCEPT
SELECT [fee_scheme_id],[issuer_id],[fee_scheme_name],[deleted_yn]
FROM [{DATABASE_NAME}].[dbo].[product_fee_scheme]