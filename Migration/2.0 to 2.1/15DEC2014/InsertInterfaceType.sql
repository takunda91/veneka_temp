USE [indigo_database_main_dev]
GO

--Add card statuses
INSERT INTO [interface_type] (interface_type_id, interface_type_name)
	VALUES (4, 'FILE_LOADER')

GO

--id	language_name
--0	English
--1	French
--2	Portuguese
--3	Spanish

INSERT INTO [interface_type_language]
	(interface_type_id, [language_id], [language_text])
SELECT interface_type_id, 0, interface_type_name
FROM [interface_type]
WHERE interface_type_id >= 4

GO

INSERT INTO [interface_type_language]
	(interface_type_id, [language_id], [language_text])
SELECT interface_type_id, 1, interface_type_name + '_fr'
FROM [interface_type] 
WHERE interface_type_id >= 4
GO

INSERT INTO [interface_type_language]
	(interface_type_id, [language_id], [language_text])
SELECT interface_type_id, 2, interface_type_name + '_pt'
FROM [interface_type] 
WHERE interface_type_id >= 4
GO

INSERT INTO [interface_type_language]
	(interface_type_id, [language_id], [language_text])
SELECT interface_type_id, 3, interface_type_name + '_sp'
FROM [interface_type] 
WHERE interface_type_id >= 4
GO