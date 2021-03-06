

MERGE INTO [dbo].[card_issue_reason_language] AS trgt
USING	(VALUES
		(0,0,'NEW ACCOUNT - NEW CUSTOMER'),
		(0,1,'NOUVEAU COMPTE - NOUVEAU CLIENT'),
		(0,2,'NEW ACCOUNT - NEW CUSTOMER_pt'),
		(0,3,'NEW ACCOUNT - NEW CUSTOMER_sp'),
		(1,0,'NEW ACCOUNT - EXISTING CUSTOMER'),
		(1,1,'NOUVEAU COMPTE - CLIENT EXISTANT'),
		(1,2,'NEW ACCOUNT - EXISTING CUSTOMER_pt'),
		(1,3,'NEW ACCOUNT - EXISTING CUSTOMER_sp'),
		(2,0,'CARD RENEWAL'),
		(2,1,'RENOUVELLEMENT DE CARTE'),
		(2,2,'CARD RENEWAL_pt'),
		(2,3,'CARD RENEWAL_sp'),
		(3,0,'CARD REPLACEMENT'),
		(3,1,'REMPLACEMENT DE CARTE'),
		(3,2,'CARD REPLACEMENT_pt'),
		(3,3,'CARD REPLACEMENT_sp'),
		(4,0,'SUPPLEMENTARY CARD'),
		(4,1,'CARTE SUPPLMENTAIRE'),
		(4,2,'SUPPLEMENTARY CARD_pt'),
		(4,3,'SUPPLEMENTARY CARD_sp')
		) AS src([card_issue_reason_id],[language_id],[language_text])
ON
	trgt.[card_issue_reason_id] = src.[card_issue_reason_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[card_issue_reason_id] = src.[card_issue_reason_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([card_issue_reason_id],[language_id],[language_text])
	VALUES ([card_issue_reason_id],[language_id],[language_text])

;

