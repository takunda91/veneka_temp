CREATE TABLE [dbo].[issuer_interface] (
    [interface_type_id]       INT       NOT NULL,
    [issuer_id]               INT       NOT NULL,
    [connection_parameter_id] INT       NOT NULL,
    [interface_area]          INT       NOT NULL,
    [interface_guid]          CHAR (36) NULL,
    CONSTRAINT [PK_issuer_interface] PRIMARY KEY CLUSTERED ([interface_type_id] ASC, [issuer_id] ASC, [interface_area] ASC),
    CONSTRAINT [FK_issuer_interface_connection_parameters] FOREIGN KEY ([connection_parameter_id]) REFERENCES [dbo].[connection_parameters] ([connection_parameter_id]),
    CONSTRAINT [FK_issuer_interface_interface_type] FOREIGN KEY ([interface_type_id]) REFERENCES [dbo].[interface_type] ([interface_type_id]),
    CONSTRAINT [FK_issuer_interface_issuer] FOREIGN KEY ([issuer_id]) REFERENCES [dbo].[issuer] ([issuer_id])
);

