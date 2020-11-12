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
           (216, 0, 
				'Cannot delete user group, users are still linked to user group.',
				'Cannot delete user group, users are still linked to user group._fr',
				'Cannot delete user group, users are still linked to user group._pt',
				'Cannot delete user group, users are still linked to user group._es')
GO


