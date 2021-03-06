

MERGE INTO [dbo].[card_priority] AS trgt
USING	(VALUES
		(0,1,'HIGH',0),
		(1,2,'NORMAL',1),
		(2,3,'LOW',0)
		) AS src([card_priority_id],[card_priority_order],[card_priority_name],[default_selection])
ON
	trgt.[card_priority_id] = src.[card_priority_id]
WHEN MATCHED THEN
	UPDATE SET
		[card_priority_id] = src.[card_priority_id]
		, [card_priority_order] = src.[card_priority_order]
		, [card_priority_name] = src.[card_priority_name]
		, [default_selection] = src.[default_selection]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([card_priority_id],[card_priority_order],[card_priority_name],[default_selection])
	VALUES ([card_priority_id],[card_priority_order],[card_priority_name],[default_selection])

;

