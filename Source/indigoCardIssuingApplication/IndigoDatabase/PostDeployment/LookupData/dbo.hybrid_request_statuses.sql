

MERGE INTO [dbo].[hybrid_request_statuses] AS trgt
USING	(VALUES
		(0,'CREATED'),
		(1,'APPROVED'),
		(2,'ASSIGN_TO_BATCH'),
		(3,'REJECTED')
		) AS src([hybrid_request_statuses_id],[hybrid_request_statuses])
ON
	trgt.[hybrid_request_statuses_id] = src.[hybrid_request_statuses_id]
WHEN MATCHED THEN
	UPDATE SET
		[hybrid_request_statuses_id] = src.[hybrid_request_statuses_id]
		, [hybrid_request_statuses] = src.[hybrid_request_statuses]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([hybrid_request_statuses_id],[hybrid_request_statuses])
	VALUES ([hybrid_request_statuses_id],[hybrid_request_statuses])

;

