

MERGE INTO [dbo].[pin_batch_type] AS trgt
USING	(VALUES
		(0,'PRODUCTION'),
		(1,'DISTRIBUTION'),
		(2,'REPRINT')
		) AS src([pin_batch_type_id],[pin_batch_type_name])
ON
	trgt.[pin_batch_type_id] = src.[pin_batch_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[pin_batch_type_id] = src.[pin_batch_type_id]
		, [pin_batch_type_name] = src.[pin_batch_type_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([pin_batch_type_id],[pin_batch_type_name])
	VALUES ([pin_batch_type_id],[pin_batch_type_name])

;

