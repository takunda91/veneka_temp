

MERGE INTO [dbo].[card_issue_method] AS trgt
USING	(VALUES
		(0,'CENTRALISED'),
		(1,'INSTANT')
		) AS src([card_issue_method_id],[card_issue_method_name])
ON
	trgt.[card_issue_method_id] = src.[card_issue_method_id]
WHEN MATCHED THEN
	UPDATE SET
		[card_issue_method_id] = src.[card_issue_method_id]
		, [card_issue_method_name] = src.[card_issue_method_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([card_issue_method_id],[card_issue_method_name])
	VALUES ([card_issue_method_id],[card_issue_method_name])

;

