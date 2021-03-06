

MERGE INTO [dbo].[print_batch_request_statuses_language] AS trgt
USING	(VALUES
		(1,0,'APPROVED'),
		(1,1,'APPROVED_FR'),
		(1,2,'APPROVED_ES'),
		(1,3,'APPROVED_PT'),
		(2,0,'SENT_TO_PRINT'),
		(2,1,'SENT_TO_PRINT_FR'),
		(2,2,'SENT_TO_PRINT_ES'),
		(2,3,'SENT_TO_PRINT_PT'),
		(3,0,'PRINT_SUCESSFUL'),
		(3,1,'PRINT_SUCESSFUL_FR'),
		(3,2,'PRINT_SUCESSFUL_ES'),
		(3,3,'PRINT_SUCESSFUL_PT'),
		(4,0,'PRINT_FAILED'),
		(4,1,'PRINT_FAILED_FR'),
		(4,2,'PRINT_FAILED_ES'),
		(4,3,'PRINT_FAILED_PT'),
		(5,0,'CMS_ERROR'),
		(5,1,'CMS_ERROR_FR'),
		(5,2,'CMS_ERROR_ES'),
		(5,3,'CMS_ERROR_PT')
		) AS src([print_batch_request_status_id],[language_id],[language_text])
ON
	trgt.[print_batch_request_status_id] = src.[print_batch_request_status_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[print_batch_request_status_id] = src.[print_batch_request_status_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([print_batch_request_status_id],[language_id],[language_text])
	VALUES ([print_batch_request_status_id],[language_id],[language_text])

;

