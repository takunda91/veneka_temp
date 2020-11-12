CREATE TABLE [dbo].[customer_type_language] (
    [customer_type_id] INT           NOT NULL,
    [language_id]      INT           NOT NULL,
    [language_text]    VARCHAR (100) NOT NULL,
   CONSTRAINT [FK_customer_type_language_customer_type_id] FOREIGN KEY ([customer_type_id]) REFERENCES [dbo].[customer_type] ([customer_type_id]),
  CONSTRAINT [FK_customer_type_language_language_id] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]), 
    CONSTRAINT [PK_customer_type_language] PRIMARY KEY ([customer_type_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'customer_type_language'