

MERGE INTO [dbo].[card_priority_language] AS trgt
USING	(VALUES
		(0,0,N'HIGH'),
		(0,1,N'HIGH_fr'),
		(0,2,N'HIGH_pt'),
		(0,3,N'HIGH_sp'),
		(1,0,N'NORMAL'),
		(1,1,N'NORMAL_fr'),
		(1,2,N'NORMAL_pt'),
		(1,3,N'NORMAL_sp'),
		(2,0,N'LOW'),
		(2,1,N'LOW_fr'),
		(2,2,N'LOW_pt'),
		(2,3,N'LOW_sp')
		) AS src([card_priority_id],[language_id],[language_text])
ON
	trgt.[card_priority_id] = src.[card_priority_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[card_priority_id] = src.[card_priority_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([card_priority_id],[language_id],[language_text])
	VALUES ([card_priority_id],[language_id],[language_text])

;

