

MERGE INTO [dbo].[print_statuses] AS trgt
USING	(VALUES
		(0,'CREATED'),
		(1,'SENT_TO_PRINTER'),
		(2,'FAILED'),
		(3,'PRINTED')
		) AS src([print_statuses_id],[print_statuses])
ON
	trgt.[print_statuses_id] = src.[print_statuses_id]
WHEN MATCHED THEN
	UPDATE SET
		[print_statuses_id] = src.[print_statuses_id]
		, [print_statuses] = src.[print_statuses]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([print_statuses_id],[print_statuses])
	VALUES ([print_statuses_id],[print_statuses])

;

