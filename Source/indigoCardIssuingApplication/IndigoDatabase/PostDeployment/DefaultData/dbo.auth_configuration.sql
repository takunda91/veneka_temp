
SET IDENTITY_INSERT [dbo].[auth_configuration] ON;

DECLARE @auth_config_id INT = 1;
DECLARE @auth_config_name VARCHAR(100) = 'Indigo User Auth';

-- Check if the default indigo auth is in the database. Using the name field as the ID might be different on older versions of Indigo
IF NOT EXISTS (SELECT [authentication_configuration_id] FROM [dbo].[auth_configuration] WHERE [authentication_configuration] = @auth_config_name)
BEGIN
	INSERT INTO [dbo].[auth_configuration] ([authentication_configuration_id], [authentication_configuration]) VALUES (@auth_config_id, @auth_config_name)

	INSERT INTO [dbo].[auth_configuration_connection_parameters] (authentication_configuration_id, auth_type_id, connection_parameter_id, interface_guid, external_interface_id)
	VALUES (@auth_config_id,NULL,NULL,NULL,NULL)
END

SET IDENTITY_INSERT [dbo].[auth_configuration] OFF;