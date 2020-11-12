CREATE TABLE [dbo].[card_priority_language] (
    [card_priority_id] INT            NOT NULL,
    [language_id]      INT            NOT NULL,
    [language_text]    NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [FK_card_priority_language] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]), 
    CONSTRAINT [PK_card_priority_language] PRIMARY KEY ([card_priority_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'card_priority_language'