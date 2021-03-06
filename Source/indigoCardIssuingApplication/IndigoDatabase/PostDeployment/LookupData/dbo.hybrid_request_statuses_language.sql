

MERGE INTO [dbo].[hybrid_request_statuses_language] AS trgt
USING	(VALUES
		(0,0,'CREATED'),
		(0,1,'CREATED_FR'),
		(0,2,'CREATED_PT'),
		(0,3,'CREATED_ES'),
		(1,0,'APPROVED'),
		(1,1,'APPROVED_FR'),
		(1,2,'APPROVED_PT'),
		(1,3,'APPROVED_ES'),
		(2,0,'ASSIGN_TO_BATCH'),
		(2,1,'ASSIGN_TO_BATCH_FR'),
		(2,2,'ASSIGN_TO_BATCH_PT'),
		(2,3,'ASSIGN_TO_BATCH_ES'),
		(3,0,'REJECTED'),
		(3,1,'REJECTED_FR'),
		(3,2,'REJECTED_PT'),
		(3,3,'REJECTED_ES')
		) AS src([hybrid_request_statuses_id],[language_id],[language_text])
ON
	trgt.[hybrid_request_statuses_id] = src.[hybrid_request_statuses_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[hybrid_request_statuses_id] = src.[hybrid_request_statuses_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([hybrid_request_statuses_id],[language_id],[language_text])
	VALUES ([hybrid_request_statuses_id],[language_id],[language_text])

;

