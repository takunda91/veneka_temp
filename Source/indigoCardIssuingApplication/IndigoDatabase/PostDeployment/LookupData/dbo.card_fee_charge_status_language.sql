

MERGE INTO [dbo].[card_fee_charge_status_language] AS trgt
USING	(VALUES
		(0,N'PENDING',0),
		(0,N'PENDING_fr',1),
		(0,N'PENDING_pt',2),
		(0,N'PENDING_es',3),
		(1,N'SUCCESSFUL',0),
		(1,N'SUCCESSFUL_fr',1),
		(1,N'SUCCESSFUL_pt',2),
		(1,N'SUCCESSFUL_es',3),
		(2,N'FAILED',0),
		(2,N'FAILED_fr',1),
		(2,N'FAILED_pt',2),
		(2,N'FAILED_es',3),
		(3,N'REVERSED',0),
		(3,N'REVERSED_fr',1),
		(3,N'REVERSED_pt',2),
		(3,N'REVERSED_es',3)
		) AS src([card_fee_charge_status_id],[langauge_text],[language_id])
ON
	trgt.[card_fee_charge_status_id] = src.[card_fee_charge_status_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[card_fee_charge_status_id] = src.[card_fee_charge_status_id]
		, [langauge_text] = src.[langauge_text]
		, [language_id] = src.[language_id]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([card_fee_charge_status_id],[langauge_text],[language_id])
	VALUES ([card_fee_charge_status_id],[langauge_text],[language_id])

;

