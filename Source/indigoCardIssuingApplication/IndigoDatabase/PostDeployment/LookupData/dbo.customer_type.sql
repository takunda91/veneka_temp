

MERGE INTO [dbo].[customer_type] AS trgt
USING	(VALUES
		(0,'PRIVATE'),
		(1,'CORPORATE')
		) AS src([customer_type_id],[customer_type_name])
ON
	trgt.[customer_type_id] = src.[customer_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[customer_type_id] = src.[customer_type_id]
		, [customer_type_name] = src.[customer_type_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([customer_type_id],[customer_type_name])
	VALUES ([customer_type_id],[customer_type_name])

;

