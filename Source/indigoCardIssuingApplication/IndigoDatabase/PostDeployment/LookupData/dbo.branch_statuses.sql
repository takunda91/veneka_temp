

MERGE INTO [dbo].[branch_statuses] AS trgt
USING	(VALUES
		(0,'ACTIVE'),
		(1,'INACTIVE'),
		(2,'DELETED')
		) AS src([branch_status_id],[branch_status])
ON
	trgt.[branch_status_id] = src.[branch_status_id]
WHEN MATCHED THEN
	UPDATE SET
		[branch_status_id] = src.[branch_status_id]
		, [branch_status] = src.[branch_status]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([branch_status_id],[branch_status])
	VALUES ([branch_status_id],[branch_status])

;

