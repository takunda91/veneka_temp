

MERGE INTO [dbo].[dist_batch_type] AS trgt
USING	(VALUES
		(0,'PRODUCTION'),
		(1,'DISTRIBUTION')
		) AS src([dist_batch_type_id],[dist_batch_type_name])
ON
	trgt.[dist_batch_type_id] = src.[dist_batch_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[dist_batch_type_id] = src.[dist_batch_type_id]
		, [dist_batch_type_name] = src.[dist_batch_type_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([dist_batch_type_id],[dist_batch_type_name])
	VALUES ([dist_batch_type_id],[dist_batch_type_name])

;

