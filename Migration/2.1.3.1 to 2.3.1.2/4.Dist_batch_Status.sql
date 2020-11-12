
INSERT INTO [dbo].[dist_batch_statuses]
           ([dist_batch_statuses_id]
           ,[dist_batch_status_name]
           ,[dist_batch_expected_statuses_id])
     VALUES
           (22
           ,'REMOVED'
           ,Null)
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (22, 0, N'REMOVED')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (22, 1, N'REMOVED_fr')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (22, 2, N'REMOVED_pt')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (22, 3, N'REMOVED_es')
GO

