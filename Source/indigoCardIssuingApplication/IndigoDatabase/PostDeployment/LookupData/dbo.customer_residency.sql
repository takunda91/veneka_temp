

MERGE INTO [dbo].[customer_residency] AS trgt
USING	(VALUES
		(0,'RESIDENT'),
		(1,'NONRESIDENT')
		) AS src([resident_id],[residency_name])
ON
	trgt.[resident_id] = src.[resident_id]
WHEN MATCHED THEN
	UPDATE SET
		[resident_id] = src.[resident_id]
		, [residency_name] = src.[residency_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([resident_id],[residency_name])
	VALUES ([resident_id],[residency_name])

;

