CREATE TABLE [dbo].[customer_residency_language] (
    [resident_id]   INT           NOT NULL,
    [language_id]   INT           NOT NULL,
    [language_text] VARCHAR (MAX) NOT NULL,
  CONSTRAINT [FK_customer_residency_language_language_id]  FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]),
  CONSTRAINT [FK_customer_residency_language_resident_id]  FOREIGN KEY ([resident_id]) REFERENCES [dbo].[customer_residency] ([resident_id]), 
    CONSTRAINT [PK_customer_residency_language] PRIMARY KEY ([resident_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'customer_residency_language'