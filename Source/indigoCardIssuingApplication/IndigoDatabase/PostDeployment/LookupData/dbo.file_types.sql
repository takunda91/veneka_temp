

MERGE INTO [dbo].[file_types] AS trgt
USING	(VALUES
		(0,'PIN_MAILER'),
		(1,'CARD_IMPORT'),
		(3,'UNKNOWN')
		) AS src([file_type_id],[file_type])
ON
	trgt.[file_type_id] = src.[file_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[file_type_id] = src.[file_type_id]
		, [file_type] = src.[file_type]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([file_type_id],[file_type])
	VALUES ([file_type_id],[file_type])

;

