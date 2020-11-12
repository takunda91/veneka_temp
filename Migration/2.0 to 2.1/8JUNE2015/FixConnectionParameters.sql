USE [indigo_database_main_dev]
GO

UPDATE [connection_parameters]
SET name_of_file = null
WHERE connection_parameter_type_id != 1