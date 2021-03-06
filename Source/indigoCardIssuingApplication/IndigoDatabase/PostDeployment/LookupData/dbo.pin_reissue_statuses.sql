

MERGE INTO [dbo].[pin_reissue_statuses] AS trgt
USING	(VALUES
		(0,'REQUESTED'),
		(1,'APPROVED'),
		(2,'REJECTED'),
		(3,'UPLOADED'),
		(4,'EXPIRED'),
		(5,'CANCEL')
		) AS src([pin_reissue_statuses_id],[pin_reissue_statuses_name])
ON
	trgt.[pin_reissue_statuses_id] = src.[pin_reissue_statuses_id]
WHEN MATCHED THEN
	UPDATE SET
		[pin_reissue_statuses_id] = src.[pin_reissue_statuses_id]
		, [pin_reissue_statuses_name] = src.[pin_reissue_statuses_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([pin_reissue_statuses_id],[pin_reissue_statuses_name])
	VALUES ([pin_reissue_statuses_id],[pin_reissue_statuses_name])

;

