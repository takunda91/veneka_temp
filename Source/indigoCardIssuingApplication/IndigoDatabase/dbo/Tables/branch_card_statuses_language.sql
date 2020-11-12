CREATE TABLE [dbo].[branch_card_statuses_language] (
    [branch_card_statuses_id] INT           NOT NULL,
    [language_id]             INT           NOT NULL,
    [language_text]           VARCHAR (100) NOT NULL,
    CONSTRAINT [FK_branch_card_statuses_language_branch_status] FOREIGN KEY ([branch_card_statuses_id]) REFERENCES [dbo].[branch_card_statuses] ([branch_card_statuses_id]),
    CONSTRAINT [FK_branch_card_statuses_language_language_id] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]), 
    CONSTRAINT [PK_branch_card_statuses_language] PRIMARY KEY ([branch_card_statuses_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'branch_card_statuses_language'