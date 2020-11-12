

MERGE INTO [dbo].[dist_card_statuses_language] AS trgt
USING	(VALUES
		(0,0,'ALLOCATED_TO_BRANCH'),
		(0,1,'ALLOCATED_TO_BRANCH_fr'),
		(0,2,'ALLOCATED_TO_BRANCH_pt'),
		(0,3,'ALLOCATED_TO_BRANCH_sp'),
		(1,0,'AVAILABLE_FOR_ISSUE'),
		(1,1,'AVAILABLE_FOR_ISSUE_fr'),
		(1,2,'AVAILABLE_FOR_ISSUE_pt'),
		(1,3,'AVAILABLE_FOR_ISSUE_sp'),
		(2,0,'RECEIVED_AT_BRANCH'),
		(2,1,'RECEIVED_AT_BRANCH_fr'),
		(2,2,'RECEIVED_AT_BRANCH_pt'),
		(2,3,'RECEIVED_AT_BRANCH_sp'),
		(3,0,'ALLOCATED_TO_CUST'),
		(3,1,'ALLOCATED_TO_CUST_fr'),
		(3,2,'ALLOCATED_TO_CUST_pt'),
		(3,3,'ALLOCATED_TO_CUST_sp'),
		(4,0,'CARD_PRINTED'),
		(4,1,'CARD_PRINTED_fr'),
		(4,2,'CARD_PRINTED_pt'),
		(4,3,'CARD_PRINTED_sp'),
		(5,0,'PIN_CAPTURED'),
		(5,1,'PIN_CAPTURED_fr'),
		(5,2,'PIN_CAPTURED_pt'),
		(5,3,'PIN_CAPTURED_sp'),
		(6,0,'ISSUED'),
		(6,1,'ISSUED_fr'),
		(6,2,'ISSUED_pt'),
		(6,3,'ISSUED_sp'),
		(7,0,'REJECTED'),
		(7,1,'REJECTED_fr'),
		(7,2,'REJECTED_pt'),
		(7,3,'REJECTED_sp'),
		(8,0,'CANCELLED'),
		(8,1,'CANCELLED_fr'),
		(8,2,'CANCELLED_pt'),
		(8,3,'CANCELLED_sp'),
		(9,0,'INVALID'),
		(9,1,'INVALID_fr'),
		(9,2,'INVALID_pt'),
		(9,3,'INVALID_sp'),
		(10,0,'LINKED_TO_ACCOUNT'),
		(10,1,'LINKED_TO_ACCOUNT_fr'),
		(10,2,'LINKED_TO_ACCOUNT_pt'),
		(10,3,'LINKED_TO_ACCOUNT_sp'),
		(11,0,'SPOILED'),
		(11,1,'SPOILED_fr'),
		(11,2,'SPOILED_pt'),
		(11,3,'SPOILED_sp'),
		(12,0,'CREATED'),
		(12,1,'CREATED_fr'),
		(12,2,'CREATED_pt'),
		(12,3,'CREATEDes'),
		(13,0,'PAN_GENERATED'),
		(13,1,'PAN_GENERATED_fr'),
		(13,2,'PAN_GENERATED_pt'),
		(13,3,'PAN_GENERATEDes'),
		(14,0,'SECURITY_GENERATED'),
		(14,1,'SECURITY_GENERATED_fr'),
		(14,2,'SECURITY_GENERATED_pt'),
		(14,3,'SECURITY_GENERATEDes'),
		(15,0,'PIN_MAILER_PRINTED'),
		(15,1,'PIN_MAILER_PRINTED_fr'),
		(15,2,'PIN_MAILER_PRINTED_pt'),
		(15,3,'PIN_MAILER_PRINTEDes'),
		(16,0,'CARD_PRODUCED'),
		(16,1,'CARD_PRODUCED_fr'),
		(16,2,'CARD_PRODUCED_pt'),
		(16,3,'CARD_PRODUCEDes'),
		(17,0,'PIN_PRINTED'),
		(17,1,'PIN_PRINTED_fr'),
		(17,2,'PIN_PRINTED_pt'),
		(17,3,'PIN_PRINTED_es'),
		(18,0,'RECEIVED_AT_CARD_CENTRE'),
		(18,1,'RECEIVED_AT_CARD_CENTRE_fr'),
		(18,2,'RECEIVED_AT_CARD_CENTRE_pt'),
		(18,3,'RECEIVED_AT_CARD_CENTRE_es'),
		(19,0,'ALLOCATED_TO_CARD_CENTRE'),
		(19,1,'ALLOCATED_TO_CARD_CENTRE_fr'),
		(19,2,'ALLOCATED_TO_CARD_CENTRE_pt'),
		(19,3,'ALLOCATED_TO_CARD_CENTRE_es'),
		(20,0,'LOAD_PENDING'),
		(20,1,'LOAD_PENDING_fr'),
		(20,2,'LOAD_PENDING_pt'),
		(20,3,'LOAD_PENDING_es'),
		(21,0,'LOAD_COMPLETE'),
		(21,1,'LOAD_COMPLETE_fr'),
		(21,2,'LOAD_COMPLETE_pt'),
		(21,3,'LOAD_COMPLETE_es'),
		(22,0,'REMOVED'),
		(22,1,'REMOVED_fr'),
		(22,2,'REMOVED_pt'),
		(22,3,'REMOVED_es'),
		(23,0,'PRINT_FAILED'),
		(23,1,'PRINT_FAILED_fr'),
		(23,2,'PRINT_FAILED_pt'),
		(23,3,'PRINT_FAILED_es')
		) AS src([dist_card_status_id],[language_id],[language_text])
ON
	trgt.[dist_card_status_id] = src.[dist_card_status_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[dist_card_status_id] = src.[dist_card_status_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([dist_card_status_id],[language_id],[language_text])
	VALUES ([dist_card_status_id],[language_id],[language_text])

;

