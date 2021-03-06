

MERGE INTO [dbo].[external_system_types] AS trgt
USING	(VALUES
		(0,'Core Banking System'),
		(1,'Card Production System'),
		(2,'Card Management System'),
		(3,'Hardware Security Module'),
		(4,'Remote CMS')
		) AS src([external_system_type_id],[system_type_name])
ON
	trgt.[external_system_type_id] = src.[external_system_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[external_system_type_id] = src.[external_system_type_id]
		, [system_type_name] = src.[system_type_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([external_system_type_id],[system_type_name])
	VALUES ([external_system_type_id],[system_type_name])

;

