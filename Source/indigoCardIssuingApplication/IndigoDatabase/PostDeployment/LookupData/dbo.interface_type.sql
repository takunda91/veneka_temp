

MERGE INTO [dbo].[interface_type] AS trgt
USING	(VALUES
		(0,'CBS'),
		(1,'CMS'),
		(2,'HSM'),
		(3,'CARD_PRODUCTION'),
		(4,'FILE_LOADER'),
		(5,'FEE_SCHEME'),
		(6,'External auth'),
		(7,'NOTIFICATIONS'),
		(8,'FILE_EXPORT'),
		(9,'REMOTE_CMS'),
		(10,'MULTIFACTOR'),
		(11,'3D_SECURE')
		) AS src([interface_type_id],[interface_type_name])
ON
	trgt.[interface_type_id] = src.[interface_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[interface_type_id] = src.[interface_type_id]
		, [interface_type_name] = src.[interface_type_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([interface_type_id],[interface_type_name])
	VALUES ([interface_type_id],[interface_type_name])

;

