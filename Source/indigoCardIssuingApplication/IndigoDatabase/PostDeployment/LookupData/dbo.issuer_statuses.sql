

MERGE INTO [dbo].[issuer_statuses] AS trgt
USING	(VALUES
		(0,'ACTIVE'),
		(1,'INACTIVE'),
		(2,'DELETED')
		) AS src([issuer_status_id],[issuer_status_name])
ON
	trgt.[issuer_status_id] = src.[issuer_status_id]
WHEN MATCHED THEN
	UPDATE SET
		[issuer_status_id] = src.[issuer_status_id]
		, [issuer_status_name] = src.[issuer_status_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([issuer_status_id],[issuer_status_name])
	VALUES ([issuer_status_id],[issuer_status_name])

;

