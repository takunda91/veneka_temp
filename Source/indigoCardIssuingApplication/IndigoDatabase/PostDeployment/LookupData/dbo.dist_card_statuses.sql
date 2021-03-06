

MERGE INTO [dbo].[dist_card_statuses] AS trgt
USING	(VALUES
		(0,'ALLOCATED_TO_BRANCH'),
		(1,'AVAILABLE_FOR_ISSUE'),
		(2,'RECEIVED_AT_BRANCH'),
		(3,'ALLOCATED_TO_CUST'),
		(4,'CARD_PRINTED'),
		(5,'PIN_CAPTURED'),
		(6,'ISSUED'),
		(7,'REJECTED'),
		(8,'CANCELLED'),
		(9,'INVALID'),
		(10,'LINKED_TO_ACCOUNT'),
		(11,'SPOILED'),
		(12,'CREATED'),
		(13,'PAN_GENERATED'),
		(14,'SECURITY_GENERATED'),
		(15,'PIN_MAILER_PRINTED'),
		(16,'CARD_PRODUCED'),
		(17,'PIN_PRINTED'),
		(18,'RECEIVED_AT_CARD_CENTRE'),
		(19,'ALLOCATED_TO_CARD_CENTRE'),
		(20,'LOAD_PENDING'),
		(21,'LOAD_COMPLETE'),
		(22,'REMOVED'),
		(23,'PRINT_FAILED')
		) AS src([dist_card_status_id],[dist_card_status_name])
ON
	trgt.[dist_card_status_id] = src.[dist_card_status_id]
WHEN MATCHED THEN
	UPDATE SET
		[dist_card_status_id] = src.[dist_card_status_id]
		, [dist_card_status_name] = src.[dist_card_status_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([dist_card_status_id],[dist_card_status_name])
	VALUES ([dist_card_status_id],[dist_card_status_name])

;

