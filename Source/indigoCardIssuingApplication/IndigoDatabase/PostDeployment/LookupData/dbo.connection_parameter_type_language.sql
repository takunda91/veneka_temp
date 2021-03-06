

MERGE INTO [dbo].[connection_parameter_type_language] AS trgt
USING	(VALUES
		(0,0,N'WEBSERVICE'),
		(0,1,N'WEBSERVICE_fr'),
		(0,2,N'WEBSERVICE_pt'),
		(0,3,N'WEBSERVICE_es'),
		(1,0,N'FILE_SYSTEM'),
		(1,1,N'FILE_SYSTEM_fr'),
		(1,2,N'FILE_SYSTEM_pt'),
		(1,3,N'FILE_SYSTEM_es'),
		(2,0,N'THALESHSM'),
		(2,1,N'THALESHSM_fr'),
		(2,2,N'THALESHSM_pt'),
		(2,3,N'THALESHSM_es'),
		(3,0,N'SOCKET'),
		(3,1,N'SOCKET_fr'),
		(3,2,N'SOCKET_pt'),
		(3,3,N'SOCKET_es'),
		(4,0,N'ACTIVE DIRECTORY'),
		(4,1,N'ACTIVE DIRECTORY_FR'),
		(4,2,N'ACTIVE DIRECTORY_PT'),
		(4,3,N'ACTIVE DIRECTORY_ES')
		) AS src([connection_parameter_type_id],[language_id],[language_text])
ON
	trgt.[connection_parameter_type_id] = src.[connection_parameter_type_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[connection_parameter_type_id] = src.[connection_parameter_type_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([connection_parameter_type_id],[language_id],[language_text])
	VALUES ([connection_parameter_type_id],[language_id],[language_text])

;

