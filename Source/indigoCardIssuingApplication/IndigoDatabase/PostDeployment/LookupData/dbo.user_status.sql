

MERGE INTO [dbo].[user_status] AS trgt
USING	(VALUES
		(0,'ACTIVE'),
		(1,'INACTIVE'),
		(2,'DELETED'),
		(3,'ACCOUNT_LOCKED')
		) AS src([user_status_id],[user_status_text])
ON
	trgt.[user_status_id] = src.[user_status_id]
WHEN MATCHED THEN
	UPDATE SET
		[user_status_id] = src.[user_status_id]
		, [user_status_text] = src.[user_status_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([user_status_id],[user_status_text])
	VALUES ([user_status_id],[user_status_text])

;

