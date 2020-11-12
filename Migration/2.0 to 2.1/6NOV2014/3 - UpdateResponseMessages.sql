USE [indigo_database_main_dev]
GO

INSERT INTO [dbo].[response_messages]
           ([system_response_code]
           ,[system_area]
           ,[english_response]
           ,[french_response]
           ,[portuguese_response]
           ,[spanish_response])
     VALUES (100, 0, 'Batch not in correct status', 'Batch not in correct status_fr', 'Batch not in correct status_pt', 'Batch not in correct status_es')