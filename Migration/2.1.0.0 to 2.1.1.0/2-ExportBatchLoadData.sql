USE [indigo_database_main_dev]
GO
INSERT [dbo].[export_batch_statuses] ([export_batch_statuses_id], [export_batch_statuses_name]) VALUES (0, N'CREATED')
GO
INSERT [dbo].[export_batch_statuses] ([export_batch_statuses_id], [export_batch_statuses_name]) VALUES (1, N'APPROVED')
GO
INSERT [dbo].[export_batch_statuses] ([export_batch_statuses_id], [export_batch_statuses_name]) VALUES (2, N'EXPORTED')
GO
INSERT [dbo].[export_batch_statuses] ([export_batch_statuses_id], [export_batch_statuses_name]) VALUES (3, N'REQUEST_EXPORT')
GO
INSERT [dbo].[export_batch_statuses] ([export_batch_statuses_id], [export_batch_statuses_name]) VALUES (4, N'REJECTED')
GO

INSERT INTO export_batch_statuses_language (export_batch_statuses_id, language_id, language_text)
SELECT export_batch_statuses_id, 0, export_batch_statuses_name
FROM export_batch_statuses
GO

INSERT INTO export_batch_statuses_language (export_batch_statuses_id, language_id, language_text)
SELECT export_batch_statuses_id, 1, export_batch_statuses_name + '_fr'
FROM export_batch_statuses
GO

INSERT INTO export_batch_statuses_language (export_batch_statuses_id, language_id, language_text)
SELECT export_batch_statuses_id, 2, export_batch_statuses_name + '_pt'
FROM export_batch_statuses
GO

INSERT INTO export_batch_statuses_language (export_batch_statuses_id, language_id, language_text)
SELECT export_batch_statuses_id, 3, export_batch_statuses_name + '_es'
FROM export_batch_statuses
GO