

MERGE INTO [dbo].[issuer_statuses_language] AS trgt
USING	(VALUES
		(0,0,'ACTIVE'),
		(0,1,'ACTIVE'),
		(0,2,'ACTIVE_pt'),
		(0,3,'ACTIVE_sp'),
		(1,0,'INACTIVE'),
		(1,1,'INACTIF'),
		(1,2,'INACTIVE_pt'),
		(1,3,'INACTIVE_sp'),
		(2,0,'DELETED'),
		(2,1,'SUPPRIME'),
		(2,2,'DELETED_pt'),
		(2,3,'DELETED_sp')
		) AS src([issuer_status_id],[language_id],[language_text])
ON
	trgt.[issuer_status_id] = src.[issuer_status_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[issuer_status_id] = src.[issuer_status_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([issuer_status_id],[language_id],[language_text])
	VALUES ([issuer_status_id],[language_id],[language_text])

;

