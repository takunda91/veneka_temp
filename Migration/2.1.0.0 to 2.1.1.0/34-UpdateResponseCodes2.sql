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
           (228
           ,26
           ,'Parameter already in use on another file interface, please use a different parameter.'
           ,'Parameter already in use on another file interface, please use a different parameter._fr'
           ,'Parameter already in use on another file interface, please use a different parameter._pt'
           ,'Parameter already in use on another file interface, please use a different parameter._es')
GO
