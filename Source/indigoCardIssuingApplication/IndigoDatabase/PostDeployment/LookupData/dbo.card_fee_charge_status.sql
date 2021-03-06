

MERGE INTO [dbo].[card_fee_charge_status] AS trgt
USING	(VALUES
		(0,'PENDING'),
		(1,'SUCCESSFUL'),
		(2,'FAILED'),
		(3,'REVERSED')
		) AS src([card_fee_charge_status_id],[card_fee_charge_status_name])
ON
	trgt.[card_fee_charge_status_id] = src.[card_fee_charge_status_id]
WHEN MATCHED THEN
	UPDATE SET
		[card_fee_charge_status_id] = src.[card_fee_charge_status_id]
		, [card_fee_charge_status_name] = src.[card_fee_charge_status_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([card_fee_charge_status_id],[card_fee_charge_status_name])
	VALUES ([card_fee_charge_status_id],[card_fee_charge_status_name])

;

