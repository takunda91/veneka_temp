INSERT INTO [dbo].[reports]([Reportid],[ReportName]) Values(25,'Cards InProgress Report')
GO
INSERT INTO [dbo].[report_fields]  ([reportfieldid] ,[reportfieldname]) VALUES  (104,'Cards InProgress Report')
GO
INSERT INTO [dbo].[report_fields]  ([reportfieldid] ,[reportfieldname]) VALUES  (106,'Cards Status')
GO
INSERT INTO [dbo].[reportfields_language]  ([reportfieldid],[language_id],[language_text]) VALUES (104,0,'Cards InProgress Report')
GO
INSERT INTO [dbo].[reportfields_language]  ([reportfieldid],[language_id],[language_text]) VALUES (104,1,'Cards InProgress Report_fr')
GO
INSERT INTO [dbo].[reportfields_language]  ([reportfieldid],[language_id],[language_text]) VALUES (104,2,'Cards InProgress Report_pt')
GO
INSERT INTO [dbo].[reportfields_language]  ([reportfieldid],[language_id],[language_text]) VALUES (104,3,'Cards InProgress Report_es')
GO
INSERT INTO [dbo].[reportfields_language]  ([reportfieldid],[language_id],[language_text]) VALUES (106,0,'Cards Status')
GO
INSERT INTO [dbo].[reportfields_language]  ([reportfieldid],[language_id],[language_text]) VALUES (106,1,'Cards Status_fr')
GO
INSERT INTO [dbo].[reportfields_language]  ([reportfieldid],[language_id],[language_text]) VALUES (106,2,'Cards Status_pt')
GO
INSERT INTO [dbo].[reportfields_language]  ([reportfieldid],[language_id],[language_text]) VALUES (106,3,'Cards Status_es')
GO


INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,104,1)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,31,2)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,32,3)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,33,4)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,34,5)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,35,6)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,8,7)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,37,8)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,47,9)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,38,10)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,39,11)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,48,12)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,49,13)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,4,14)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,45,15)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,43,16)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,44,17)
INSERT INTO [dbo].[report_reportfields] ([reportid],[reportfieldid],[reportfieldorderno]) values(25,106,18)












