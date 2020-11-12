CREATE TABLE [dbo].[product_load_type_language] (
    [product_load_type_id] INT           NOT NULL,
    [language_id]          INT           NOT NULL,
    [language_text]        VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_product_load_type_language] PRIMARY KEY CLUSTERED ([product_load_type_id] ASC, [language_id] ASC),
    CONSTRAINT [FK_product_load_type_language_languages] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]),
    CONSTRAINT [FK_product_load_type_language_product_load_type_language] FOREIGN KEY ([product_load_type_id]) REFERENCES [dbo].[product_load_type] ([product_load_type_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'product_load_type_language'