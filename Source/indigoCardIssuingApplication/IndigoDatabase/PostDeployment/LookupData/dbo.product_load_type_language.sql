

MERGE INTO [dbo].[product_load_type_language] AS trgt
USING	(VALUES
		(0,0,'NO_LOAD'),
		(0,1,'NO_LOAD_fr'),
		(0,2,'NO_LOAD_es'),
		(0,3,'NO_LOAD_pt'),
		(1,0,'LOAD_TO_PROD'),
		(1,1,'LOAD_TO_PROD_fr'),
		(1,2,'LOAD_TO_PROD_es'),
		(1,3,'LOAD_TO_PROD_pt'),
		(2,0,'LOAD_TO_DIST'),
		(2,1,'LOAD_TO_DIST_fr'),
		(2,2,'LOAD_TO_DIST_es'),
		(2,3,'LOAD_TO_DIST_pt'),
		(3,0,'LOAD_TO_CENTRE'),
		(3,1,'LOAD_TO_CENTRE_fr'),
		(3,2,'LOAD_TO_CENTRE_es'),
		(3,3,'LOAD_TO_CENTRE_pt'),
		(4,0,'LOAD_TO_EXISTING'),
		(4,1,'LOAD_TO_EXISTING_fr'),
		(4,2,'LOAD_TO_EXISTING_pt'),
		(4,3,'LOAD_TO_EXISTING_es'),
		(5,0,'LOAD_REQUESTS'),
		(5,1,'LOAD_REQUESTS_fr'),
		(5,2,'LOAD_REQUESTS_es'),
		(5,3,'LOAD_REQUESTS_pt'),
		(6,0,'LOAD_REQUESTS_TO_DIST'),
		(6,1,'LOAD_REQUESTS_TO_DIST_fr'),
		(6,2,'LOAD_REQUESTS_TO_DIST_es'),
		(6,3,'LOAD_REQUESTS_TO_DIST_pt')
		) AS src([product_load_type_id],[language_id],[language_text])
ON
	trgt.[product_load_type_id] = src.[product_load_type_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[product_load_type_id] = src.[product_load_type_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([product_load_type_id],[language_id],[language_text])
	VALUES ([product_load_type_id],[language_id],[language_text])

;

