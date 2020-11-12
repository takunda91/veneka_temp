CREATE TABLE [dbo].[customer_account_type] (
    [account_type_id]   INT           NOT NULL,
    [account_type_name] VARCHAR (100) NOT NULL,
    [active_YN]         BIT           NOT NULL, 
    CONSTRAINT [PK_customer_account_type] PRIMARY KEY ([account_type_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'customer_account_type'