

MERGE INTO [dbo].[card_issue_method_language] AS trgt
USING	(VALUES
		(0,0,N'CENTRALISED'),
		(0,1,N'CENTRALISED_fr'),
		(0,2,N'CENTRALISED_pt'),
		(0,3,N'CENTRALISED_sp'),
		(1,0,N'INSTANT'),
		(1,1,N'INSTANT_fr'),
		(1,2,N'INSTANT_pt'),
		(1,3,N'INSTANT_sp')
		) AS src([card_issue_method_id],[language_id],[language_text])
ON
	trgt.[card_issue_method_id] = src.[card_issue_method_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[card_issue_method_id] = src.[card_issue_method_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([card_issue_method_id],[language_id],[language_text])
	VALUES ([card_issue_method_id],[language_id],[language_text])

;

