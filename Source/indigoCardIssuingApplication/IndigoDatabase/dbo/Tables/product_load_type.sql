CREATE TABLE [dbo].[product_load_type] (
    [product_load_type_id]   INT           NOT NULL,
    [product_load_type_name] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_product_load_type] PRIMARY KEY CLUSTERED ([product_load_type_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'product_load_type'