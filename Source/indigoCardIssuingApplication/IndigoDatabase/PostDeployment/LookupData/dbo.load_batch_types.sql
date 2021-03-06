

MERGE INTO [dbo].[load_batch_types] AS trgt
USING	(VALUES
		(1,N'CARD FILE'),
		(2,N'CARD REQUEST FILE')
		) AS src([load_batch_type_id],[load_batch_type])
ON
	trgt.[load_batch_type_id] = src.[load_batch_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[load_batch_type_id] = src.[load_batch_type_id]
		, [load_batch_type] = src.[load_batch_type]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([load_batch_type_id],[load_batch_type])
	VALUES ([load_batch_type_id],[load_batch_type])

;

