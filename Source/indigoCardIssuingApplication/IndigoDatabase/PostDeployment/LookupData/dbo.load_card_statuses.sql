

MERGE INTO [dbo].[load_card_statuses] AS trgt
USING	(VALUES
		(0,'LOADED'),
		(1,'AVAILABLE'),
		(2,'ALLOCATED'),
		(3,'REJECTED'),
		(4,'CANCELLED'),
		(5,'INVALID')
		) AS src([load_card_status_id],[load_card_status])
ON
	trgt.[load_card_status_id] = src.[load_card_status_id]
WHEN MATCHED THEN
	UPDATE SET
		[load_card_status_id] = src.[load_card_status_id]
		, [load_card_status] = src.[load_card_status]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([load_card_status_id],[load_card_status])
	VALUES ([load_card_status_id],[load_card_status])

;

