

MERGE INTO [dbo].[customer_account_type] AS trgt
USING	(VALUES
		(0,'CURRENT',1),
		(1,'SAVINGS',1),
		(2,'CHEQUE',1),
		(3,'CREDIT',1),
		(4,'UNIVERSAL',1),
		(5,'INVESTMENT',1)
		) AS src([account_type_id],[account_type_name],[active_YN])
ON
	trgt.[account_type_id] = src.[account_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[account_type_id] = src.[account_type_id]
		, [account_type_name] = src.[account_type_name]
		, [active_YN] = src.[active_YN]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([account_type_id],[account_type_name],[active_YN])
	VALUES ([account_type_id],[account_type_name],[active_YN])

;

