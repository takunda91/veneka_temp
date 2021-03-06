

MERGE INTO [dbo].[pin_batch_card_statuses] AS trgt
USING	(VALUES
		(0,'CREATED'),
		(1,'ALLOCATED'),
		(2,'RECEIVED_AT_CC'),
		(3,'ALLOCATED_TO_BRANCH'),
		(4,'RECEIVED_AT_BRANCH'),
		(5,'REJECTED')
		) AS src([pin_batch_card_statuses_id],[pin_batch_card_statuses_name])
ON
	trgt.[pin_batch_card_statuses_id] = src.[pin_batch_card_statuses_id]
WHEN MATCHED THEN
	UPDATE SET
		[pin_batch_card_statuses_id] = src.[pin_batch_card_statuses_id]
		, [pin_batch_card_statuses_name] = src.[pin_batch_card_statuses_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([pin_batch_card_statuses_id],[pin_batch_card_statuses_name])
	VALUES ([pin_batch_card_statuses_id],[pin_batch_card_statuses_name])

;

