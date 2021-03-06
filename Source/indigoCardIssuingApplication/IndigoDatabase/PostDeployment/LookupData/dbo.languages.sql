

MERGE INTO [dbo].[languages] AS trgt
USING	(VALUES
		(0,N'English','Anglais','English_pt','English_sp'),
		(1,N'French','Français','French_pt','French_sp'),
		(2,N'Portuguese','Portugais','Portuguese_pt','Portuguese_sp'),
		(3,N'Spanish','Espagnol','Spanish_pt','Spanish_sp')
		) AS src([id],[language_name],[language_name_fr],[language_name_pt],[language_name_sp])
ON
	trgt.[id] = src.[id]
WHEN MATCHED THEN
	UPDATE SET
		[id] = src.[id]
		, [language_name] = src.[language_name]
		, [language_name_fr] = src.[language_name_fr]
		, [language_name_pt] = src.[language_name_pt]
		, [language_name_sp] = src.[language_name_sp]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([id],[language_name],[language_name_fr],[language_name_pt],[language_name_sp])
	VALUES ([id],[language_name],[language_name_fr],[language_name_pt],[language_name_sp])

;

