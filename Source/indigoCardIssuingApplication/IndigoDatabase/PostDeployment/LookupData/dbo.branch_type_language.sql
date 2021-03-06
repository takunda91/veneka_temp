

MERGE INTO [dbo].[branch_type_language] AS trgt
USING	(VALUES
		(0,0,'CARD_CENTER'),
		(1,0,'MAIN'),
		(2,0,'SATELLITE'),
		(0,1,'CARD_CENTER_FR'),
		(1,1,'MAIN_FR'),
		(2,1,'SATELLITE_FR'),
		(0,2,'CARD_CENTER_ES'),
		(1,2,'MAIN_ES'),
		(2,2,'SATELLITE_ES'),
		(0,3,'CARD_CENTER_PT'),
		(1,3,'MAIN_PT'),
		(2,3,'SATELLITE_PT')
		) AS src([branch_type_id],[language_id],[language_text])
ON
	trgt.[branch_type_id] = src.[branch_type_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[branch_type_id] = src.[branch_type_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([branch_type_id],[language_id],[language_text])
	VALUES ([branch_type_id],[language_id],[language_text])

;

