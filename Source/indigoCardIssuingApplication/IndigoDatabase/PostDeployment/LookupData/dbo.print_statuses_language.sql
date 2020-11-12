

MERGE INTO [dbo].[print_statuses_language] AS trgt
USING	(VALUES
		(0,0,'CREATED'),
		(0,1,'CREATED_fr'),
		(0,2,'CREATED_pt'),
		(0,3,'CREATED_es'),
		(1,0,'SENT_TO_PRINTER'),
		(1,1,'SENT_TO_PRINTER_fr'),
		(1,2,'SENT_TO_PRINTER_pt'),
		(1,3,'SENT_TO_PRINTER_es'),
		(2,0,'FAILED'),
		(2,1,'FAILED_fr'),
		(2,2,'FAILED_pt'),
		(2,3,'FAILED_es'),
		(3,0,'PRINTED'),
		(3,1,'PRINTED_fr'),
		(3,2,'PRINTED_pt'),
		(3,3,'PRINTED_es')
	
		) AS src([print_statuses_id],[language_id],[language_text])
ON
	trgt.[print_statuses_id] = src.[print_statuses_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[print_statuses_id] = src.[print_statuses_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([print_statuses_id],[language_id],[language_text])
	VALUES ([print_statuses_id],[language_id],[language_text])

;

