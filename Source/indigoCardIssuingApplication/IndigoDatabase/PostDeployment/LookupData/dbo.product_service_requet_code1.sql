

MERGE INTO [dbo].[product_service_requet_code1] AS trgt
USING	(VALUES
		(1,'International card'),
		(2,'International card - integrated circuit facilities'),
		(5,'National use only'),
		(6,'National use only - integrated circuit facilities'),
		(9,'Test card - online authorization mandatory')
		) AS src([src1_id],[name])
ON
	trgt.[src1_id] = src.[src1_id]
WHEN MATCHED THEN
	UPDATE SET
		[src1_id] = src.[src1_id]
		, [name] = src.[name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([src1_id],[name])
	VALUES ([src1_id],[name])

;

