CREATE TABLE [dbo].[auth_configuration_connection_parameters] (
    [authentication_configuration_id] INT       NOT NULL,
    [auth_type_id]                    INT       NULL,
    [connection_parameter_id]         INT       NULL,
    [interface_guid]                  CHAR (36) NULL,
    [external_interface_id]           CHAR (36) NULL,
    CONSTRAINT [FK_auth_configuration_connection_parameters_auth_configuration] FOREIGN KEY ([authentication_configuration_id]) REFERENCES [dbo].[auth_configuration] ([authentication_configuration_id]),
    CONSTRAINT [FK_auth_configuration_connection_parameters_auth_type] FOREIGN KEY ([auth_type_id]) REFERENCES [dbo].[auth_type] ([auth_type_id]),
    CONSTRAINT [FK_auth_configuration_connection_parameters_connection_parameters] FOREIGN KEY ([connection_parameter_id]) REFERENCES [dbo].[connection_parameters] ([connection_parameter_id])
);

