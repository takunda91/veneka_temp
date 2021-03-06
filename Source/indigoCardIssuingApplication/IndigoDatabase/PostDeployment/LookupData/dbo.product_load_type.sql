

MERGE INTO [dbo].[product_load_type] AS trgt
USING	(VALUES
		(0,'NO_LOAD'),
		(1,'LOAD_TO_PROD'),
		(2,'LOAD_TO_DIST'),
		(3,'LOAD_TO_CENTRE'),
		(4,'LOAD_TO_EXISTING'),
		(5,'LOAD_REQUESTS'),
		(6,'LOAD_REQUESTS_TO_DIST')
		) AS src([product_load_type_id],[product_load_type_name])
ON
	trgt.[product_load_type_id] = src.[product_load_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[product_load_type_id] = src.[product_load_type_id]
		, [product_load_type_name] = src.[product_load_type_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([product_load_type_id],[product_load_type_name])
	VALUES ([product_load_type_id],[product_load_type_name])

;

