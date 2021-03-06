

MERGE INTO [dbo].[pin_reissue_statuses_language] AS trgt
USING	(VALUES
		(0,0,'REQUESTED'),
		(0,1,'REQUESTED_fr'),
		(0,2,'REQUESTED_pt'),
		(0,3,'REQUESTED_es'),
		(1,0,'APPROVED'),
		(1,1,'APPROVED_fr'),
		(1,2,'APPROVED_pt'),
		(1,3,'APPROVED_es'),
		(2,0,'REJECTED'),
		(2,1,'REJECTED_fr'),
		(2,2,'REJECTED_pt'),
		(2,3,'REJECTED_es'),
		(3,0,'UPLOADED'),
		(3,1,'UPLOADED_fr'),
		(3,2,'UPLOADED_pt'),
		(3,3,'UPLOADED_es'),
		(4,0,'EXPIRED'),
		(4,1,'EXPIRED_fr'),
		(4,2,'EXPIRED_pt'),
		(4,3,'EXPIRED_es'),
		(5,0,'CANCEL'),
		(5,1,'CANCEL_fr'),
		(5,2,'CANCEL_pt'),
		(5,3,'CANCEL_es')
		) AS src([pin_reissue_statuses_id],[language_id],[language_text])
ON
	trgt.[pin_reissue_statuses_id] = src.[pin_reissue_statuses_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[pin_reissue_statuses_id] = src.[pin_reissue_statuses_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([pin_reissue_statuses_id],[language_id],[language_text])
	VALUES ([pin_reissue_statuses_id],[language_id],[language_text])

;

