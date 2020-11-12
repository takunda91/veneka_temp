

MERGE INTO [dbo].[print_field_types_language] AS trgt
USING	(VALUES
		(0,0,'Text'),
		(0,1,'Text_fr'),
		(0,2,'Text_pt'),
		(0,3,'Text_es'),
		(1,0,'Image'),
		(1,1,'Image_fr'),
		(1,2,'Image_pt'),
		(1,3,'Image_es')
	
		) AS src([print_field_type_id],[language_id],[language_text])
ON
	trgt.[print_field_type_id] = src.[print_field_type_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[print_field_type_id] = src.[print_field_type_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([print_field_type_id],[language_id],[language_text])
	VALUES ([print_field_type_id],[language_id],[language_text])

;

