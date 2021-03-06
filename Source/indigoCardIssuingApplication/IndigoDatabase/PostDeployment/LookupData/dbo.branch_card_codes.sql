

MERGE INTO [dbo].[branch_card_codes] AS trgt
USING	(VALUES
		(0,0,'PRINT_SUCCESS',1,0,0),
		(1,0,'PRINTER_JAMMED',1,1,1),
		(2,0,'CARD_INSERTED_INCORRECTLY',1,1,1),
		(3,0,'PRINTER_NO_INK',1,1,1),
		(4,1,'CMS_SUCCESS',1,0,0),
		(5,1,'CARD_NOT_FOUND',1,1,1),
		(6,1,'ACCOUNT_NOT_FOUND',1,1,1),
		(7,1,'UNKNOWN_ERROR',1,1,1),
		(8,1,'RELINK_FAILED',1,0,1),
		(9,1,'EDIT_FAILED',1,0,1)
		) AS src([branch_card_code_id],[branch_card_code_type_id],[branch_card_code_name],[branch_card_code_enabled],[spoil_only],[is_exception])
ON
	trgt.[branch_card_code_id] = src.[branch_card_code_id]
WHEN MATCHED THEN
	UPDATE SET
		[branch_card_code_id] = src.[branch_card_code_id]
		, [branch_card_code_type_id] = src.[branch_card_code_type_id]
		, [branch_card_code_name] = src.[branch_card_code_name]
		, [branch_card_code_enabled] = src.[branch_card_code_enabled]
		, [spoil_only] = src.[spoil_only]
		, [is_exception] = src.[is_exception]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([branch_card_code_id],[branch_card_code_type_id],[branch_card_code_name],[branch_card_code_enabled],[spoil_only],[is_exception])
	VALUES ([branch_card_code_id],[branch_card_code_type_id],[branch_card_code_name],[branch_card_code_enabled],[spoil_only],[is_exception])

;

