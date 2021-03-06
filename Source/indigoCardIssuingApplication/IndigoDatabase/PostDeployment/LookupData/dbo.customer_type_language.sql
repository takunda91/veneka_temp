

MERGE INTO [dbo].[customer_type_language] AS trgt
USING	(VALUES
		(0,0,'PRIVATE'),
		(0,1,'Privé'),
		(0,2,'PRIVATE_pt'),
		(0,3,'PRIVATE_sp'),
		(1,0,'CORPORATE'),
		(1,1,'Entreprise'),
		(1,2,'CORPORATE_pt'),
		(1,3,'CORPORATE_sp')
		) AS src([customer_type_id],[language_id],[language_text])
ON
	trgt.[customer_type_id] = src.[customer_type_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[customer_type_id] = src.[customer_type_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([customer_type_id],[language_id],[language_text])
	VALUES ([customer_type_id],[language_id],[language_text])

;

