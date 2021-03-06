

MERGE INTO [dbo].[branch_type] AS trgt
USING	(VALUES
		(0,N'CARD_CENTER'),
		(1,N'MAIN'),
		(2,N'SATELLITE')
		) AS src([branch_type_id],[branch_type_name])
ON
	trgt.[branch_type_id] = src.[branch_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[branch_type_id] = src.[branch_type_id]
		, [branch_type_name] = src.[branch_type_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([branch_type_id],[branch_type_name])
	VALUES ([branch_type_id],[branch_type_name])

;

