

MERGE INTO [dbo].[pin_mailer_reprint_statuses] AS trgt
USING	(VALUES
		(0,'REQUESTED'),
		(1,'APPROVED'),
		(2,'PROCESSING'),
		(3,'COMPLETE'),
		(4,'REJECTED')
		) AS src([pin_mailer_reprint_status_id],[pin_mailer_reprint_status_name])
ON
	trgt.[pin_mailer_reprint_status_id] = src.[pin_mailer_reprint_status_id]
WHEN MATCHED THEN
	UPDATE SET
		[pin_mailer_reprint_status_id] = src.[pin_mailer_reprint_status_id]
		, [pin_mailer_reprint_status_name] = src.[pin_mailer_reprint_status_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([pin_mailer_reprint_status_id],[pin_mailer_reprint_status_name])
	VALUES ([pin_mailer_reprint_status_id],[pin_mailer_reprint_status_name])

;

