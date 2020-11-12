USE [indigo_database_main_dev]
GO

UPDATE [connection_parameter_type]
SET [connection_parameter_type_name] = 'WEBSERVICE'
WHERE [connection_parameter_type_id] = 0

UPDATE [connection_parameter_type]
SET [connection_parameter_type_name] = 'FILE_SYSTEM'
WHERE [connection_parameter_type_id] = 1

UPDATE [connection_parameter_type]
SET [connection_parameter_type_name] = 'THALESHSM'
WHERE [connection_parameter_type_id] = 2



UPDATE [connection_parameter_type_language]
SET language_text = [connection_parameter_type_name]
FROM [connection_parameter_type]
		INNER JOIN [connection_parameter_type_language]
			ON [connection_parameter_type].[connection_parameter_type_id] = [connection_parameter_type_language].connection_parameter_type_id
WHERE [connection_parameter_type_language].language_id = 0

UPDATE [connection_parameter_type_language]
SET language_text = [connection_parameter_type_name] + '_fr'
FROM [connection_parameter_type]
		INNER JOIN [connection_parameter_type_language]
			ON [connection_parameter_type].[connection_parameter_type_id] = [connection_parameter_type_language].connection_parameter_type_id
WHERE [connection_parameter_type_language].language_id = 1

UPDATE [connection_parameter_type_language]
SET language_text = [connection_parameter_type_name] + '_pt'
FROM [connection_parameter_type]
		INNER JOIN [connection_parameter_type_language]
			ON [connection_parameter_type].[connection_parameter_type_id] = [connection_parameter_type_language].connection_parameter_type_id
WHERE [connection_parameter_type_language].language_id = 2

UPDATE [connection_parameter_type_language]
SET language_text = [connection_parameter_type_name] + '_es'
FROM [connection_parameter_type]
		INNER JOIN [connection_parameter_type_language]
			ON [connection_parameter_type].[connection_parameter_type_id] = [connection_parameter_type_language].connection_parameter_type_id
WHERE [connection_parameter_type_language].language_id = 3