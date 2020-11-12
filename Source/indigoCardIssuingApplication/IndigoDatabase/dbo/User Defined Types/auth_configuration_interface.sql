CREATE TYPE [dbo].[auth_configuration_interface] AS TABLE (
    [authentication_configuration_id] INT       NULL,
    [auth_type_id]                    INT       NULL,
    [connection_parameter_id]         INT       NULL,
    [interface_guid]                  CHAR (36) NULL,
    [external_interface_id]           CHAR (36) NULL);

