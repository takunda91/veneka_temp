

MERGE INTO [dbo].[product_service_requet_code3] AS trgt
USING	(VALUES
		(0,'PIN required'),
		(1,'No restrictions - normal cardholder verification'),
		(2,'Goods and services only'),
		(3,'PIN required, ATM only'),
		(5,'PIN required, goods and services only at POS, cash at ATM'),
		(6,'PIN required if PIN pad present'),
		(7,'PIN required if PIN pad present, goods and services only at POS, cash at ATM')
		) AS src([src3_id],[name])
ON
	trgt.[src3_id] = src.[src3_id]
WHEN MATCHED THEN
	UPDATE SET
		[src3_id] = src.[src3_id]
		, [name] = src.[name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([src3_id],[name])
	VALUES ([src3_id],[name])

;

