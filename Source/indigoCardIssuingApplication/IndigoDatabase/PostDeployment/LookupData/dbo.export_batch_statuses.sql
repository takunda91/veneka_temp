

MERGE INTO [dbo].[export_batch_statuses] AS trgt
USING	(VALUES
		(0,'CREATED'),
		(1,'APPROVED'),
		(2,'EXPORTED'),
		(3,'REQUEST_EXPORT'),
		(4,'REJECTED')
		) AS src([export_batch_statuses_id],[export_batch_statuses_name])
ON
	trgt.[export_batch_statuses_id] = src.[export_batch_statuses_id]
WHEN MATCHED THEN
	UPDATE SET
		[export_batch_statuses_id] = src.[export_batch_statuses_id]
		, [export_batch_statuses_name] = src.[export_batch_statuses_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([export_batch_statuses_id],[export_batch_statuses_name])
	VALUES ([export_batch_statuses_id],[export_batch_statuses_name])

;

