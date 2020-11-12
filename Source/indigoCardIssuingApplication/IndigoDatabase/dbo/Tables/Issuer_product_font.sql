CREATE TABLE [dbo].[Issuer_product_font] (
    [font_id]       INT           NOT NULL,
    [font_name]     VARCHAR (50)  NOT NULL,
    [resource_path] VARCHAR (200) NULL,
    [DeletedYN]     BIT           NULL,
    CONSTRAINT [PK_Issuer_product_font] PRIMARY KEY CLUSTERED ([font_id] ASC),
    CONSTRAINT [FK_Issuer_product_font_Issuer_product_font] FOREIGN KEY ([font_id]) REFERENCES [dbo].[Issuer_product_font] ([font_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuer_product_font'