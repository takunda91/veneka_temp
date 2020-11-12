USE [indigo_database_main_dev]
GO

INSERT INTO [dbo].[file_statuses]
           ([file_status_id]
           ,[file_status])
     VALUES (23, 'MULTIPLE_PRODUCTS_IN_FILE'),
			(24, 'PRODUCT_NOT_ACTIVE'),
			(25, 'NO_LOAD_FOR_PRODUCT'),
			(26, 'ISSUER_NOT_ACTIVE'),
			(27, 'ISSUER_LICENCE_EXPIRED')

GO
INSERT INTO [dbo].[file_statuses_language] ([file_status_id] ,[language_id] ,[language_text])
SELECT [file_status_id], 0, [file_status]
FROM [file_statuses]
WHERE [file_status_id] NOT IN (SELECT [file_status_id] FROM [file_statuses_language] WHERE [language_id] = 0)

INSERT INTO [dbo].[file_statuses_language] ([file_status_id] ,[language_id] ,[language_text])
SELECT [file_status_id], 1, [file_status] + '_fr'
FROM [file_statuses]
WHERE [file_status_id] NOT IN (SELECT [file_status_id] FROM [file_statuses_language] WHERE [language_id] = 1)

INSERT INTO [dbo].[file_statuses_language] ([file_status_id] ,[language_id] ,[language_text])
SELECT [file_status_id], 2, [file_status] + '_pt'
FROM [file_statuses]
WHERE [file_status_id] NOT IN (SELECT [file_status_id] FROM [file_statuses_language] WHERE [language_id] = 2)

INSERT INTO [dbo].[file_statuses_language] ([file_status_id] ,[language_id] ,[language_text])
SELECT [file_status_id], 3, [file_status] + '_es'
FROM [file_statuses]
WHERE [file_status_id] NOT IN (SELECT [file_status_id] FROM [file_statuses_language] WHERE [language_id] = 3)



GO


