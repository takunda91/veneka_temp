USE [indigo_database_main_dev]
GO

INSERT INTO connection_parameter_type (connection_parameter_type_id, connection_parameter_type_name)
VALUES (3, 'SOCKET')
GO

INSERT INTO connection_parameter_type_language (connection_parameter_type_id, language_id, language_text)
VALUES (3, 0, 'SOCKET'),
		(3, 1, 'SOCKET_fr'),
		(3, 2, 'SOCKET_pt'),
		(3, 3, 'SOCKET_es')