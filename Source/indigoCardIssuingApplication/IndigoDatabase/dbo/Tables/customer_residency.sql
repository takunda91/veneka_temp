CREATE TABLE [dbo].[customer_residency] (
    [resident_id]    INT           NOT NULL,
    [residency_name] VARCHAR (100) NOT NULL, 
    CONSTRAINT [PK_customer_residency] PRIMARY KEY ([resident_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'customer_residency'