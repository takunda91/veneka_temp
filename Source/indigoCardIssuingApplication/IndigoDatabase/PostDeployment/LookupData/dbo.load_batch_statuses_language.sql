

MERGE INTO [dbo].[load_batch_statuses_language] AS trgt
USING	(VALUES
		(0,0,'LOADED'),
		(0,1,'CHARGÉ'),
		(0,2,'LOADED_pt'),
		(0,3,'LOADED_sp'),
		(1,0,'APPROVED'),
		(1,1,'APPROUVÉ'),
		(1,2,'APPROVED_pt'),
		(1,3,'APPROVED_sp'),
		(2,0,'REJECTED'),
		(2,1,'REJETÉ'),
		(2,2,'REJECTED_pt'),
		(2,3,'REJECTED_sp'),
		(3,0,'INVALID'),
		(3,1,'INVALID'),
		(3,2,'INVALID_pt'),
		(3,3,'INVALID_sp')
		) AS src([load_batch_statuses_id],[language_id],[language_text])
ON
	trgt.[load_batch_statuses_id] = src.[load_batch_statuses_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[load_batch_statuses_id] = src.[load_batch_statuses_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([load_batch_statuses_id],[language_id],[language_text])
	VALUES ([load_batch_statuses_id],[language_id],[language_text])

;

