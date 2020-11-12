USE [indigo_database_main_dev]
GO

INSERT INTO [dbo].[response_messages]
           ([system_response_code]
           ,[system_area]
           ,[english_response]
           ,[french_response]
           ,[portuguese_response]
           ,[spanish_response])
     VALUES
           (226
           ,0
           ,'Duplicate Fee Scheme Name, pleasew change name.'
           ,'Duplicate Fee Scheme Name, pleasew change name._fr'
           ,'Duplicate Fee Scheme Name, pleasew change name._es'
           ,'Duplicate Fee Scheme Name, pleasew change name._pt')
GO
INSERT INTO [dbo].[response_messages]
           ([system_response_code]
           ,[system_area]
           ,[english_response]
           ,[french_response]
           ,[portuguese_response]
           ,[spanish_response])
     VALUES
           (227
           ,0
           ,'Duplicate Fee Detail Name, pleasew change name.'
           ,'Duplicate Fee Detail Name, pleasew change name._fr'
           ,'Duplicate Fee Detail Name, pleasew change name._es'
           ,'Duplicate Fee Detail Name, pleasew change name._pt')

