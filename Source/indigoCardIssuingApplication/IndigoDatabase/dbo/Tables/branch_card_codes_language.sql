CREATE TABLE [dbo].[branch_card_codes_language] (
    [branch_card_code_id] INT            NOT NULL,
    [language_id]         INT            NOT NULL,
    [language_text]       VARCHAR (1000) NULL,
    CONSTRAINT [PK_branch_card_codes_language] PRIMARY KEY CLUSTERED ([language_id] ASC, [branch_card_code_id] ASC),
    CONSTRAINT [FK_branch_card_codes_language_branch_card_codes] FOREIGN KEY ([branch_card_code_id]) REFERENCES [dbo].[branch_card_codes] ([branch_card_code_id]),
    CONSTRAINT [FK_branch_card_codes_language_languages] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'branch_card_codes_language'