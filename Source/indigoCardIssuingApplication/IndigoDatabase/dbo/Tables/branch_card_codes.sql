CREATE TABLE [dbo].[branch_card_codes] (
    [branch_card_code_id]      INT          NOT NULL,
    [branch_card_code_type_id] INT          NOT NULL,
    [branch_card_code_name]    VARCHAR (50) NOT NULL,
    [branch_card_code_enabled] BIT          NOT NULL,
    [spoil_only]               BIT          NOT NULL,
    [is_exception]             BIT          NOT NULL,
    CONSTRAINT [PK_branch_card_codes] PRIMARY KEY CLUSTERED ([branch_card_code_id] ASC),
    CONSTRAINT [FK_branch_card_codes_branch_card_code_type] FOREIGN KEY ([branch_card_code_type_id]) REFERENCES [dbo].[branch_card_code_type] ([branch_card_code_type_id]),
    CONSTRAINT [IX_branch_card_codes] UNIQUE NONCLUSTERED ([branch_card_code_name] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'branch_card_codes'