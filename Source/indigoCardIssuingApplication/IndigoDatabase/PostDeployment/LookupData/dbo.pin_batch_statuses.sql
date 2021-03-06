

MERGE INTO [dbo].[pin_batch_statuses] AS trgt
USING	(VALUES
		(0,'CREATED'),
		(1,'DISPATCHED_TO_CC'),
		(2,'RECIEVED_AT_CC'),
		(3,'DISPATCHED_TO_BRANCH'),
		(4,'RECIEVED_AT_BRANCH'),
		(5,'REJECTED_AT_CC'),
		(6,'REJECTED_AT_BRANCH'),
		(7,'SENT_TO_PRINTER'),
		(8,'PIN_PRINTED'),
		(9,'SENT_TO_CMS'),
		(10,'PROCESSED_IN_CMS'),
		(11,'APPROVED')
		) AS src([pin_batch_statuses_id],[pin_batch_statuses_name])
ON
	trgt.[pin_batch_statuses_id] = src.[pin_batch_statuses_id]
WHEN MATCHED THEN
	UPDATE SET
		[pin_batch_statuses_id] = src.[pin_batch_statuses_id]
		, [pin_batch_statuses_name] = src.[pin_batch_statuses_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([pin_batch_statuses_id],[pin_batch_statuses_name])
	VALUES ([pin_batch_statuses_id],[pin_batch_statuses_name])

;

