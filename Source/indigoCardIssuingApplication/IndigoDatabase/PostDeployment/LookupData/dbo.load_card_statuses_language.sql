

MERGE INTO [dbo].[load_card_statuses_language] AS trgt
USING	(VALUES
		(0,0,'LOADED'),
		(0,1,'LOADED_fr'),
		(0,2,'LOADED_pt'),
		(0,3,'LOADED_sp'),
		(1,0,'AVAILABLE'),
		(1,1,'AVAILABLE_fr'),
		(1,2,'AVAILABLE_pt'),
		(1,3,'AVAILABLE_sp'),
		(2,0,'ALLOCATED'),
		(2,1,'ALLOCATED_fr'),
		(2,2,'ALLOCATED_pt'),
		(2,3,'ALLOCATED_sp'),
		(3,0,'REJECTED'),
		(3,1,'REJECTED_fr'),
		(3,2,'REJECTED_pt'),
		(3,3,'REJECTED_sp'),
		(4,0,'CANCELLED'),
		(4,1,'CANCELLED_fr'),
		(4,2,'CANCELLED_pt'),
		(4,3,'CANCELLED_sp'),
		(5,0,'INVALID'),
		(5,1,'INVALID_fr'),
		(5,2,'INVALID_pt'),
		(5,3,'INVALID_sp')
		) AS src([load_card_status_id],[language_id],[language_text])
ON
	trgt.[load_card_status_id] = src.[load_card_status_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[load_card_status_id] = src.[load_card_status_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([load_card_status_id],[language_id],[language_text])
	VALUES ([load_card_status_id],[language_id],[language_text])

;

