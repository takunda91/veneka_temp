USE [indigo_database_main_dev]
GO

--Add card statuses
INSERT INTO [connection_parameter_type] (connection_parameter_type_id, connection_parameter_type_name)
	VALUES (2, 'THALESHSM')

GO


--id	language_name
--0	English
--1	French
--2	Portuguese
--3	Spanish

INSERT INTO [connection_parameter_type_language]
	([connection_parameter_type_id], [language_id], [language_text])
SELECT [connection_parameter_type_id], 0, [connection_parameter_type_name]
FROM [connection_parameter_type] 
WHERE connection_parameter_type_id = 2
GO

INSERT INTO [connection_parameter_type_language]
	([connection_parameter_type_id], [language_id], [language_text])
SELECT [connection_parameter_type_id], 1, [connection_parameter_type_name] + '_fr'
FROM [connection_parameter_type] 
WHERE connection_parameter_type_id = 2
GO

INSERT INTO [connection_parameter_type_language]
	([connection_parameter_type_id], [language_id], [language_text])
SELECT [connection_parameter_type_id], 2, [connection_parameter_type_name] + '_pt'
FROM [connection_parameter_type] 
WHERE connection_parameter_type_id = 2
GO

INSERT INTO [connection_parameter_type_language]
	([connection_parameter_type_id], [language_id], [language_text])
SELECT [connection_parameter_type_id], 3, [connection_parameter_type_name] + '_sp'
FROM [connection_parameter_type] 
WHERE connection_parameter_type_id = 2
GO