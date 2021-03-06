

MERGE INTO [dbo].[connection_parameter_type] AS trgt
USING	(VALUES
		(0,'WEBSERVICE'),
		(1,'FILE_SYSTEM'),
		(2,'THALESHSM'),
		(3,'SOCKET'),
		(4,'Active Directory')
		) AS src([connection_parameter_type_id],[connection_parameter_type_name])
ON
	trgt.[connection_parameter_type_id] = src.[connection_parameter_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[connection_parameter_type_id] = src.[connection_parameter_type_id]
		, [connection_parameter_type_name] = src.[connection_parameter_type_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([connection_parameter_type_id],[connection_parameter_type_name])
	VALUES ([connection_parameter_type_id],[connection_parameter_type_name])

;

