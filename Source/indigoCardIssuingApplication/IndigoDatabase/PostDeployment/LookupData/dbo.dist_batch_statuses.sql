

MERGE INTO [dbo].[dist_batch_statuses] AS trgt
USING	(VALUES
		(0,'CREATED',NULL),
		(1,'APPROVED',0),
		(2,'DISPATCHED',1),
		(3,'RECEIVED_AT_BRANCH',2),
		(4,'REJECTED_AT_BRANCH',2),
		(5,'REJECT_AND_REISSUE',0),
		(6,'REJECT_AND_CANCEL',0),
		(7,'INVALID',NULL),
		(8,'REJECTED',NULL),
		(9,'APPROVED_FOR_PRODUCTION',NULL),
		(10,'SENT_TO_CMS',NULL),
		(11,'PROCESSED_IN_CMS',NULL),
		(12,'AT_CARD_PRODUCTION',NULL),
		(13,'CARDS_PRODUCED',NULL),
		(14,'RECEIVED_AT_CARD_CENTER',NULL),
		(15,'FAILED_IN_CMS',NULL),
		(16,'REJECTED_AT_CARD_CENTER',NULL),
		(17,'SENT_TO_PRINTER',NULL),
		(18,'PIN_PRINTED',NULL),
		(19,'DISPATCHED_TO_CC',NULL),
		(20,'LOAD_PENDING',NULL),
		(21,'LOAD_COMPLETE',NULL),
		(22,'REMOVED',NULL),
		(23,'LOAD_PENDING_APPROVAL',NULL),
		(24,'PRINT_SUCCESSFUL',NULL),
		(25,'PARTIAL_PRINT_SUCCESSFUL',NULL)
		) AS src([dist_batch_statuses_id],[dist_batch_status_name],[dist_batch_expected_statuses_id])
ON
	trgt.[dist_batch_statuses_id] = src.[dist_batch_statuses_id]
WHEN MATCHED THEN
	UPDATE SET
		[dist_batch_statuses_id] = src.[dist_batch_statuses_id]
		, [dist_batch_status_name] = src.[dist_batch_status_name]
		, [dist_batch_expected_statuses_id] = src.[dist_batch_expected_statuses_id]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([dist_batch_statuses_id],[dist_batch_status_name],[dist_batch_expected_statuses_id])
	VALUES ([dist_batch_statuses_id],[dist_batch_status_name],[dist_batch_expected_statuses_id])

;

