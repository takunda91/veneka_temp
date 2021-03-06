

MERGE INTO [dbo].[product_service_requet_code2] AS trgt
USING	(VALUES
		(0,'Normal authorization'),
		(2,'Online authorization mandatory'),
		(4,'Online authorization mandatory')
		) AS src([src2_id],[name])
ON
	trgt.[src2_id] = src.[src2_id]
WHEN MATCHED THEN
	UPDATE SET
		[src2_id] = src.[src2_id]
		, [name] = src.[name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([src2_id],[name])
	VALUES ([src2_id],[name])

;

