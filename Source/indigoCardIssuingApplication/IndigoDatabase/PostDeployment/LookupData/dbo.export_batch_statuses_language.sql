

MERGE INTO [dbo].[export_batch_statuses_language] AS trgt
USING	(VALUES
		(0,0,'CREATED'),
		(0,1,'CREATED_fr'),
		(0,2,'CREATED_pt'),
		(0,3,'CREATED_es'),
		(1,0,'APPROVED'),
		(1,1,'APPROVED_fr'),
		(1,2,'APPROVED_pt'),
		(1,3,'APPROVED_es'),
		(2,0,'EXPORTED'),
		(2,1,'EXPORTED_fr'),
		(2,2,'EXPORTED_pt'),
		(2,3,'EXPORTED_es'),
		(3,0,'REQUEST_EXPORT'),
		(3,1,'REQUEST_EXPORT_fr'),
		(3,2,'REQUEST_EXPORT_pt'),
		(3,3,'REQUEST_EXPORT_es'),
		(4,0,'REJECTED'),
		(4,1,'REJECTED_fr'),
		(4,2,'REJECTED_pt'),
		(4,3,'REJECTED_es')
		) AS src([export_batch_statuses_id],[language_id],[language_text])
ON
	trgt.[export_batch_statuses_id] = src.[export_batch_statuses_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[export_batch_statuses_id] = src.[export_batch_statuses_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([export_batch_statuses_id],[language_id],[language_text])
	VALUES ([export_batch_statuses_id],[language_id],[language_text])

;

