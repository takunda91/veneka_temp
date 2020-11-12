CREATE TABLE [dbo].[customer_account_type_language] (
    [account_type_id] INT           NOT NULL,
    [language_id]     INT           NOT NULL,
    [language_text]   VARCHAR (MAX) NOT NULL,
    CONSTRAINT [FK_customer_account_type_language_account_type_id] FOREIGN KEY ([account_type_id]) REFERENCES [dbo].[customer_account_type] ([account_type_id]),
    CONSTRAINT [FK_customer_account_type_language_language_id] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]), 
    CONSTRAINT [PK_customer_account_type_language] PRIMARY KEY ([account_type_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'customer_account_type_language'