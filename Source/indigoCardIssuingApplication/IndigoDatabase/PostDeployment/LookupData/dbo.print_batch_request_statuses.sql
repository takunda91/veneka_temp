

MERGE INTO [dbo].[print_batch_request_statuses] AS trgt
USING	(VALUES
		(0,'CREATED'),
		(1,'APPROVED'),
		(2,'SENT_TO_PRINT'),
		(3,'PRINT_SUCESSFUL'),
		(4,'PRINT_FAILED'),
		(5,'CMS_ERROR')
		) AS src([print_batch_request_status_id],[print_batch_request_status])
ON
	trgt.[print_batch_request_status_id] = src.[print_batch_request_status_id]
WHEN MATCHED THEN
	UPDATE SET
		[print_batch_request_status_id] = src.[print_batch_request_status_id]
		, [print_batch_request_status] = src.[print_batch_request_status]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([print_batch_request_status_id],[print_batch_request_status])
	VALUES ([print_batch_request_status_id],[print_batch_request_status])

;

