

MERGE INTO [dbo].[threed_secure_batch_statuses_language] AS trgt
USING	(VALUES
		(0,0,'CREATED'),
		(1,0,'REGISTERED'),
		(2,0,'FAILED'),
		(3,0,'RECREATED'),
		(0,1,'CREATED_fr'),
		(1,1,'REGISTERED_fr'),
		(2,1,'FAILED_fr'),
		(3,1,'RECREATED_fr'),
		(0,2,'CREATED_pt'),
		(1,2,'REGISTERED_pt'),
		(2,2,'FAILED_pt'),
		(3,2,'RECREATED_pt'),
		(0,3,'CREATED_es'),
		(1,3,'REGISTERED_es'),
		(2,3,'FAILED_es'),
		(3,3,'RECREATED_es')
		) AS src([threed_batch_statuses_id],[language_id],[language_text])
ON
	trgt.[threed_batch_statuses_id] = src.[threed_batch_statuses_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[threed_batch_statuses_id] = src.[threed_batch_statuses_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([threed_batch_statuses_id],[language_id],[language_text])
	VALUES ([threed_batch_statuses_id],[language_id],[language_text])

;

