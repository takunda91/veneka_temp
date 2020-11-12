CREATE TABLE [dbo].[customer_type] (
    [customer_type_id]   INT           NOT NULL,
    [customer_type_name] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_customer_type] PRIMARY KEY CLUSTERED ([customer_type_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'customer_type'