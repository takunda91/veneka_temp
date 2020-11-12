

INSERT INTO [dbo].[report_fields]
           ([reportfieldid]
           ,[reportfieldname])
     VALUES
           (103
           ,'Origin Branch Name:')
GO

INSERT INTO [dbo].[report_fields]
           ([reportfieldid]
           ,[reportfieldname])
     VALUES
           (107
           ,'Origin Branch Code:')
GO



INSERT INTO [dbo].[reportfields_language]
           ([reportfieldid]
           ,[language_id]
           ,[language_text])
     VALUES
           (103
           ,0
           ,'Origin Branch Name:')
GO

INSERT INTO [dbo].[reportfields_language]
           ([reportfieldid]
           ,[language_id]
           ,[language_text])
     VALUES
           (103
           ,1
           ,'Origin Branch Name_fr')
		   GO
		   INSERT INTO [dbo].[reportfields_language]
           ([reportfieldid]
           ,[language_id]
           ,[language_text])
     VALUES
           (103
           ,2
           ,'Origin Branch Name_pt')
		   GO
		      INSERT INTO [dbo].[reportfields_language]
           ([reportfieldid]
           ,[language_id]
           ,[language_text])
     VALUES
           (103
           ,3
           ,'Origin Branch Name_es')

GO
INSERT INTO [dbo].[reportfields_language]
           ([reportfieldid]
           ,[language_id]
           ,[language_text])
     VALUES
           (107
           ,0
           ,'Origin Branch Code:')
GO

INSERT INTO [dbo].[reportfields_language]
           ([reportfieldid]
           ,[language_id]
           ,[language_text])
     VALUES
           (107
           ,1
           ,'Origin Branch Code_fr')
		   GO
		   INSERT INTO [dbo].[reportfields_language]
           ([reportfieldid]
           ,[language_id]
           ,[language_text])
     VALUES
           (107
           ,2
           ,'Origin Branch Code_pt')
		   GO
		      INSERT INTO [dbo].[reportfields_language]
           ([reportfieldid]
           ,[language_id]
           ,[language_text])
     VALUES
           (107
           ,3
           ,'Origin Branch Code_es')
		   GO
INSERT INTO [dbo].[report_reportfields]
           ([reportid]
           ,[reportfieldid]
           ,[reportfieldorderno])
     VALUES
           (2
           ,103
           ,15)
GO

INSERT INTO [dbo].[report_reportfields]
           ([reportid]
           ,[reportfieldid]
           ,[reportfieldorderno])
     VALUES
           (2
           ,107
           ,15)

