

MERGE INTO [dbo].[customer_account_type_language] AS trgt
USING	(VALUES
		(0,0,'CURRENT'),
		(0,1,'COURANT'),
		(0,2,'CURRENT_pt'),
		(0,3,'CURRENT_sp'),
		(1,0,'SAVINGS'),
		(1,1,'EPARGNE'),
		(1,2,'SAVINGS_pt'),
		(1,3,'SAVINGS_sp'),
		(2,0,'CHEQUE'),
		(2,1,'CHEQUE'),
		(2,2,'CHEQUE_pt'),
		(2,3,'CHEQUE_sp'),
		(3,0,'CREDIT'),
		(3,1,'CREDIT'),
		(3,2,'CREDIT_pt'),
		(3,3,'CREDIT_sp'),
		(4,0,'UNIVERSAL'),
		(4,1,'UNIVERSEL'),
		(4,2,'UNIVERSAL_pt'),
		(4,3,'UNIVERSAL_sp'),
		(5,0,'INVESTMENT'),
		(5,1,'INVESTISSEMENT'),
		(5,2,'INVESTMENT_pt'),
		(5,3,'INVESTMENT_sp')
		) AS src([account_type_id],[language_id],[language_text])
ON
	trgt.[account_type_id] = src.[account_type_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[account_type_id] = src.[account_type_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([account_type_id],[language_id],[language_text])
	VALUES ([account_type_id],[language_id],[language_text])

;

