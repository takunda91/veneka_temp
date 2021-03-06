SET IDENTITY_INSERT [dbo].[user_gender] ON;

MERGE INTO [dbo].[user_gender] AS trgt
USING	(VALUES
		(1,'MALE'),
		(2,'FEMALE'),
		(3,'UNSPECIFIED')
		) AS src([user_gender_id],[user_gender_text])
ON
	trgt.[user_gender_id] = src.[user_gender_id]
WHEN MATCHED THEN
	UPDATE SET
		[user_gender_text] = src.[user_gender_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([user_gender_id],[user_gender_text])
	VALUES ([user_gender_id],[user_gender_text])

;
SET IDENTITY_INSERT [dbo].[user_gender] OFF;
