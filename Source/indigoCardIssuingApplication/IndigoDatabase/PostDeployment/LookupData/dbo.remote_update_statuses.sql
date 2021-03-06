

MERGE INTO [dbo].[remote_update_statuses] AS trgt
USING	(VALUES
		(0,'PENDING'),
		(1,'SENT'),
		(2,'COMPLETE'),
		(3,'RESEND'),
		(4,'FAILED')
		) AS src([remote_update_statuses_id],[remote_update_statuses_name])
ON
	trgt.[remote_update_statuses_id] = src.[remote_update_statuses_id]
WHEN MATCHED THEN
	UPDATE SET
		[remote_update_statuses_id] = src.[remote_update_statuses_id]
		, [remote_update_statuses_name] = src.[remote_update_statuses_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([remote_update_statuses_id],[remote_update_statuses_name])
	VALUES ([remote_update_statuses_id],[remote_update_statuses_name])

;

