USE [indigo_database_main_dev]
GO

INSERT INTO [pin_batch_type] ([pin_batch_type_id], [pin_batch_type_name])
	VALUES (2, 'REPRINT')
GO

INSERT INTO [pin_mailer_reprint_statuses] (pin_mailer_reprint_status_id, pin_mailer_reprint_status_name)
	VALUES  (0, 'REQUESTED'),
			(1, 'APPROVED'),
			(2, 'PROCESSING'),
			(3, 'COMPLETE'),
			(4, 'REJECTED')

GO

INSERT INTO [pin_mailer_reprint_statuses_language] (pin_mailer_reprint_status_id, [language_id], [language_text])
SELECT pin_mailer_reprint_status_id, 0, pin_mailer_reprint_status_name
FROM [pin_mailer_reprint_statuses]

INSERT INTO [pin_mailer_reprint_statuses_language] (pin_mailer_reprint_status_id, [language_id], [language_text])
SELECT pin_mailer_reprint_status_id, 1, pin_mailer_reprint_status_name + '_fr'
FROM [pin_mailer_reprint_statuses]

INSERT INTO [pin_mailer_reprint_statuses_language] (pin_mailer_reprint_status_id, [language_id], [language_text])
SELECT pin_mailer_reprint_status_id, 2, pin_mailer_reprint_status_name + '_pt'
FROM [pin_mailer_reprint_statuses]

INSERT INTO [pin_mailer_reprint_statuses_language] (pin_mailer_reprint_status_id, [language_id], [language_text])
SELECT pin_mailer_reprint_status_id, 3, pin_mailer_reprint_status_name + '_es'
FROM [pin_mailer_reprint_statuses]

GO

INSERT INTO [pin_batch_type] (pin_batch_type_id, pin_batch_type_name)
	VALUES (2, 'REPRINT')

GO

INSERT INTO [pin_batch_statuses] (pin_batch_statuses_id, pin_batch_statuses_name)
	VALUES (7, 'SENT_TO_PRINTER'),
			(8, 'PIN_PRINTED'),
			(9, 'SENT_TO_CMS'),
			(10, 'PROCESSED_IN_CMS'),
			(11, 'APPROVED')
GO

INSERT INTO [pin_batch_statuses_language] (pin_batch_statuses_id, [language_id], [language_text])
SELECT pin_batch_statuses_id, 0, pin_batch_statuses_name
FROM [pin_batch_statuses]
WHERE pin_batch_statuses_id NOT IN ( SELECT pin_batch_statuses_id FROM [pin_batch_statuses_language] WHERE [language_id] = 0)

INSERT INTO [pin_batch_statuses_language] (pin_batch_statuses_id, [language_id], [language_text] )
SELECT pin_batch_statuses_id, 1, pin_batch_statuses_name + '_fr'
FROM [pin_batch_statuses]
WHERE pin_batch_statuses_id NOT IN ( SELECT pin_batch_statuses_id FROM [pin_batch_statuses_language] WHERE [language_id] = 1)

INSERT INTO [pin_batch_statuses_language] (pin_batch_statuses_id, [language_id], [language_text])
SELECT pin_batch_statuses_id, 2, pin_batch_statuses_name + '_pt'
FROM [pin_batch_statuses]
WHERE pin_batch_statuses_id NOT IN ( SELECT pin_batch_statuses_id FROM [pin_batch_statuses_language] WHERE [language_id] = 2)

INSERT INTO [pin_batch_statuses_language] (pin_batch_statuses_id, [language_id], [language_text])
SELECT pin_batch_statuses_id, 3, pin_batch_statuses_name + '_es'
FROM [pin_batch_statuses]
WHERE pin_batch_statuses_id NOT IN ( SELECT pin_batch_statuses_id FROM [pin_batch_statuses_language] WHERE [language_id] = 3)

GO

ALTER TABLE [issuer]
	ADD [pin_mailer_reprint_YN] bit

GO

UPDATE [issuer]
	SET [pin_mailer_reprint_YN] = 0
GO

ALTER TABLE [issuer]
	ALTER COLUMN [pin_mailer_reprint_YN] bit not null
GO

