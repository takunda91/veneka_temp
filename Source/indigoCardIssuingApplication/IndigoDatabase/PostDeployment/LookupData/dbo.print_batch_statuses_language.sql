

MERGE INTO [dbo].[print_batch_statuses_language] AS trgt
USING	(VALUES
		(0,0,'CREATED'),
		(1,0,'APPROVED'),
		(2,0,'SENT_TO_PRINT'),
		(3,0,'PRINT_SUCESSFUL'),
		(4,0,'PRINT_FAILED'),
		(5,0,'REJECTED'),
		(6,0,'PROCESSED_IN_CMS'),
		(7,0,'CMS_ERROR'),
		(8,0,'SPOIL'),
		(0,1,'CREATED_fr'),
		(1,1,'APPROVED_fr'),
		(2,1,'SENT_TO_PRINT_fr'),
		(3,1,'PRINT_SUCESSFUL_fr'),
		(4,1,'PRINT_FAILED_fr'),
		(5,1,'REJECTED_fr'),
		(6,1,'PROCESSED_IN_CMS_fr'),
		(7,1,'CMS_ERROR_fr'),
		(8,1,'SPOIL_fr'),
		(0,2,'CREATED_pt'),
		(1,2,'APPROVED_pt'),
		(2,2,'SENT_TO_PRINT_pt'),
		(3,2,'PRINT_SUCESSFUL_pt'),
		(4,2,'PRINT_FAILED_pt'),
		(5,2,'REJECTED_pt'),
		(6,2,'PROCESSED_IN_CMS_pt'),
		(7,2,'CMS_ERROR_pt'),
		(8,2,'SPOIL_pt'),
		(0,3,'CREATED_es'),
		(1,3,'APPROVED_es'),
		(2,3,'SENT_TO_PRINT_es'),
		(3,3,'PRINT_SUCESSFUL_es'),
		(4,3,'PRINT_FAILED_es'),
		(5,3,'REJECTED_es'),
		(6,3,'PROCESSED_IN_CMS_es'),
		(7,3,'CMS_ERROR_es'),
		(8,3,'SPOIL_es')
		) AS src([print_batch_statuses_id],[language_id],[language_text])
ON
	trgt.[print_batch_statuses_id] = src.[print_batch_statuses_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[print_batch_statuses_id] = src.[print_batch_statuses_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([print_batch_statuses_id],[language_id],[language_text])
	VALUES ([print_batch_statuses_id],[language_id],[language_text])

;

