

INSERT INTO [dbo].[report_fields]  ([reportfieldid] ,[reportfieldname]) VALUES  (105,'Product Name')
GO
INSERT INTO [dbo].[reportfields_language]  ([reportfieldid],[language_id],[language_text]) VALUES (105,0,'Product Name')
GO
INSERT INTO [dbo].[reportfields_language]  ([reportfieldid],[language_id],[language_text]) VALUES (105,1,'Product Name_fr')
GO
INSERT INTO [dbo].[reportfields_language]  ([reportfieldid],[language_id],[language_text]) VALUES (105,2,'Product Name_pt')
GO
INSERT INTO [dbo].[reportfields_language]  ([reportfieldid],[language_id],[language_text]) VALUES (105,3,'Product Name_es')
GO
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(24,105,18)