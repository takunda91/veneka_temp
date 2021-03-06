

MERGE INTO [dbo].[branch_card_statuses] AS trgt
USING	(VALUES
		(0,'CHECKED_IN'),
		(1,'AVAILABLE_FOR_ISSUE'),
		(2,'ALLOCATED_TO_CUST'),
		(3,'APPROVED_FOR_ISSUE'),
		(4,'CARD_PRINTED'),
		(5,'PIN_CAPTURED'),
		(6,'ISSUED'),
		(7,'SPOILED'),
		(8,'PRINT_ERROR'),
		(9,'CMS_ERROR'),
		(10,'REQUESTED'),
		(11,'MAKERCHECKER_REJECT'),
		(12,'CARD_REQUEST_DELETED'),
		(13,'REDISTRIBUTED'),
		(14,'PIN_AUTHORISED'),
		(15,'CMS_REUPLOAD'),
		(16,'ASSIGN_TO_REQUEST')
		) AS src([branch_card_statuses_id],[branch_card_statuses_name])
ON
	trgt.[branch_card_statuses_id] = src.[branch_card_statuses_id]
WHEN MATCHED THEN
	UPDATE SET
		[branch_card_statuses_id] = src.[branch_card_statuses_id]
		, [branch_card_statuses_name] = src.[branch_card_statuses_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([branch_card_statuses_id],[branch_card_statuses_name])
	VALUES ([branch_card_statuses_id],[branch_card_statuses_name])

;

