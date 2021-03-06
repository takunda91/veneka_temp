

MERGE INTO [dbo].[card_issue_reason] AS trgt
USING	(VALUES
		(0,'NEW ACCOUNT - NEW CUSTOMER'),
		(1,'NEW ACCOUNT - EXISTING CUSTOMER'),
		(2,'CARD RENEWAL'),
		(3,'CARD REPLACEMENT'),
		(4,'SUPPLEMENTARY CARD')
		) AS src([card_issue_reason_id],[card_issuer_reason_name])
ON
	trgt.[card_issue_reason_id] = src.[card_issue_reason_id]
WHEN MATCHED THEN
	UPDATE SET
		[card_issue_reason_id] = src.[card_issue_reason_id]
		, [card_issuer_reason_name] = src.[card_issuer_reason_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([card_issue_reason_id],[card_issuer_reason_name])
	VALUES ([card_issue_reason_id],[card_issuer_reason_name])

;

