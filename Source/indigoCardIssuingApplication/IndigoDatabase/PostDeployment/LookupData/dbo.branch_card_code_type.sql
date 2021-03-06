

MERGE INTO [dbo].[branch_card_code_type] AS trgt
USING	(VALUES
		(0,'PRINTER'),
		(1,'CMS')
		) AS src([branch_card_code_type_id],[branch_card_code_name])
ON
	trgt.[branch_card_code_type_id] = src.[branch_card_code_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[branch_card_code_type_id] = src.[branch_card_code_type_id]
		, [branch_card_code_name] = src.[branch_card_code_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([branch_card_code_type_id],[branch_card_code_name])
	VALUES ([branch_card_code_type_id],[branch_card_code_name])

;

