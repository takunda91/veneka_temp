

MERGE INTO [dbo].[pin_batch_statuses_language] AS trgt
USING	(VALUES
		(0,0,'CREATED'),
		(0,1,'CREATED_fr'),
		(0,2,'CREATED_pt'),
		(0,3,'CREATED_es'),
		(1,0,'DISPATCHED_TO_CC'),
		(1,1,'DISPATCHED_TO_CC_fr'),
		(1,2,'DISPATCHED_TO_CC_pt'),
		(1,3,'DISPATCHED_TO_CC_es'),
		(2,0,'RECIEVED_AT_CC'),
		(2,1,'RECIEVED_AT_CC_fr'),
		(2,2,'RECIEVED_AT_CC_pt'),
		(2,3,'RECIEVED_AT_CC_es'),
		(3,0,'DISPATCHED_TO_BRANCH'),
		(3,1,'DISPATCHED_TO_BRANCH_fr'),
		(3,2,'DISPATCHED_TO_BRANCH_pt'),
		(3,3,'DISPATCHED_TO_BRANCH_es'),
		(4,0,'RECIEVED_AT_BRANCH'),
		(4,1,'RECIEVED_AT_BRANCH_fr'),
		(4,2,'RECIEVED_AT_BRANCH_pt'),
		(4,3,'RECIEVED_AT_BRANCH_es'),
		(5,0,'REJECTED_AT_CC'),
		(5,1,'REJECTED_AT_CC_fr'),
		(5,2,'REJECTED_AT_CC_pt'),
		(5,3,'REJECTED_AT_CC_es'),
		(6,0,'REJECTED_AT_BRANCH'),
		(6,1,'REJECTED_AT_BRANCH_fr'),
		(6,2,'REJECTED_AT_BRANCH_pt'),
		(6,3,'REJECTED_AT_BRANCH_es'),
		(7,0,'SENT_TO_PRINTER'),
		(7,1,'SENT_TO_PRINTER_fr'),
		(7,2,'SENT_TO_PRINTER_pt'),
		(7,3,'SENT_TO_PRINTER_es'),
		(8,0,'PIN_PRINTED'),
		(8,1,'PIN_PRINTED_fr'),
		(8,2,'PIN_PRINTED_pt'),
		(8,3,'PIN_PRINTED_es'),
		(9,0,'SENT_TO_CMS'),
		(9,1,'SENT_TO_CMS_fr'),
		(9,2,'SENT_TO_CMS_pt'),
		(9,3,'SENT_TO_CMS_es'),
		(10,0,'PROCESSED_IN_CMS'),
		(10,1,'PROCESSED_IN_CMS_fr'),
		(10,2,'PROCESSED_IN_CMS_pt'),
		(10,3,'PROCESSED_IN_CMS_es'),
		(11,0,'APPROVED'),
		(11,1,'APPROVED_fr'),
		(11,2,'APPROVED_pt'),
		(11,3,'APPROVED_es')
		) AS src([pin_batch_statuses_id],[language_id],[language_text])
ON
	trgt.[pin_batch_statuses_id] = src.[pin_batch_statuses_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[pin_batch_statuses_id] = src.[pin_batch_statuses_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([pin_batch_statuses_id],[language_id],[language_text])
	VALUES ([pin_batch_statuses_id],[language_id],[language_text])

;

