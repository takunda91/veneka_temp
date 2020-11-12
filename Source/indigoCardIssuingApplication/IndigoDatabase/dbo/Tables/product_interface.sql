CREATE TABLE [dbo].[product_interface] (
    [interface_type_id]       INT       NOT NULL,
    [product_id]              INT       NOT NULL,
    [connection_parameter_id] INT       NOT NULL,
    [interface_area]          INT       NOT NULL,
    [interface_guid]          CHAR (36) NULL,
    CONSTRAINT [PK_product_interface] PRIMARY KEY CLUSTERED ([interface_type_id] ASC, [product_id] ASC, [interface_area] ASC),
    CONSTRAINT [CK_FILE_PARAMETER] CHECK ([dbo].[FileParameterValidation]([connection_parameter_id],[interface_guid],[interface_type_id])=(1)),
    CONSTRAINT [FK_product_interface_connection_parameters] FOREIGN KEY ([connection_parameter_id]) REFERENCES [dbo].[connection_parameters] ([connection_parameter_id]),
    CONSTRAINT [FK_product_interface_interface_type] FOREIGN KEY ([interface_type_id]) REFERENCES [dbo].[interface_type] ([interface_type_id]),
    CONSTRAINT [FK_product_interface_product] FOREIGN KEY ([product_id]) REFERENCES [dbo].[issuer_product] ([product_id])
);

