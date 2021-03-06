

MERGE INTO [dbo].[load_batch_statuses] AS trgt
USING	(VALUES
		(0,'LOADED'),
		(1,'APPROVED'),
		(2,'REJECTED'),
		(3,'INVALID')
		) AS src([load_batch_statuses_id],[load_batch_status_name])
ON
	trgt.[load_batch_statuses_id] = src.[load_batch_statuses_id]
WHEN MATCHED THEN
	UPDATE SET
		[load_batch_statuses_id] = src.[load_batch_statuses_id]
		, [load_batch_status_name] = src.[load_batch_status_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([load_batch_statuses_id],[load_batch_status_name])
	VALUES ([load_batch_statuses_id],[load_batch_status_name])

;

