

MERGE INTO [dbo].[customer_title] AS trgt
USING	(VALUES
		(0,'MR'),
		(1,'MRS'),
		(2,'MISS'),
		(3,'MS'),
		(4,'PROF'),
		(5,'DR'),
		(6,'REV'),
		(7,'OTHER')
		) AS src([customer_title_id],[customer_title_name])
ON
	trgt.[customer_title_id] = src.[customer_title_id]
WHEN MATCHED THEN
	UPDATE SET
		[customer_title_id] = src.[customer_title_id]
		, [customer_title_name] = src.[customer_title_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([customer_title_id],[customer_title_name])
	VALUES ([customer_title_id],[customer_title_name])

;

