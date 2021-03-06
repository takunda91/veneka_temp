

MERGE INTO [dbo].[pin_mailer_reprint_statuses_language] AS trgt
USING	(VALUES
		(0,0,N'REQUESTED'),
		(0,1,N'REQUESTED_fr'),
		(0,2,N'REQUESTED_pt'),
		(0,3,N'REQUESTED_es'),
		(1,0,N'APPROVED'),
		(1,1,N'APPROVED_fr'),
		(1,2,N'APPROVED_pt'),
		(1,3,N'APPROVED_es'),
		(2,0,N'PROCESSING'),
		(2,1,N'PROCESSING_fr'),
		(2,2,N'PROCESSING_pt'),
		(2,3,N'PROCESSING_es'),
		(3,0,N'COMPLETE'),
		(3,1,N'COMPLETE_fr'),
		(3,2,N'COMPLETE_pt'),
		(3,3,N'COMPLETE_es'),
		(4,0,N'REJECTED'),
		(4,1,N'REJECTED_fr'),
		(4,2,N'REJECTED_pt'),
		(4,3,N'REJECTED_es')
		) AS src([pin_mailer_reprint_status_id],[language_id],[language_text])
ON
	trgt.[pin_mailer_reprint_status_id] = src.[pin_mailer_reprint_status_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[pin_mailer_reprint_status_id] = src.[pin_mailer_reprint_status_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([pin_mailer_reprint_status_id],[language_id],[language_text])
	VALUES ([pin_mailer_reprint_status_id],[language_id],[language_text])

;

