CREATE TABLE [dbo].[customer_title_language] (
    [customer_title_id] INT           NOT NULL,
    [language_id]       INT           NOT NULL,
    [language_text]     VARCHAR (MAX) NOT NULL,
   CONSTRAINT [FK_customer_title_language_customer_title_id] FOREIGN KEY ([customer_title_id]) REFERENCES [dbo].[customer_title] ([customer_title_id]),
   CONSTRAINT [FK_customer_title_language_customer_language_id] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]), 
    CONSTRAINT [PK_customer_title_language] PRIMARY KEY ([customer_title_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'customer_title_language'