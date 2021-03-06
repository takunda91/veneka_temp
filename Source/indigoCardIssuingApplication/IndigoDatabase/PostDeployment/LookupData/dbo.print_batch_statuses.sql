

MERGE INTO [dbo].[print_batch_statuses] AS trgt
USING	(VALUES
		(0,'CREATED'),
		(1,'APPROVED'),
		(2,'SENT_TO_PRINTER'),
		(3,'PRINT_SUCESSFUL'),
		(4,'PRINT_FAILED'),
		(5,'REJECTED'),
		(6,'PROCESSED_IN_CMS'),
		(7,'CMS_ERROR'),
		(8,'SPOIL')
		) AS src([print_batch_statuses_id],[print_batch_statuses])
ON
	trgt.[print_batch_statuses_id] = src.[print_batch_statuses_id]
WHEN MATCHED THEN
	UPDATE SET
		[print_batch_statuses_id] = src.[print_batch_statuses_id]
		, [print_batch_statuses] = src.[print_batch_statuses]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([print_batch_statuses_id],[print_batch_statuses])
	VALUES ([print_batch_statuses_id],[print_batch_statuses])

;

