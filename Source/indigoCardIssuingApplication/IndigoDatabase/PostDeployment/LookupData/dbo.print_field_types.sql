

MERGE INTO [dbo].[print_field_types] AS trgt
USING	(VALUES
		(0,'Text'),
		(1,'Image')
		) AS src([print_field_type_id],[print_field_name])
ON
	trgt.[print_field_type_id] = src.[print_field_type_id]
WHEN MATCHED THEN
	UPDATE SET
		[print_field_type_id] = src.[print_field_type_id]
		, [print_field_name] = src.[print_field_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([print_field_type_id],[print_field_name])
	VALUES ([print_field_type_id],[print_field_name])

;

