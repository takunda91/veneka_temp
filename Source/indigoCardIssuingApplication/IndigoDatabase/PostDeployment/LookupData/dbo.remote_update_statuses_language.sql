

MERGE INTO [dbo].[remote_update_statuses_language] AS trgt
USING	(VALUES
		(0,0,N'PENDING'),
		(0,1,N'PENDING_fr'),
		(0,2,N'PENDING_pt'),
		(0,3,N'PENDING_es'),
		(1,0,N'SENT'),
		(1,1,N'SENT_fr'),
		(1,2,N'SENT_pt'),
		(1,3,N'SENT_es'),
		(2,0,N'COMPLETE'),
		(2,1,N'COMPLETE_fr'),
		(2,2,N'COMPLETE_pt'),
		(2,3,N'COMPLETE_es'),
		(3,0,N'RESEND'),
		(3,1,N'RESEND_fr'),
		(3,2,N'RESEND_pt'),
		(3,3,N'RESEND_es'),
		(4,0,N'FAILED'),
		(4,1,N'FAILED_fr'),
		(4,2,N'FAILED_pt'),
		(4,3,N'FAILED_es')
		) AS src([remote_update_statuses_id],[language_id],[language_text])
ON
	trgt.[remote_update_statuses_id] = src.[remote_update_statuses_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[remote_update_statuses_id] = src.[remote_update_statuses_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([remote_update_statuses_id],[language_id],[language_text])
	VALUES ([remote_update_statuses_id],[language_id],[language_text])

;

