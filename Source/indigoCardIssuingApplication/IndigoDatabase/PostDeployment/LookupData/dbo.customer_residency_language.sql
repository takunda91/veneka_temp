

MERGE INTO [dbo].[customer_residency_language] AS trgt
USING	(VALUES
		(0,0,'RESIDENT'),
		(0,1,'Résident'),
		(0,2,'RESIDENT_pt'),
		(0,3,'RESIDENT_sp'),
		(1,0,'NONRESIDENT'),
		(1,1,'Non résident'),
		(1,2,'NONRESIDENT_pt'),
		(1,3,'NONRESIDENT_sp')
		) AS src([resident_id],[language_id],[language_text])
ON
	trgt.[resident_id] = src.[resident_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[resident_id] = src.[resident_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([resident_id],[language_id],[language_text])
	VALUES ([resident_id],[language_id],[language_text])

;

