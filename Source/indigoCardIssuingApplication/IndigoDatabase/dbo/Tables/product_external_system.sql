CREATE TABLE [dbo].[product_external_system] (
    [external_system_field_id] INT           NOT NULL,
    [product_id]               INT           NOT NULL,
    [field_value]              VARCHAR (150) NOT NULL,
    CONSTRAINT [PK_product_external_system] PRIMARY KEY CLUSTERED ([external_system_field_id] ASC, [product_id] ASC),
    CONSTRAINT [FK_external_system_product] FOREIGN KEY ([external_system_field_id]) REFERENCES [dbo].[external_system_fields] ([external_system_field_id]),
    CONSTRAINT [FK_product_external_system] FOREIGN KEY ([product_id]) REFERENCES [dbo].[issuer_product] ([product_id])
);

