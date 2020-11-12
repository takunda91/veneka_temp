

MERGE INTO [dbo].[users_to_users_groups] AS trgt
USING	(VALUES
		(-3,-4),
		(-2,-3),
		(-1,-2),
		(-1,-1)
		) AS src([user_id],[user_group_id])
ON
	trgt.[user_id] = src.[user_id]
AND trgt.[user_group_id] = src.[user_group_id]
WHEN MATCHED THEN
	UPDATE SET
		[user_id] = src.[user_id]
		, [user_group_id] = src.[user_group_id]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([user_id],[user_group_id])
	VALUES ([user_id],[user_group_id])

;

