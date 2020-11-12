CREATE TABLE [dbo].[customer_title] (
    [customer_title_id]   INT           NOT NULL,
    [customer_title_name] VARCHAR (100) NOT NULL, 
    CONSTRAINT [PK_customer_title] PRIMARY KEY ([customer_title_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'customer_title'