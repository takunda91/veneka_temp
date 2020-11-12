CREATE TABLE [dbo].[product_service_requet_code2] (
    [src2_id] INT           NOT NULL,
    [name]    VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_product_service_requet_code2] PRIMARY KEY CLUSTERED ([src2_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'product_service_requet_code2'