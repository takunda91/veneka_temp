CREATE TABLE [dbo].[product_service_requet_code1] (
    [src1_id] INT           NOT NULL,
    [name]    VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_product_service_requet_code1] PRIMARY KEY CLUSTERED ([src1_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'product_service_requet_code1'