

MERGE INTO [dbo].[interface_type_language] AS trgt
USING	(VALUES
		(0,0,'ACCOUNT_VALIDATION'),
		(0,1,'VALIDATION DU COMPTE'),
		(0,2,'ACCOUNT_VALIDATION_pt'),
		(0,3,'ACCOUNT_VALIDATION_sp'),
		(1,0,'CORE_BANKING'),
		(1,1,'SERVICE BANCAIRES DE BASE'),
		(1,2,'CORE_BANKING_pt'),
		(1,3,'CORE_BANKING_sp'),
		(2,0,'HSM'),
		(2,1,'HSM_fr'),
		(2,2,'HSM_pt'),
		(2,3,'HSM_sp'),
		(3,0,'CARD_PRODUCTION'),
		(3,1,'CARD_PRODUCTION_fr'),
		(3,2,'CARD_PRODUCTION_pt'),
		(3,3,'CARD_PRODUCTION_sp'),
		(4,0,'FILE_LOADER'),
		(4,1,'FILE_LOADER_fr'),
		(4,2,'FILE_LOADER_pt'),
		(4,3,'FILE_LOADER_sp'),
		(5,0,'FEE_SCHEME'),
		(5,1,'FEE_SCHEME_fr'),
		(5,2,'FEE_SCHEME_pt'),
		(5,3,'FEE_SCHEME_sp'),
		(7,0,'NOTIFICATIONS'),
		(7,1,'NOTIFICATIONS_fr'),
		(7,2,'NOTIFICATIONS_pt'),
		(7,3,'NOTIFICATIONS_es'),
		(8,0,'FILE_EXPORT'),
		(8,1,'FILE_EXPORT_fr'),
		(8,2,'FILE_EXPORT_pt'),
		(8,3,'FILE_EXPORT_es'),
		(9,0,'REMOTE_CMS'),
		(9,1,'REMOTE_CMS_fr'),
		(9,2,'REMOTE_CMS_pt'),
		(9,3,'REMOTE_CMS_es'),
		(10,0,'MULTIFACTOR'),
		(10,1,'MULTIFACTOR_fr'),
		(10,2,'MULTIFACTOR_pt'),
		(10,3,'MULTIFACTOR_es'),
		(11,0,'3D_SECURE'),
		(11,1,'3D_SECURE_fr'),
		(11,2,'3D_SECURE_pt'),
		(11,3,'3D_SECURE_es')
		) AS src([interface_type_id],[language_id],[language_text])
ON
	trgt.[interface_type_id] = src.[interface_type_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[interface_type_id] = src.[interface_type_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([interface_type_id],[language_id],[language_text])
	VALUES ([interface_type_id],[language_id],[language_text])

;

