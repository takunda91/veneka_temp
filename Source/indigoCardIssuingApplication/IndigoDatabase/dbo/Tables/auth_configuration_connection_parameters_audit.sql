CREATE TABLE [dbo].[auth_configuration_connection_parameters_audit] (
    [authentication_configuration_id] INT       NOT NULL,
    [auth_type_id]                    INT       NULL,
    [interface_guid]                  CHAR (36) NULL,
    [connection_parameter_id]         INT       NULL,
    [external_interface_id]           CHAR (36) NULL
);

