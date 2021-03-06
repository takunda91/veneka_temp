

MERGE INTO [dbo].[branch_card_statuses_language] AS trgt
USING	(VALUES
		(0,0,'CHECKED_IN'),
		(0,1,'DESALLOUÉES'),
		(0,2,'CHECKED_IN_pt'),
		(0,3,'CHECKED_IN_sp'),
		(1,0,'AVAILABLE_FOR_ISSUE'),
		(1,1,'PRÊTES POUR EMISSION'),
		(1,2,'AVAILABLE_FOR_ISSUE_pt'),
		(1,3,'AVAILABLE_FOR_ISSUE_sp'),
		(2,0,'ALLOCATED_TO_CUST'),
		(2,1,'EMISES AUX CLIENTS'),
		(2,2,'ALLOCATED_TO_CUST_pt'),
		(2,3,'ALLOCATED_TO_CUST_sp'),
		(3,0,'APPROVED_FOR_ISSUE'),
		(3,1,'APPROUVÉES POUR EMISSION'),
		(3,2,'APPROVED_FOR_ISSUE_pt'),
		(3,3,'APPROVED_FOR_ISSUE_sp'),
		(4,0,'CARD_PRINTED'),
		(4,1,'CARTES IMPRIMEES'),
		(4,2,'CARD_PRINTED_pt'),
		(4,3,'CARD_PRINTED_sp'),
		(5,0,'PIN_CAPTURED'),
		(5,1,'PIN ENREGISTRÉS'),
		(5,2,'PIN_CAPTURED_pt'),
		(5,3,'PIN_CAPTURED_sp'),
		(6,0,'ISSUED'),
		(6,1,'EMISES'),
		(6,2,'ISSUED_pt'),
		(6,3,'ISSUED_sp'),
		(7,0,'SPOILED'),
		(7,1,'DETRUITES'),
		(7,2,'SPOILED_pt'),
		(7,3,'SPOILED_sp'),
		(8,0,'PRINT_ERROR'),
		(8,1,'ERREUR D''IMPRESSION'),
		(8,2,'PRINT_ERROR_pt'),
		(8,3,'PRINT_ERROR_sp'),
		(9,0,'CMS_ERROR'),
		(9,1,'ERREUR CMS'),
		(9,2,'CMS_ERROR_pt'),
		(9,3,'CMS_ERROR_sp'),
		(10,0,'REQUESTED'),
		(10,1,'REQUESTED_fr'),
		(10,2,'REQUESTED_pt'),
		(10,3,'REQUESTED_es'),
		(11,0,'MAKERCHECKER_REJECT'),
		(11,1,'MAKERCHECKER_REJECT_fr'),
		(11,2,'MAKERCHECKER_REJECT_pt'),
		(11,3,'MAKERCHECKER_REJECT_es'),
		(12,0,'CARD_REQUEST_DELETED'),
		(12,1,'CARD_REQUEST_DELETED_fr'),
		(12,2,'CARD_REQUEST_DELETED_pt'),
		(12,3,'CARD_REQUEST_DELETED_es'),
		(13,0,'REDISTRIBUTED'),
		(13,1,'REDISTRIBUTED_fr'),
		(13,2,'REDISTRIBUTED_pt'),
		(13,3,'REDISTRIBUTED_es'),
		(14,0,'PIN_AUTHORISED'),
		(14,1,'PIN_AUTHORISED_fr'),
		(14,2,'PIN_AUTHORISED_pt'),
		(14,3,'PIN_AUTHORISED_es'),
		(15,0,'CMS_REUPLOAD'),
		(15,1,'CMS_REUPLOAD_fr'),
		(15,2,'CMS_REUPLOAD_pt'),
		(15,3,'CMS_REUPLOAD_es'),
		(16,0,'ASSIGN_TO_REQUEST'),
		(16,1,'ASSIGN_TO_REQUEST_fr'),
		(16,2,'ASSIGN_TO_REQUEST_pt'),
		(16,3,'ASSIGN_TO_REQUEST_es')
		) AS src([branch_card_statuses_id],[language_id],[language_text])
ON
	trgt.[branch_card_statuses_id] = src.[branch_card_statuses_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[branch_card_statuses_id] = src.[branch_card_statuses_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([branch_card_statuses_id],[language_id],[language_text])
	VALUES ([branch_card_statuses_id],[language_id],[language_text])

;

