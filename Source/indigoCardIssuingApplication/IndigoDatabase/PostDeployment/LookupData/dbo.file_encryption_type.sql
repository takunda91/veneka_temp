

MERGE INTO [dbo].[file_encryption_type] AS trgt
USING	(VALUES
		(0,'NONE',NULL),
		(1,'PGP',NULL)
		) AS src([file_encryption_type_id],[file_encryption_type],[file_encryption_typeid])
ON
	trgt.[file_encryption_type_id] = src.[file_encryption_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[file_encryption_type_id] = src.[file_encryption_type_id]
		, [file_encryption_type] = src.[file_encryption_type]
		, [file_encryption_typeid] = src.[file_encryption_typeid]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([file_encryption_type_id],[file_encryption_type],[file_encryption_typeid])
	VALUES ([file_encryption_type_id],[file_encryption_type],[file_encryption_typeid])

;

