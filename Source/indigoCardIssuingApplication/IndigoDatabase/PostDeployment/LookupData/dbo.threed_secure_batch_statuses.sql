

MERGE INTO [dbo].[threed_secure_batch_statuses] AS trgt
USING	(VALUES
		(0,N'CREATED'),
		(1,N'REGISTERED'),
		(2,N'FAILED'),
		(3,N'RECREATED')
		) AS src([threed_batch_statuses_id],[threed_batch_statuses_name])
ON
	trgt.[threed_batch_statuses_id] = src.[threed_batch_statuses_id]
WHEN MATCHED THEN
	UPDATE SET
		[threed_batch_statuses_id] = src.[threed_batch_statuses_id]
		, [threed_batch_statuses_name] = src.[threed_batch_statuses_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([threed_batch_statuses_id],[threed_batch_statuses_name])
	VALUES ([threed_batch_statuses_id],[threed_batch_statuses_name])

;

