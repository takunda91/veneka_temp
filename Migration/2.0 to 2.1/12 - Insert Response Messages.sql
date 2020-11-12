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
           (400
           ,0
           ,'There are no card requests available to create the batch.'
           ,'There are no card requests available to create the batch._fr'
           ,'There are no card requests available to create the batch._pt'
           ,'There are no card requests available to create the batch._es')
GO


