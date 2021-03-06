

MERGE INTO [dbo].[external_system_types_language] AS trgt
USING	(VALUES
		(0,0,'Core Banking System'),
		(0,1,'Core Banking System_fr'),
		(0,2,'Core Banking System_pt'),
		(0,3,'Core Banking System_es'),
		(1,0,'Card Production System'),
		(1,1,'Card Production System_fr'),
		(1,2,'Card Production System_pt'),
		(1,3,'Card Production System_es'),
		(2,0,'Card Management System'),
		(2,1,'Card Management System_fr'),
		(2,2,'Card Management System_pt'),
		(2,3,'Card Management System_es'),
		(3,0,'Hardware Security Module'),
		(3,1,'Hardware Security Module_fr'),
		(3,2,'Hardware Security Module_pt'),
		(3,3,'Hardware Security Module_es'),
		(4,0,'Remote CMS'),
		(4,1,'Remote CMS_fr'),
		(4,2,'Remote CMS_pt'),
		(4,3,'Remote CMS_es')
		) AS src([external_system_type_id],[language_id],[language_text])
ON
	trgt.[external_system_type_id] = src.[external_system_type_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[external_system_type_id] = src.[external_system_type_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([external_system_type_id],[language_id],[language_text])
	VALUES ([external_system_type_id],[language_id],[language_text])

;

