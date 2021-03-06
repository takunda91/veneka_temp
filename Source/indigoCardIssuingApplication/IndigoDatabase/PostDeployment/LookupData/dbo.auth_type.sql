

MERGE INTO [dbo].[auth_type] AS trgt
USING	(VALUES
		(0,N'ExternalAuth'),
		(1,N'MultiFactor')
		) AS src([auth_type_id],[auth_type_name])
ON
	trgt.[auth_type_id] = src.[auth_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[auth_type_id] = src.[auth_type_id]
		, [auth_type_name] = src.[auth_type_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([auth_type_id],[auth_type_name])
	VALUES ([auth_type_id],[auth_type_name])

;

